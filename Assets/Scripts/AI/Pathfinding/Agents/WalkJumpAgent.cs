using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.Utility;
using Custom.Manager;

namespace Custom.AI.Pathfinding
{
    public class WalkJumpAgent : NavGridAgentBase
    {
        /*
         * Movement types.
         */
        protected const int WALK = 0;
        protected const int JUMP = 1;
        protected const int DROP = 2;



        private void Awake()
        {
            OnPathFindCanceled += StopMoving;
        }



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
                            && JumpPossible(node, nextNode, agentData.height / 2.0f))
                        {
                            closestRight = nextNode.x;
                            closetRightPoint = nextNode;
                            closestRightMovement = JUMP;
                        }
                    }

                    // Find drop nodes to either sides.
                    if (nextNode.y >= node.y - navGrid.FloorToCell(agentData.dropHeight) && nextNode.y < node.y)    // In range of drop height
                    {
                        Vector2Int dropNode = node + Vector2Int.right * (int)Mathf.Sign(nextNode.x - node.x);
                        Vector2 dropPos = navGrid.CellToWorld(dropNode).Value;
                        Vector2 landPos = navGrid.CellToWorld(nextNode).Value;
                        float dropTime = EstimateDropDuration(node, landPos);
                        float walkTime = Mathf.Abs(dropPos.x - landPos.x) / agentData.speed;

                        if ((dropTime < walkTime                                                                // Need jumping
                            && nextNode.x <= dropNode.x + navGrid.FloorToCell(agentData.jumpDistance) + 1       // In range of horizontal jump to the right
                            && nextNode.x >= dropNode.x - navGrid.FloorToCell(agentData.jumpDistance) - 1       // In range of horizontal jump to the left
                            && JumpPossible(dropNode, nextNode, agentData.height / 2.0f))                       
                        || (dropTime >= walkTime                                                                // Can drop straight.
                            && JumpPossible(dropNode, nextNode)))                                               
                            map[node].linkedNodes.TryAdd(nextNode, DROP);
                    }

                    // Find jump nodes to either sides.
                    if (nextNode.y <= node.y + navGrid.FloorToCell(agentData.jumpHeight) && nextNode.y > node.y // In range of vertical jump
                        && nextNode.x <= node.x + navGrid.FloorToCell(agentData.jumpDistance) + 1               // In range of horizontal jump to the right
                        && nextNode.x >= node.x - navGrid.FloorToCell(agentData.jumpDistance) - 1               // In range of horizontal jump to the left
                        && JumpPossible(node, nextNode, agentData.height / 2.0f))
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

                    result.Add(new(x, y));
                }
            }

            return result.ToArray();
        }



        private bool JumpPossible(Vector2Int _startNode, Vector2Int _endNode, float _jumpPeakOffset = 0.0f, float _step = 0.1f)
        {
            Vector2 start = navGrid.CellToWorld(_startNode).Value;
            Vector2 end = navGrid.CellToWorld(_endNode).Value;
            Vector2 searchLocation;

            if (!GetJumpDuration(start, end, _jumpPeakOffset, out float tTotal)) return false;

            float tCurrent;
            Vector2 initialVelocity = ProjMotionUtil.GetInitialVelocity(start, end, agentData.gravityAccel, tTotal);

            for (float t = 0.0f; t <= 1.0f; t += _step)
            {
                tCurrent = tTotal * t;
                searchLocation = start + (initialVelocity * tCurrent) + (0.5f * tCurrent * tCurrent * agentData.gravityAccel);

                if (navGrid.Occupied(searchLocation, new Vector2(agentData.width, agentData.height) * 0.5f)) return false;
            }

            return true;
        }

        protected bool GetJumpDuration(Vector2 _start, Vector2 _end, float _jumpPeakOffset, out float _t)
        {
            Vector2 peak = Vector2.Max(_start, _end) + new Vector2(0, _jumpPeakOffset);

            bool result = ProjMotionUtil.GetTimeAtPoint(
                _start, _end, peak,
                agentData.gravityAccel, out float t, true);

            _t = t;

            return result;
        }
        #endregion

        #region Movement
        private Coroutine movementCoroutine;



        protected override void MoveFromTo(Vector2 _start, Vector2 _end, int _movement, bool _override)
        {
            if (_override)
            {
                if (_start.y > _end.y)
                    _movement = DROP;
                else if (_start.y < _end.y)
                    _movement = JUMP;
                else
                    _movement = WALK;
            }

            switch (_movement)
            {
                case WALK:
                    Walk(_start, _end);
                    break;

                case DROP:
                    Drop(_start, _end);
                    break;

                case JUMP:
                    Jump(_start, _end);
                    break;
            }
        }



        protected void StopMoving()
        {
            if (Movement != WALK) return;

            ForceStopMoving();
        }

        private void ForceStopMoving()
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null;
            }

            Moving = false;
        }



        private void Walk(Vector2 _start, Vector2 _end)
        {
            movementCoroutine = StartCoroutine(WalkCoroutine(_start, _end));
        }

        private IEnumerator WalkCoroutine(Vector2 _start, Vector2 _end)
        {
            Moving = true;

            transform.position = _start;

            float elapsed = 0;
            float duration = (_end - _start).magnitude / agentData.speed;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, _end, agentData.speed * TimeManager.DeltaTime);
                yield return null;
            }

            transform.position = _end;

            Moving = false;
        }



        private void Drop(Vector2 _start, Vector2 _end)
        {
            movementCoroutine = StartCoroutine(DropCoroutine(_start, _end));
        }

        private IEnumerator DropCoroutine(Vector2 _start, Vector2 _end, float _waitTime = 0.05f)
        {
            Moving = true;

            transform.position = _start;

            // Start moving horizontally.
            Vector3 hVel = agentData.speed * Mathf.Sign(_end.x - _start.x) * Vector3.right;

            while (IsGrounded)
            {
                transform.position += hVel * TimeManager.DeltaTime;

                yield return null;
            }

            // Calculate next movement to test if agent can reach the target destination during falling or not.
            float elapsed = 0;
            float fallDuration = EstimateDropDuration(_start, _end);

            if (Mathf.Abs(transform.position.x - _end.x) / fallDuration > agentData.speed)
            {
                GetJumpDuration(transform.position, _end, agentData.height / 2.0f, out float jumpDuration);

                fallDuration = jumpDuration;
            }

            Vector2 initialVelocity = ProjMotionUtil.GetInitialVelocity(transform.position, _end, agentData.gravityAccel, fallDuration);
            Vector3 currentVelocity = initialVelocity;

            // Start falling or jumping.
            while (elapsed < fallDuration)
            {
                transform.position += currentVelocity * TimeManager.DeltaTime;

                elapsed += Time.deltaTime;
                currentVelocity = agentData.gravityAccel * elapsed + initialVelocity;

                yield return null;
            }

            transform.position = _end;

            yield return new WaitForSeconds(_waitTime);

            Moving = false;
        }

        private float EstimateDropDuration(Vector2 _start, Vector2 _end)
        {
            // Since we are defining down ward distances as negative
            // and _end.y will always greater than _start.y so we need to abstract _end for _start.
            return Mathf.Sqrt(2.0f * (_end.y - _start.y) / agentData.gravityAccel.y);
        }



        private void Jump(Vector2 _start, Vector2 _end)
        {
            movementCoroutine = StartCoroutine(JumpCoroutine(_start, _end));
        }

        private IEnumerator JumpCoroutine(Vector2 _start, Vector2 _end, float _waitTime = 0.05f)
        {
            Moving = true;

            transform.position = _start;

            // Offset transform using kinematic equations.
            GetJumpDuration(_start, _end, agentData.height / 2.0f, out float jumpDuration);
            Vector2 initialVelocity = ProjMotionUtil.GetInitialVelocity(_start, _end, agentData.gravityAccel, jumpDuration);
            Vector3 currentVelocity = initialVelocity;
            float elapsed = 0;

            while (elapsed < jumpDuration)
            {
                transform.position += currentVelocity * TimeManager.DeltaTime;

                elapsed += Time.deltaTime;
                currentVelocity = agentData.gravityAccel * elapsed + initialVelocity;

                yield return null;
            }

            transform.position = _end;

            yield return new WaitForSeconds(_waitTime);

            Moving = false;
        }
        #endregion

        #region Pathfinding
        protected override bool GetTargetCell(Vector2 _worldLocation, out Vector2Int _cellLocation)
        {
            _cellLocation = navGrid.WorldToCell(_worldLocation);

            if (!navGrid.Contains(_cellLocation)) return false;

            var pathNode = FindClosestPathNode(_worldLocation);
            if (!pathNode.HasValue) return false;
            _cellLocation = pathNode.Value.position;

            if (navGrid.GetFirstOccupied(_cellLocation, _cellLocation - new Vector2Int(0, navGrid.CellBounds.y), out Vector2Int projectedNode))
                if ((_worldLocation - navGrid.CellToWorld(pathNode.Value.position).Value).sqrMagnitude 
                    > (_worldLocation - navGrid.CellToWorld(projectedNode + Vector2Int.up).Value).sqrMagnitude)
                    _cellLocation = projectedNode + Vector2Int.up;

            return true;
        }
        #endregion 
    }
}
