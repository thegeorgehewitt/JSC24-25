using UnityEngine;

namespace Custom.Utility
{
    public static class BezierUtil
    {
        /// <summary>
        /// Compute a point on a cubic Bézier curve at <paramref name="_t"/>. 
        /// </summary>
        /// <param name="_p0">  The Start point of bezier curve. </param>
        /// <param name="_p1">  The Start tangent of bezier curve. </param>
        /// <param name="_p2">  The End point of bezier curve. </param>
        /// <param name="_p3">  The End tangent of bezier curve. </param>
        /// <param name="_t">   The interpolation parameter [0..1]. </param>
        /// <returns>
        /// Point value at <paramref name="_t"/> along the curve.
        /// </returns>
        public static Vector2 CubicBezier(Vector2 _p0, Vector2 _p1, Vector2 _p2, Vector2 _p3, float _t)
        {
            float u = 1 - _t;
            return (u * u * u * _p0) + (3 * u * u * _t * _p1) + (3 * u * _t * _t * _p2) + (_t * _t * _t * _p3);
        }

        /// <summary>
        /// Computes a point on a quadratic Bézier curve given three control points and an interpolation parameter.
        /// </summary>
        /// <param name="_p0">  The starting control point. </param>
        /// <param name="_p1">  The middle control point. </param>
        /// <param name="_p2">  The ending control point. </param>
        /// <param name="_t">   The interpolation parameter [0..1]. </param>
        /// <returns>
        /// The interpolated point on the quadratic Bézier curve.
        /// </returns>
        public static Vector2 QuadraticBezier(Vector2 _p0, Vector2 _p1, Vector2 _p2, float _t)
        {
            float u = 1 - _t;
            return (u * u * _p0) + (2 * _t * u * _p1) + (_t * _t * _p2);
        }

        /// <summary>
        /// Calculates the lengths of the tangents at the start and end of a cubic Bézier curve.
        /// </summary>
        /// <param name="_p0">      Start point of the curve. </param>
        /// <param name="_p3">      End point of the curve. </param>
        /// <param name="_peak">    Highest point of the curve. </param>
        /// <param name="_p1d">     Direction of the start tangent (unit vector). </param>
        /// <param name="_p2d">     Direction of the end tangent (unit vector). </param>
        /// <param name="_p1l">     <b>OUT:</b> Length of start tangent. 
        ///                         Multiplying this with <paramref name="_p1d"/> to get start tangent point. </param>
        /// <param name="_p2l">     <b>OUT:</b> Length of end tangent.
        ///                         Multiplying this with <paramref name="_p2d"/> to get end tangent point. </param>
        public static void CalculateCubicTangentsLength(
            Vector2 _p0, Vector2 _p3, Vector2 _p1d, Vector2 _p2d, Vector2 _peak,
            out float _p1l, out float _p2l)
        {
            // Estimate control points using the peak (approximate assumption)
            Vector2 p1 = new(_p0.x + (_peak.x - _p0.x) * 2.0f / 3.0f, _peak.y);
            Vector2 p2 = new(_p3.x - (_p3.x - _peak.x) * 2.0f / 3.0f, _peak.y);

            _p1l = Mathf.Abs(Vector2.Dot(3 * (p1 - _p0), _p1d.normalized));
            _p2l = Mathf.Abs(Vector2.Dot(3 * (_p3 - p2), _p2d.normalized));
        }
    }
}
