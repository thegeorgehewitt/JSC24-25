using System.Linq;
using System.Collections;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using Custom.Utility;
using Custom.Controller.General;

namespace Custom.AI.Pathfinding
{
    public class WalkJumpAgentDebug : WalkJumpAgent
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                drawWalk = !drawWalk;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                drawJump = !drawJump;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                drawDrop = !drawDrop;

            if (Input.GetKeyDown(KeyCode.Q))
                drawAllNodes = !drawAllNodes;

            if (Input.GetKeyDown(KeyCode.T))
                drawNodeIndex = Mathf.Min(PathNodes.Length - 1, drawNodeIndex + 1);
            if (Input.GetKeyDown(KeyCode.R))
                drawNodeIndex = Mathf.Max(0, drawNodeIndex - 1);

            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (isStopped)
                    StartFollowCursor();
                else
                    StopDebuggingPathFind();

                isStopped = !isStopped;
            }

            if (followingCursor)
            {
                targetPos = CameraController2D.MainCamera.ScreenToWorldPoint(Input.mousePosition);
                SetTargetLocation(targetPos);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            GUIStyle textStyle = new(EditorStyles.label);
            textStyle.alignment = TextAnchor.LowerCenter;

            if (!Application.isPlaying) return;

            if (drawAllNodes)
            {
                foreach (var node in PathNodes)
                {
                    Vector2 worldPos = navGrid.CellToWorld(node.position).Value;

                    if (node.position == targetPos)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawSphere(worldPos, 0.2f);
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawWireSphere(worldPos, 0.2f);
                    }

                    Handles.Label(worldPos + Vector2.up * 0.25f, new GUIContent($"{node.position}"), textStyle);

                    DrawConnectedPoints(node, false);
                }
            }
            else if (drawNodeIndex >= 0 && drawNodeIndex < PathNodes.Length)
            {
                PathNode node = PathNodes[drawNodeIndex];

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(navGrid.CellToWorld(node.position).Value, 0.2f);

                DrawConnectedPoints(node, true);
            }

            if (FollowingPath)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(TargetLocation, 0.2f);
                if (CurrentPath.Count > 0)
                {
                    Gizmos.DrawLine(TargetLocation, navGrid.CellToWorld(CurrentPath.Last()).Value);
                }

                Gizmos.color = Color.yellow;

                Vector2 previousPos = transform.position;
                Vector2 nextPos;
                foreach (var nextNode in CurrentPath)
                {
                    nextPos = navGrid.CellToWorld(nextNode).Value;

                    Gizmos.DrawLine(previousPos, nextPos);

                    previousPos = nextPos;
                }
            }
        }
#endif



        #region Editor Debug
        private bool drawWalk = true;
        private bool drawJump = true;
        private bool drawDrop = true;
        private bool drawAllNodes = true;
        private int drawNodeIndex = 0;



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
                    Gizmos.color = drawDrop ? new Color(0, 1, 0, 0.5f) : Color.clear;
                    Gizmos.DrawLine(navGrid.CellToWorld(_node.position).Value, navGrid.CellToWorld(linkedNode).Value);
                }
                else if (_node.linkedNodes[linkedNode] == JUMP)
                {
                    Vector2 start = navGrid.CellToWorld(_node.position).Value;
                    Vector2 end = navGrid.CellToWorld(linkedNode).Value;

                    DrawJumpCurve(start, end, 0.05f, drawJump ? new Color(1, 0, 0, 0.5f) : Color.clear);
                }
                else
                {
                    Gizmos.color = drawWalk ? new Color(0, 0, 1, 0.5f) : Color.clear;
                    Gizmos.DrawLine(navGrid.CellToWorld(_node.position).Value, navGrid.CellToWorld(linkedNode).Value);
                }
            }
        }

        private void DrawJumpCurve(Vector2 _p0, Vector2 _p1, float _step, Color _color)
        {
            Gizmos.color = _color;

            float peakOffset;
            if (_p0.y <= _p1.y)
                peakOffset = agentData.height / 2.0f;
            else
                peakOffset = 0;

            if (!GetJumpDuration(_p0, _p1, peakOffset, out float tTotal)) return;

            float tCurrent;
            Vector2 initialVelocity = ProjMotionUtil.GetInitialVelocity(_p0, _p1, agentData.gravityAccel, tTotal);
            Vector2 previousPoint = _p0;
            Vector2 nextPoint;

            for (float t = 0.0f; t <= 1.0f; t += _step)
            {
                tCurrent = tTotal * t;
                nextPoint = _p0 + (initialVelocity * tCurrent) + (0.5f * tCurrent * tCurrent * agentData.gravityAccel);

                Gizmos.DrawLine(previousPoint, nextPoint);

                previousPoint = nextPoint;
            }

            Gizmos.DrawLine(previousPoint, _p1);
        }
        #endregion

        #region Pathfinding Debug
        private Vector2 targetPos;
        private bool followingCursor = false;
        private bool isStopped = true;

        private Coroutine pathfindingDebugCoroutine;



        private void StopDebuggingPathFind()
        {
            if (pathfindingDebugCoroutine != null)
                StopCoroutine(pathfindingDebugCoroutine);

            StopFollowPath(false);

            followingCursor = false;
        }



        private void StartRandomPathfinding()
        {
            pathfindingDebugCoroutine = StartCoroutine(RandomPathfindingCoroutine());
        }

        private IEnumerator RandomPathfindingCoroutine()
        {
            while (true)
            {
                if (!FollowingPath && !Moving)
                {
                    targetPos = new Vector2(
                        Random.Range(navGrid.Min.x, navGrid.Max.x),
                        Random.Range(navGrid.Min.y, navGrid.Max.y));

                    SetTargetLocation(targetPos);
                }

                yield return null;
            }
        }



        private void StartFollowCursor()
        {
            followingCursor = true;
        }
        #endregion
    }
}
