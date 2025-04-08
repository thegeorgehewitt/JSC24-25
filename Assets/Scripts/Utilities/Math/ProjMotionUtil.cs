using UnityEngine;

namespace Custom.Utility
{
    public static class ProjMotionUtil
    {
        public static Vector2 GetInitialVelocity(Vector2 _s0, Vector2 _s1, Vector2 _g, float _t1)
        {
            return (_s1 - _s0 - (_t1 * _t1 * 0.5f * _g)) / _t1;
        }

        public static Vector2 GetInitialVelocity(Vector2 _s0, Vector2 _sp, Vector2 _g)
        {
            float y = Mathf.Sqrt(-2.0f * _g.y * (_sp.y - _s0.y));
            float x = (_sp.x - _s0.x) * _g.y / -y;

            return new(x, y);
        }



        public static Vector2 GetPeak(Vector2 _s0, Vector2 _s1, Vector2 _g, float _t1)
        {
            Vector2 v0 = GetInitialVelocity(_s0, _s1, _g, _t1);
            float tPeak = v0.y / -_g.y;

            return _s0 + (v0 * tPeak) + 0.5f * tPeak * tPeak * _g;
        }

        public static Vector2 GetPeak(Vector2 _s0, Vector2 _v0, Vector2 _g)
        {
            float tPeak = _v0.y / -_g.y;

            return _s0 + (_v0 * tPeak) + 0.5f * tPeak * tPeak * _g;
        }



        public static bool GetTimeAtPoint(Vector2 _s0, Vector2 _s1, Vector2 _sp, Vector2 _g, out float _t, bool _passPeak = true)
        {
            Vector2 v0 = GetInitialVelocity(_s0, _sp, _g);
            float cDer = v0.y * v0.y - 2.0f * _g.y * (_s0.y - _s1.y);

            if (cDer < 0)
            {
                _t = float.NaN;
                return false;
            }

            float t1 = (-v0.y + Mathf.Sqrt(cDer)) / _g.y;
            float t2 = (-v0.y - Mathf.Sqrt(cDer)) / _g.y;

            _t = _passPeak ? Mathf.Max(t1, t2) : Mathf.Min(t1, t2);

            return true;
        }



        public static Vector2 GetLandingPoint(Vector2 _s0, Vector2 _s1, Vector2 _g, float _t1)
        {
            Vector2 v0 = GetInitialVelocity(_s0, _s1, _g, _t1);
            float t = 2.0f * v0.y / -_g.y;

            return _s0 + v0 * t - 0.5f * t * t * _g;
        }



        public static Vector2 GetQuadraticControlPointFromMotion(Vector2 _s0, Vector2 _v0, Vector2 _g)
        {
            float tPeak = _v0.y / -_g.y;
            float tTotal = 2.0f * tPeak;
            Vector2 sPeak = GetPeak(_s0, _v0, _g);
            Vector2 s2 = _s0 + _v0 * tTotal + 0.5f * tTotal * tTotal * _g;

            float u = 1 - tPeak;

            return (2.0f * u * tPeak * sPeak) / -(u * u * _s0 + tPeak * tPeak * s2);
        }
    }
}