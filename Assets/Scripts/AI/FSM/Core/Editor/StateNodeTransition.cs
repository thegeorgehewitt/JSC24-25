using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

using Custom.Editor;

namespace Custom.FSM.Editor
{
    public class StateNodeTransition : GraphElement
    {
        private StateTransition transitionData;

        // The start of the transition curve in local space of parent element.
        private Vector2 localStartPoint;

        // The end of the transition curve in local space of parent element.
        private Vector2 localEndPoint;

        private float startControlWeight = 0.4f;
        private float endControlWeight = 0.4f;
        private float minTangentLength = 20.0f;

        private bool drawStartArrow;
        private bool drawEndArrow;
        private float arrowSize = 10.0f;

        private Color lineColor = Color.white;
        private float lineWidth = 1.5f;

        private Orientation orientation;

        private Vector2 OrientationVector => orientation == Orientation.Horizontal ? Vector2.right : Vector2.up;

        private float FlatLength => orientation == Orientation.Horizontal ? (EndPoint.x - StartPoint.x) : (EndPoint.y - StartPoint.y);

        private float XDiff =>
            ((EndNode == null) ? localEndPoint.x : EndNode.layout.center.x) - ((StartNode == null) ? localStartPoint.x : StartNode.layout.center.x);

        private float YDiff =>
            ((EndNode == null) ? localEndPoint.y : EndNode.layout.center.y) - ((StartNode == null) ? localStartPoint.y : StartNode.layout.center.y);



        public StateNode StartNode { get; set; }

        public StateNode EndNode { get; set; }

        /// <summary>
        /// While true, <see cref="EndPoint"/> while be updated to match the mouse cursor position.
        /// </summary>
        public bool EndFollowsCursor { get; set; }

        /// <summary>
        /// The start of the transition curve in world space.
        /// </summary>
        public Vector2 StartPoint
        {
            get
            {
                if (parent != null)
                {
                    return parent.LocalToWorld(localStartPoint);
                }

                return localStartPoint;
            }
            set
            {
                if (parent != null)
                {
                    value = parent.WorldToLocal(value);
                }

                if (drawStartArrow)
                {
                    value += Mathf.Sign(FlatLength) * arrowSize * 0.86f * OrientationVector;
                }

                if (localStartPoint != value)
                {
                    localStartPoint = value;

                    RecalculatePosition();
                    MarkDirtyRepaint();
                }
            }
        }

        /// <summary>
        /// The end of the transition curve in world space.
        /// </summary>
        public Vector2 EndPoint
        {
            get
            {
                if (parent != null)
                {
                    return parent.LocalToWorld(localEndPoint);
                }

                return localEndPoint;
            }
            set
            {
                if (parent != null)
                {
                    value = parent.WorldToLocal(value);
                }

                if (drawStartArrow)
                {
                    value += Mathf.Sign(FlatLength) * arrowSize * 0.86f * -OrientationVector;
                }

                if (localEndPoint != value)
                {
                    localEndPoint = value;

                    RecalculatePosition();
                    MarkDirtyRepaint();
                }
            }
        }

