using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.Utility;

namespace Custom.AI.Pathfinding
{
    public class WalkJumpAgent : PathFindAgentBase
    {
        private const int WALK = 1;
        private const int JUMP = 2;
        private const int DROP = 3;



        private readonly Dictionary<Vector2Int, PathNode> pathNodeLookup = new();
        private PathNode[] pathNodes = { };

        private bool drawEqual = true;
        private bool drawHigher = true;
        private bool drawLower = true;
        private bool drawAllNodes = true;
        private int drawNodeIndex = 0;

        private List<Vector2Int> resultPath = new();
        private Vector2Int startNode, endNode;



        private void Start()
        {
            navGrid.RegisterAgent(this);
        }

        private void Update()
        {
            pathNodes = navGrid.GetAgentNodeGraph(this);

            pathNodeLookup.Clear();
            foreach (var pathNode in pathNodes)
            {
                pathNodeLookup.Add(pathNode.position, pathNode);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                drawEqual = !drawEqual;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                drawHigher = !drawHigher;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                drawLower = !drawLower;

            if (Input.GetKeyDown(KeyCode.Q))
                drawAllNodes = !drawAllNodes;

            if (Input.GetKeyDown(KeyCode.T))
                drawNodeIndex = Mathf.Min(pathNodes.Length - 1, drawNodeIndex + 1);
            if (Input.GetKeyDown(KeyCode.R))
                drawNodeIndex = Mathf.Max(0, drawNodeIndex - 1);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                startNode = pathNodes[Random.Range(1, pathNodes.Length - 1)].position;
                endNode = pathNodes[Random.Range(1, pathNodes.Length - 1)].position;

                PathFinding2D.FindPath(this, navGrid, startNode, endNode, resultPath);

                if (resultPath.Count >= 2)
                {
                    StartFollowPath();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Vector2 start = navGrid.CellToWorld(pathNodes[0].position).Value;
            Vector2 end = navGrid.CellToWorld(pathNodes[drawNodeIndex].position).Value;

            DrawCurve(
                start, 
                ProjMotionUtil.GetQuadraticControlPointFromMotion(
                    start, ProjMotionUtil.GetInitialVelocity(start, end, agentData.gravityAccel, GetJumpDuration(start, end)), agentData.gravityAccel), 
                end,
                20, drawHigher ? new Color(1, 0, 0, 0.5f) : Color.clear);

            if (drawAllNodes)
            {
                foreach (var node in pathNodes)
                {
                    if (node.position == startNode)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawSphere(navGrid.CellToWorld(node.position).Value, 0.2f);

                    }
                    else if (node.position == endNode)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(navGrid.CellToWorld(node.position).Value, 0.2f);
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(navGrid.CellToWorld(node.position).Value, 0.2f);
                    }

                    DrawConnectedPoints(node, false);
                }
            }
            else if (drawNodeIndex >= 0 && drawNodeIndex < pathNodes.Length)
            {
                PathNode node = pathNodes[drawNodeIndex];

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(navGrid.CellToWorld(node.position).Value, 0.2f);

                DrawConnectedPoints(node, true);
            }

            if (resultPath.Count >= 2)
            {
                Gizmos.color = Color.yellow;

                var previousNode = resultPath[0];
                foreach (var nextNode in resultPath)
                {
                    Gizmos.DrawLine(navGrid.CellToWorld(previousNode).Value, navGrid.CellToWorld(nextNode).Value);

                    previousNode = nextNode;
                }
            }
        }



        #region Editor Debug
        private void DrawConnectedPoints(PathNode _node, bool _drawPointSphere)
        {
            foreach (var linkedNode in _node.linkedNodes.Keys)
            {
                if (_drawPointSphere)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(navGrid.CellToWorld(linkedNode).Value, 0.2f);
                }

                if (_node.linkedNodes[linkedNode] == DROP)
                {
                    Gizmos.color = drawLower ? new Color(0, 1, 0, 0.5f) : Color.clear;
                    Gizmos.DrawLine(navGrid.CellToWorld(_node.position).Value, navGrid.CellToWorld(linkedNode).Value);
                }
                else if (_node.linkedNodes[linkedNode] == JUMP)
                {
                    Vector2 start = navGrid.CellToWorld(_node.position).Value;
                    Vector2 end = navGrid.CellToWorld(linkedNode).Value;

                    DrawCurve(
                        start, 
                        ProjMotionUtil.GetQuadraticControlPointFromMotion(
                            start, ProjMotionUtil.GetInitialVelocity(start, end, agentData.gravityAccel, GetJumpDuration(start, end)), agentData.gravityAccel), 
                        end, 
                        20, drawHigher ? new Color(1, 0, 0, 0.5f) : Color.clear);
                }
                else
                {
                    Gizmos.color = drawEqual ? new Color(0, 0, 1, 0.5f) : Color.clear;
                    Gizmos.DrawLine(navGrid.CellToWorld(_node.position).Value, navGrid.CellToWorld(linkedNode).Value);
                }
            }
        }

