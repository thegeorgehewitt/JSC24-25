using UnityEngine;

namespace Custom.Extensions
{
    public static class RectExtension
    {
        /// <summary>
        /// Get the closest side of a rect based on the distance to each side's midpoint.
        /// </summary>
        /// <param name="_rect"> Source rect. </param>
        /// <param name="_point"> Point to check for. This must be in same coordinate space as <paramref name="_rect"/>. </param>
        /// <returns>
        /// Closest side of <paramref name="_rect"/> to the given <paramref name="_point"/>.
        /// </returns>
        public static RectSide ClosestSide(this Rect _rect, Vector2 _point)
        {
            float topDist = (new Vector2(_rect.center.x, _rect.yMax) - _point).sqrMagnitude;
            float bottomDist = (new Vector2(_rect.center.x, _rect.yMin) - _point).sqrMagnitude;
            float leftDist = (new Vector2(_rect.xMin, _rect.center.y) - _point).sqrMagnitude;
            float rightDist = (new Vector2(_rect.xMax, _rect.center.y) - _point).sqrMagnitude;

            float minDist = Mathf.Min(topDist, bottomDist, leftDist, rightDist);

            if (minDist == leftDist) return RectSide.Left;
            if (minDist == rightDist) return RectSide.Right;
            if (minDist == topDist) return RectSide.Top;
            return RectSide.Bottom;
        }
    }



    [System.Flags]
    public enum RectSide
    {
        Top     = 1 << 0,
        Bottom  = 1 << 1,
        Left    = 1 << 2,
        Right   = 1 << 3,
    }
}