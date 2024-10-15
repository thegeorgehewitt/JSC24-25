using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Custom.Decorative
{
    public class FieldOfView : MonoBehaviour
    {
        [Header("PREVIEW")]
        [SerializeField] public bool preview = true;
        [SerializeField] public Color color = Color.cyan;

        [Header("FIELD OF VIEW")]
        [SerializeField] public float radius = 5.0f;
        [Range(0, 360)]
        [SerializeField] public float angle = 45.0f;
        [Range(0, 360)]
        [SerializeField] public float rotation = 0.0f;

        private Vector3 from { get { return RotatePoint(transform.up, (Mathf.Deg2Rad * (rotation + angle / 2))).normalized; } }
        private Vector3 to { get { return RotatePoint(transform.up, (Mathf.Deg2Rad * (rotation - angle / 2))).normalized; } }



#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!preview) return;

            Handles.color = color;
            Gizmos.color = color;

            Handles.DrawWireArc(transform.position, Vector3.back, from, angle, radius);
            Gizmos.DrawLine(transform.position, from * radius + transform.position);
            Gizmos.DrawLine(transform.position, to * radius + transform.position);
        }
#endif



        private Vector3 RotatePoint(Vector3 _point, float _angleRad)
        {
            float s = Mathf.Sin(_angleRad);
            float c = Mathf.Cos(_angleRad);

            return new Vector2
            (
                _point.x * c - _point.y * s,
                _point.x * s + _point.y * c
            );
        }



        public bool InFieldOfView(Vector3 _direction)
        {
            return (Vector3.Cross(from, _direction).z * Vector3.Cross(from, to).z >= 0)
                && (Vector3.Cross(to, _direction).z * Vector3.Cross(to, from).z >= 0);
        }
    }
}