        private void DrawCurve(Vector2 _p0, Vector2 _p1, Vector2 _p2, int _iterations, Color _color)
        {
            Gizmos.color = _color;
            Vector2 previousPoint = _p0;

            for (int i = 1; i <= _iterations; i++)
            {
                float t = i / (float)_iterations;
                Vector2 pointOnCurve = BezierUtil.QuadraticBezier(_p0, _p1, _p2, t);

                Gizmos.DrawLine(previousPoint, pointOnCurve);
                previousPoint = pointOnCurve;
            }
        }
        #endregion

        #region Node Graph Baking
        public override PathNode[] ConnectGraphNodes(Vector2Int[] _nodes)
        {
            Dictionary<Vector2Int, PathNode> map = new(_nodes.Select(e => new KeyValuePair<Vector2Int, PathNode>(e, new PathNode(e))));

            foreach (var node in _nodes)
            {
                int closestRight = int.MaxValue;
                Vector2Int closetRightPoint = node;
                int closestRightMovement = WALK;

                // Find closest point on the same height.
                foreach (var nextNode in _nodes)
                {
                    if (node == nextNode) continue;

                    // Find closest node to the right.
                    if (nextNode.y == node.y 
                        && nextNode.x > node.x
                        && closestRight > nextNode.x
                        && !navGrid.Occupied(node + Vector2Int.right)
                        && !navGrid.Occupied(nextNode + Vector2Int.left))
                    {
                        // Node on same platform.
                        if (navGrid.Occupied(node + new Vector2Int(1, -1))
                            && navGrid.Occupied(nextNode + new Vector2Int(-1, -1)))
                        {
                            closestRight = nextNode.x;
                            closetRightPoint = nextNode;
                            closestRightMovement = WALK;
                        }

                        // Node on different platform.
                        else if (!navGrid.Occupied(node + new Vector2Int(1, -1))
                            && !navGrid.Occupied(nextNode + new Vector2Int(-1, -1))
                            && nextNode.x <= node.x + navGrid.FloorToCell(agentData.jumpDistance) + 1
                            && JumpPossible(node, nextNode))
                        {
                            closestRight = nextNode.x;
                            closetRightPoint = nextNode;
                            closestRightMovement = JUMP;
                        }
                    }

                    // Find drop nodes to either sides.
                    if (nextNode.y >= node.y - navGrid.FloorToCell(agentData.dropHeight) - 1 && nextNode.y < node.y)    // In range of drop height
                        if ((nextNode.x == node.x + 1 && !navGrid.OccupiedFromTo(node + Vector2Int.right, nextNode))    // Not occupied to the right downward.
                        || (nextNode.x == node.x - 1 && !navGrid.OccupiedFromTo(node + Vector2Int.left, nextNode)))     // Not occupied to the left downward.
                            map[node].linkedNodes.TryAdd(nextNode, DROP);

                    // Find jump nodes to either sides.
                    if (nextNode.y <= node.y + navGrid.FloorToCell(agentData.jumpHeight) + 1 && nextNode.y > node.y // In range of vertical jump
                        && nextNode.x <= node.x + navGrid.FloorToCell(agentData.jumpDistance) + 1                   // In range of horizontal jump to the right
                        && nextNode.x >= node.x - navGrid.FloorToCell(agentData.jumpDistance) - 1                   // In range of horizontal jump to the left
                        && JumpPossible(node, nextNode))
                    {
                        map[node].linkedNodes.TryAdd(nextNode, JUMP);
                        map[nextNode].linkedNodes.TryAdd(node, DROP);
                    }
                }

                // Add closet right node
                if (closetRightPoint != node)
                {
                    map[node].linkedNodes.TryAdd(closetRightPoint, closestRightMovement);
                    map[closetRightPoint].linkedNodes.TryAdd(node, closestRightMovement);
                }
            }

            return map.Values.ToArray();
        }

        public override Vector2Int[] GenerateGraphNodes(NavGrid2D _grid)
        {
            HashSet<Vector2Int> result = new();

            for (int x = 0; x < _grid.CellBounds.x; x++)
            {
                for (int y = 0; y < _grid.CellBounds.y; y++)
                {
                    // If current cell walkable.
                    if (_grid.Occupied(new(x, y))) continue;                     

                    // If below cell unwalkable and exists.
                    if (!_grid.Occupied(new(x, y - 1)) || !_grid.Contains(new(x, y - 1))) continue;

                    // If there are walls to either side of the cell or drops to either side of the walls.
                    if (!_grid.Occupied(new(x + 1, y)) && !_grid.Occupied(new(x - 1, y))
                        && _grid.Occupied(new(x + 1, y - 1)) && _grid.Occupied(new(x - 1, y - 1))) continue;

                    result.Add(new(x, y));

                    // Drop left
                    if (navGrid.GetFirstOccupied(new(x + 1, y), new(x + 1, y - navGrid.FloorToCell(agentData.dropHeight) - 1), out Vector2Int cellLeft)
                        && (cellLeft.y + 1 < y))
                    {
                        result.Add(cellLeft + Vector2Int.up);
                    }

                    // Drop right
                    if (navGrid.GetFirstOccupied(new(x - 1, y), new(x - 1, y - navGrid.FloorToCell(agentData.dropHeight) - 1), out Vector2Int cellRight)
                        && (cellRight.y + 1 < y))
                    {
                        result.Add(cellRight + Vector2Int.up);
                    }
                }
            }

            return result.ToArray();
        }



