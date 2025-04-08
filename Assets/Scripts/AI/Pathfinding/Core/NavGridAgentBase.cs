using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.AI.Pathfinding
{
    public abstract class NavGridAgentBase : MonoBehaviour
    {
        public event Action<bool> OnPathFindCanceled;



        [Header("NAVIGATION")]
        [SerializeField] protected NavGrid2D navGrid;
        [SerializeField] protected NavGridAgentData agentData;

        private Dictionary<Vector2Int, PathNode> pathNodeLookup = new();
        private PathNode[] pathNodes = { };



        /// <summary>
        /// A valid agent needs to reference a valid <see cref="agentData">agentData</see> 
        /// and assigned to valid <see cref="navGrid">navGrid</see>.
        /// </summary>
        public bool IsValid => agentData && navGrid;

        /// <summary>
        /// Get the generated <see cref="PathNode"/>s of this agent. <br/>
        /// </summary>
        public PathNode[] PathNodes => pathNodes;

        /// <summary>
        /// Get the nav grid this agent is assigned to.
        /// </summary>
        public NavGrid2D NavGrid => navGrid;

        /// <summary>
        /// Is this agent currently following a path called by <see cref="SetTargetLocation(Vector2)"/>?
        /// </summary>
        public bool FollowingPath { get; private set; }

        /// <summary>
        /// Get a list of the current path.
        /// </summary>
        public List<Vector2Int> CurrentPath { get; private set; } = new();

        /// <summary>
        /// Get the current following node if valid, otherwise return the closest path node to the agent position.
        /// </summary>
        public Vector2Int CurrentNode => currentNode != new Vector2Int(-1, -1) ? currentNode : FindClosestPathNode(transform.position).Value.position;

        /// <summary>
        /// Use this value in child classes to control how movement is handled during pathfinding. <br/><br/>
        /// While <see langword="true"/>: pathfinding logic will pause to wait for movement. <br/>
        /// While <see langword="false"/>: pathfinding logic will call <see cref="MoveFromTo(Vector2Int, Vector2Int, int)"/> to next node in current path.
        /// </summary>
        protected bool Moving { get; set; }

        /// <summary>
        /// Get the current movement of the agent while following a path.
        /// A value of -1 means the agent is not following a path.
        /// </summary>
        protected int Movement { get; private set; }

        /// <summary>
        /// Get the current targeted location for pathfinding.
        /// </summary>
        protected Vector3 TargetLocation { get; private set; }



        /// <summary>
        /// Get grounded state of the agent. <br/>
        /// Default behaviour can be overridden in derived classes.
        /// </summary>
        public virtual bool IsGrounded
        {
            get
            {
                return navGrid.Occupied(transform.position - new Vector3(0, 0.05f), agentData.Extents);
            }
        }



        protected virtual void OnEnable()
        {
            NavGrid2D.OnNavGridUpdated += OnNavGridUpdated;
        }

        protected virtual void OnDisable()
        {
            NavGrid2D.OnNavGridUpdated -= OnNavGridUpdated;
        }

        protected virtual void Start()
        {
            foreach (var collider in Physics2D.OverlapPointAll(transform.position))
            {
                if (collider.gameObject.TryGetComponent(out NavGrid2D asNavGrid)) navGrid = asNavGrid;
            }

            if (navGrid != null)
            {
                navGrid.RegisterAgent(this);
            }
            else
            {
                enabled = false;
            }
        }



        /// <summary>
        /// Generate key nodes for pathfinding from the target <paramref name="_grid"/>. <br/>
        /// The agent will only attempt to path find between these points.
        /// </summary>
        /// <param name="_grid">    The source <see cref="NavGrid2D"/> to generate key nodes from. </param>
        /// <returns>
        /// An array of generated key nodes position.
        /// </returns>
        public abstract Vector2Int[] GenerateGraphNodes(NavGrid2D _grid);

        /// <summary>
        /// Connect generated key nodes to determine the final node graph of this agent. <br/><br/>
        /// Adding B to A's list of linked nodes will be an one-way connection A -> B. <br/>
        /// To define a two-way connections, add A to B's list of linked nodes as well.
        /// </summary>
        /// <param name="_nodes">   An array of key nodes. Connections will be done within these nodes only. </param>
        /// <returns>
        /// An array of connected <see cref="PathNode"/>s ready for pathfinding.
        /// </returns>
        public abstract PathNode[] ConnectGraphNodes(Vector2Int[] _nodes);

        /// <summary>
        /// Calculates the nearest reachable node to the given world location.
        /// </summary>
        /// <param name="_worldLocation">   The target world location to calculate cell location of. </param>
        /// <param name="_cellLocation">    <b>OUT:</b> The calculated cell location. <br/>
        ///                                 If the returns value of this method is <see langword="false"/>, this value is invalid. </param>
        /// <returns>
        /// If a valid cell was found, returns <see langword="true"/>. Otherwise, returns <see langword="false"/>.
        /// </returns>
        protected abstract bool GetTargetCell(Vector2 _worldLocation, out Vector2Int _cellLocation);

        /// <summary>
        /// Define movement of the agent from any point <paramref name="_start"/> to point <paramref name="_end"/>.
        /// </summary>
        /// <param name="_start">   The start world location. </param>
        /// <param name="_end">     The end world location. </param>
        /// <param name="_movement">    Movement value defined during <see cref="ConnectGraphNodes"/>. </param>
        /// <param name="_overrideValue">   If <see langword="true"/>, child classes should decide their own movement value. </param>
        protected abstract void MoveFromTo(Vector2 _start, Vector2 _end, int _movement, bool _overrideValue = false);



        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), agentData);   
        }



        #region Node Graph Utility
        /// <summary>
        /// Get the associated <see cref="PathNode"/> at the given <paramref name="_cellLocation"/>.
        /// </summary>
        /// <param name="_cellLocation">    The cell location to check for. </param>
        /// <returns>
        /// The <see cref="PathNode"/> at the given location if exist. Otherwise, returns <see langword="null"/>. 
        /// </returns>
        public PathNode? GetPathNode(Vector2Int _cellLocation)
        {
            if (!pathNodeLookup.ContainsKey(_cellLocation)) return null;

            return pathNodeLookup[_cellLocation];
        }

        /// <param name="_pathNode">    <b>OUT:</b> the <see cref="PathNode"/> at the given cell location. </param>
        /// <returns>
        /// <see langword="true"/> if a valid <see cref="PathNode"/> was found. Otherwise, <see langword="false"/>. 
        /// </returns>
        /// <inheritdoc cref="GetPathNode(Vector2Int)"/>
        public bool GetPathNode(Vector2Int _cellLocation, out PathNode _pathNode)
        {
            if (!pathNodeLookup.ContainsKey(_cellLocation))
            {
                _pathNode = new();
                return false;
            }
            else
            {
                _pathNode = pathNodeLookup[_cellLocation];
                return true;
            }
        }
        #endregion

        #region Pathfind Utility
        /// <summary>
        /// Set the target navigation location for this agent.
        /// </summary>
        /// <param name="_worldLocation">   World space position to set as target. </param>
        /// <returns>
        /// If a reachable cell was found in the agent's NavGrid, returns <see langword="true"/>. Otherwise, returns <see langword="false"/>.
        /// </returns>
        public bool SetTargetLocation(Vector2 _worldLocation)
        {
            // Get the custom defined closest node to the given world location.
            if (!GetTargetCell(_worldLocation, out Vector2Int targetCell)) return false;

            // Get closest path node to start position (transform.position) and end position (world location of targetCell).
            var startNode = FindClosestPathNode(transform.position);
            if (!startNode.HasValue) return false;

            var endNode = FindClosestPathNode(navGrid.CellToWorld(targetCell).Value);
            if (!endNode.HasValue) return false;

            // Get new path.
            List<Vector2Int> newPath = new();
            if (!PathFinding2D.FindPath(this, startNode.Value.position, endNode.Value.position, newPath)) return false;

            // If the current position is closer to the next node than the starting node, skip the starting node.
            if (Vector2.Distance(navGrid.CellToWorld(newPath[0]).Value, navGrid.CellToWorld(newPath[1]).Value)
                > Vector2.Distance(transform.position, navGrid.CellToWorld(newPath[1]).Value))
                newPath.RemoveAt(0);

            StartFollowPath(newPath);

            TargetLocation = _worldLocation;

            return true;
        }

        /// <summary>
        /// Get the closest <see cref="PathNode"/> to the given world location. <br/>
        /// <b>NOTE:</b> This method ignores pathfinding and calculate on pure distances. <br/>
        /// To find the closest reachable cell, use <see cref="FindClosestReachableCell(Vector2)"/> instead.
        /// </summary>
        /// <param name="_worldLocation">   The world location to get the closest <see cref="PathNode"/> of. </param>
        /// <returns>
        /// The closest <see cref="PathNode"/> to the given location if exist. Otherwise, returns <see langword="null"/>. 
        /// </returns>
        public PathNode? FindClosestPathNode(Vector2 _worldLocation)
        {
            PathNode? result = null;
            float minDistance = float.MaxValue;

            foreach (var node in pathNodes)
            {
                float distance = (navGrid.CellToWorld(node.position).Value - _worldLocation).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    result = node;
                }
            }

            return result;
        }

        /// <summary>
        /// Cancel the current path finding (including movement if executing)/
        /// </summary>
        /// <returns>
        /// Whether the operation was successful or not.
        /// </returns>
        public bool CancelPathfinding()
        {
            if (!FollowingPath) return false;

            StopFollowPath(false);

            return true;
        }



        private void OnNavGridUpdated(NavGrid2D _navGrid)
        {
            if (_navGrid != navGrid) return;

            pathNodes = navGrid.GetAgentNodeGraph(this);

            pathNodeLookup.Clear();
            foreach (var pathNode in pathNodes)
            {
                pathNodeLookup.Add(pathNode.position, pathNode);
            }
        }
        #endregion

        #region Movement
        private Vector2Int currentNode = new(-1, -1);

        private Coroutine followPathCoroutine;



        /// <summary>
        /// Stop following <see cref="CurrentPath"/>.
        /// </summary>
        /// <param name="_pathCompleted"> Is follow path stopped after reaching the final node. </param>
        protected void StopFollowPath(bool _pathCompleted)
        {
            if (!FollowingPath) return;

            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine);
                followPathCoroutine = null;
            }

            FollowingPath = false;
            Movement = -1;

            OnPathFindCanceled?.Invoke(_pathCompleted);
        }

        /// <summary>
        /// Start following <see cref="CurrentPath"/>'s nodes in index sequence.
        /// </summary>
        protected void StartFollowPath(List<Vector2Int> _nextPath)
        {
            // Might need optimization, left this for future implementations.
            CurrentPath = _nextPath;

            followPathCoroutine ??= StartCoroutine(FollowPathCoroutine());
        }



        private IEnumerator FollowPathCoroutine()
        {
            FollowingPath = true;

            Vector2Int nextNode = CurrentPath[0];

            // Move to the first node.
            MoveFromTo(
                transform.position,
                navGrid.CellToWorld(nextNode).Value,
                -1,
                true);

            yield return null;

            // Iterate through each node and call child class's movement behaviour defined in MoveFromTo.
            while (CurrentPath.Count > 1)
            {
                if (!Moving)
                {
                    currentNode = nextNode;
                    CurrentPath.RemoveAt(0);
                    nextNode = CurrentPath[0];

                    var pathNode = GetPathNode(currentNode);
                    if (!pathNode.HasValue) break;                                  // If current node is removed during pathfinding.
                    if (!pathNode.Value.linkedNodes.ContainsKey(nextNode)) break;   // If next node is removed during .

                    Movement = pathNode.Value.linkedNodes[nextNode];

                    MoveFromTo(
                        navGrid.CellToWorld(currentNode).Value,
                        navGrid.CellToWorld(nextNode).Value,
                        Movement);
                }

                yield return null;
            }

            yield return new WaitWhile(() => Moving);

            CurrentPath.Clear();

            StopFollowPath(true);
        }
        #endregion
    }
}