        /// <summary>
        /// Should the arrow at the starting point be drawn?
        /// </summary>
        public bool DrawStartArrow
        {
            get
            {
                return drawStartArrow;
            }
            set
            {
                if (drawStartArrow == value) return;

                drawStartArrow = value;
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Should the arrow at the ending point be drawn?
        /// </summary>
        public bool DrawEndArrow
        {
            get
            {
                return drawEndArrow;
            }
            set
            {
                if (drawEndArrow == value) return;

                drawEndArrow = value; 
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Edge length of arrows (a perfect triangle) in pixel.
        /// </summary>
        public float ArrowSize
        {
            get
            {
                return arrowSize;
            }
            set
            {
                if (arrowSize == value) return;

                arrowSize = value;
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Width of the transition curve.
        /// </summary>
        public float LineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                if (lineWidth == value) return;

                lineWidth = value;
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Rendered color of the transition curve.
        /// </summary>
        public Color LineColor
        {
            get
            {
                return lineColor;
            }
            set
            {
                if (lineColor == value) return;

                lineColor = value;
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// Tangent point of transition curve's starting point in world space.
        /// </summary>
        public Vector2 StartTangent
        {
            get
            {
                float weight = (Mathf.Abs(FlatLength * startControlWeight) > minTangentLength) ? FlatLength * startControlWeight : minTangentLength;

                return StartPoint + weight * OrientationVector;
            }
        }

        /// <summary>
        /// Tangent point of transition curve's ending point in world space.
        /// </summary>
        public Vector2 EndTangent
        {
            get
            {
                float weight = (Mathf.Abs(FlatLength * endControlWeight) > minTangentLength) ? FlatLength * endControlWeight : minTangentLength;

                return EndPoint + weight * -OrientationVector;
            }
        }





        public StateNodeTransition()
        {
            capabilities |=
                Capabilities.Selectable |
                Capabilities.Deletable |
                Capabilities.Ascendable;

            style.paddingTop = 0;
            style.paddingBottom = 0;
            style.paddingLeft = 0;
            style.paddingRight = 0;
            style.marginTop = 0;
            style.marginBottom = 0;
            style.marginLeft = 0;
            style.marginRight = 0;

            generateVisualContent += DrawTransitionArrow;
        }



        /// <summary>
        /// Snap the transition to a <see cref="StateNode"/> bounds.
        /// </summary>
        /// <param name="_node">        The target <see cref="StateNode"/>. </param>
        /// <param name="_direction">   Snap start or end of the transition? (input = end, output = start). </param>
        /// <param name="_side">        Which side of the rect to snap to. </param>
        /// <param name="_padding">     Offset distance from <paramref name="_node"/> world bounds. </param>
        public void SnapToNode(StateNode _node, Direction _direction, RectSide _side)
        {
            if (_node == null) return;

            Vector2 worldPos = Vector2.zero;

            switch (_side)
            {
                case RectSide.Top:
                    worldPos += new Vector2(_node.worldBound.center.x, _node.worldBound.yMin);
                    break;

                case RectSide.Bottom:
                    worldPos += new Vector2(_node.worldBound.center.x, _node.worldBound.yMax);
                    break;

                case RectSide.Left:
                    worldPos += new Vector2(_node.worldBound.xMin, _node.worldBound.center.y);
                    break;

                case RectSide.Right:
                    worldPos += new Vector2(_node.worldBound.xMax, _node.worldBound.center.y);
                    break;

                default: break;
            }

            if (_direction == Direction.Input)
            {
                EndPoint = worldPos;
            }
            else
            {
                StartPoint = worldPos;
            }
        }



        #region Overrides
        public override void OnSelected()
        {
            base.OnSelected();

            LineColor = Color.yellow;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();

            LineColor = Color.white;
        }



        public override bool HitTest(Vector2 _localPoint)
        {
            float distance = HandleUtility.DistancePointBezier(this.LocalToWorld(_localPoint), StartPoint, EndPoint, StartTangent, EndTangent);

            return distance <= lineWidth + 5.0f;
        }

        public override bool Overlaps(Rect _rectangle)
        {
            return HitTest(_rectangle.center);
        }
        #endregion

        #region Draws
        private void DrawTransitionArrow(MeshGenerationContext _mgc)
        {
            DrawCurve(_mgc.painter2D);

            if (drawStartArrow)
                DrawArrow(_mgc.painter2D, GetBezierPoint(0), GetBezierTangent(0), false);

            if (drawEndArrow)
                DrawArrow(_mgc.painter2D, GetBezierPoint(1), GetBezierTangent(1), true);
        }

        private void DrawCurve(Painter2D _painter)
        {
            _painter.strokeColor = lineColor;
            _painter.lineWidth = lineWidth;

            _painter.BeginPath();
            _painter.MoveTo(this.WorldToLocal(StartPoint));
            _painter.BezierCurveTo(this.WorldToLocal(StartTangent), this.WorldToLocal(EndTangent), this.WorldToLocal(EndPoint));

            _painter.Stroke();
        }

        private void DrawArrow(Painter2D _painter, Vector2 _position, Vector2 _direction, bool _startFromBase = false)
        {
            _painter.fillColor = lineColor;

            _direction.Normalize();
            Vector2 left = Quaternion.Euler(0, 0, 150) * _direction * arrowSize;
            Vector2 right = Quaternion.Euler(0, 0, -150) * _direction * arrowSize;

            _position = this.WorldToLocal(_position);

            if (_startFromBase)
                _position += _direction * 0.86f * arrowSize;

            _painter.BeginPath();
            _painter.MoveTo(_position);
            _painter.LineTo(_position + left);
            _painter.LineTo(_position + right);
            _painter.LineTo(_position);

            _painter.Fill();
        }

        private void DrawRect(Painter2D _painter)
        {
            _painter.strokeColor = Color.green;
            _painter.lineWidth = lineWidth;

            _painter.BeginPath();
            _painter.MoveTo(Vector2.zero);
            _painter.LineTo(Vector2.zero + Vector2.right * resolvedStyle.width);
            _painter.LineTo(Vector2.zero + Vector2.up * resolvedStyle.height + Vector2.right * resolvedStyle.width);
            _painter.LineTo(Vector2.zero + Vector2.up * resolvedStyle.height);
            _painter.LineTo(Vector2.zero);

            _painter.Stroke();
        }
        #endregion

        #region Math
        private Vector2 GetBezierPoint(float _t)
        {
            // Compute Bezier point at _t using cubic Bezier formula.
            return Mathf.Pow(1 - _t, 3) * StartPoint +
                   3 * Mathf.Pow(1 - _t, 2) * _t * StartTangent +
                   3 * (1 - _t) * Mathf.Pow(_t, 2) * EndTangent +
                   Mathf.Pow(_t, 3) * EndPoint;
        }

        private Vector2 GetBezierTangent(float _t)
        {
            // Compute derivative of cubic Bezier to get tangent at _t.
            return 3 * Mathf.Pow(1 - _t, 2) * (StartTangent - StartPoint) +
                   6 * (1 - _t) * _t * (EndTangent - StartTangent) +
                   3 * Mathf.Pow(_t, 2) * (EndPoint - EndTangent);
        }

        private void RecalculatePosition()
        {
            float width = Mathf.Abs(XDiff);
            float height = Mathf.Abs(YDiff);

            //
            // ================================================================================ TO BE FIXED 
            //
            //RectSide outputSide = GetClosestSide(StartNode.worldBound, EndPoint);

            //if (width >= height)
            //    orientation = Orientation.Horizontal;
            //else
            //    orientation = Orientation.Vertical;

            //if (StartNode != null) SnapToNode(StartNode, Direction.Output, outputSide);
            //if (EndNode != null) SnapToNode(EndNode, Direction.Input, inputSide);

            style.position = Position.Absolute;
            style.left = Mathf.Min(localStartPoint.x, localEndPoint.x);
            style.top = Mathf.Min(localStartPoint.y, localEndPoint.y);
            style.width = Mathf.Abs(XDiff);
            style.height = Mathf.Abs(YDiff);
        }

        private RectSide GetClosestSide(Rect _rect, Vector2 _point)
        {
            float leftDist = Mathf.Abs(_point.x - _rect.xMin);
            float rightDist = Mathf.Abs(_point.x - _rect.xMax);
            float topDist = Mathf.Abs(_point.y - _rect.yMax);
            float bottomDist = Mathf.Abs(_point.y - _rect.yMin);

            float minDist = Mathf.Min(leftDist, rightDist, topDist, bottomDist);

            if (minDist == leftDist) return RectSide.Left;
            if (minDist == rightDist) return RectSide.Right;
            if (minDist == topDist) return RectSide.Top;
            return RectSide.Bottom;
        }
        #endregion
    }
}