        private bool JumpPossible(Vector2Int _startNode, Vector2Int _endNode)
        {
            Vector2 start = navGrid.CellToWorld(_startNode).Value;
            Vector2 end = navGrid.CellToWorld(_endNode).Value;

            return 
                !navGrid.OccupiedCells(navGrid.QuadraticBezierToCells(
                start, 
                ProjMotionUtil.GetQuadraticControlPointFromMotion(
                    start, ProjMotionUtil.GetInitialVelocity(start, end, agentData.gravityAccel, GetJumpDuration(start, end)), agentData.gravityAccel), 
                end));
        }
        #endregion

        #region Movement
        private Coroutine followPathCoroutine;
        private Coroutine movementCoroutine;

        private bool moving = false;



        protected override void MoveFromTo(Vector2Int _start, Vector2Int _end)
        {
            PathNode startNode = pathNodeLookup[_start];

            if (!startNode.linkedNodes.ContainsKey(_end)) return;

            int movement = startNode.linkedNodes[_end];
            Vector2 start = navGrid.CellToWorld(_start).Value;
            Vector2 end = navGrid.CellToWorld(_end).Value;

            switch (movement)
            {
                case WALK:
                    Walk(start, end);
                    break;

                case DROP:
                    Drop(start, end);
                    break;

                case JUMP:
                    Jump(start, end);
                    break;
            }
        }



        private void StopFollowPath()
        {
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);

            if (followPathCoroutine != null)
                StopCoroutine(followPathCoroutine);

            moving = false;
        }

        private void StartFollowPath()
        {
            StopFollowPath();

            followPathCoroutine = StartCoroutine(FollowPathCoroutine());
        }

        private IEnumerator FollowPathCoroutine()
        {
            int prevIndex = 0, nextIndex = 1;

            while (nextIndex < resultPath.Count)
            {
                if (!moving)
                {
                    MoveFromTo(resultPath[prevIndex], resultPath[nextIndex]);

                    prevIndex++;
                    nextIndex++;
                }

                yield return null;
            }
        }



        private void Walk(Vector2 _start, Vector2 _end)
        {
            movementCoroutine = StartCoroutine(WalkCoroutine(_start, _end));
        }

        private IEnumerator WalkCoroutine(Vector2 _start, Vector2 _end)
        {
            moving = true;
            float elapsed = 0;

            transform.position = _start;
            while (elapsed < (_end - _start).magnitude / agentData.speed)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, _end, agentData.speed * Time.deltaTime);
                yield return null;
            }

            transform.position = _end;
            moving = false;
        }



        private void Drop(Vector2 _start, Vector2 _end)
        {
            movementCoroutine = StartCoroutine(JumpCoroutine(_start, _end, GetJumpDuration(_start, _end)));
        }

        private float EstimateDropDuration(Vector2 _start, Vector2 _end)
        {
            // Initial velocity is calculated using the second equation of motion
            // with original velocity in the Y axis is 0 (since we are dropping).
            return Mathf.Sqrt(2.0f * agentData.gravityAccel.magnitude * (_start - _end).magnitude) / agentData.gravityAccel.magnitude;
        }



        private void Jump(Vector2 _start, Vector2 _end)
        {
            movementCoroutine = StartCoroutine(JumpCoroutine(_start, _end, GetJumpDuration(_start, _end)));
        }

        private IEnumerator JumpCoroutine(Vector2 _start, Vector2 _end, float _t, float _waitTime = 0.0f)
        {
            moving = true;
            Vector2 initialVelocity = ProjMotionUtil.GetInitialVelocity(_start, _end, agentData.gravityAccel, _t);
            Vector3 currentVelocity = initialVelocity;
            float elapsed = 0;

            transform.position = _start;
            while (elapsed < _t)
            {
                transform.position += currentVelocity * Time.deltaTime;

                elapsed += Time.deltaTime;
                currentVelocity = agentData.gravityAccel * elapsed + initialVelocity;

                yield return null;
            }

            transform.position = _end;

            yield return new WaitForSeconds(_waitTime);
            moving = false;
        }

        private float GetJumpDuration(Vector2 _start, Vector2 _end)
        {
            return ProjMotionUtil.GetTimeAtPointPassPeak(_start, _end, Vector2.Max(_start, _end) + navGrid.CellSize * 0.5f, agentData.gravityAccel);
        }
        #endregion
    }
}
