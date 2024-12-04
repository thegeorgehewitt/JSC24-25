using System.Collections.Generic;

using UnityEngine;

using Custom.Attribute;
using static UnityEngine.GraphicsBuffer;

namespace Custom.Utility
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FieldOfView : MonoBehaviour
    {
        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float distance;
            public float angle;

            public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle)
            {
                hit = _hit;
                point = _point;
                distance = _distance;
                angle = _angle;
            }
        }

        public struct EdgeInfo
        {
            public Vector3 pointA;
            public Vector3 pointB;

            public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
            {
                pointA = _pointA;
                pointB = _pointB;
            }
        }



        [Header("REFERENCES")]
        [ReadOnly] 
        public MeshFilter viewMeshFilter;

        [Header("FIELD OF VIEW")]
        [SerializeField] private float radius = 5.0f;
        [Range(0, 360)]
        [SerializeField] private float angle = 45.0f;
        [Range(0, 360)]
        [Tooltip("Counter clock-wise rotation offset from transform.up.")]
        [SerializeField] private float rotation = 0.0f;

        [Header("DISPLAY")]
        [SerializeField] [HideInInspector] private bool drawViewMesh = true;
        [SerializeField] [HideInInspector] private float meshResolution = 3;
        [SerializeField] [HideInInspector] private float edgeDistanceThreshold = 0.1f;
        [SerializeField] [HideInInspector] private int edgeResolveIterations = 3;

        [Header("PREVIEW")]
#pragma warning disable CS0414
        [SerializeField] [HideInInspector] private bool preview = true;                 // Used in custom editor
        [SerializeField] [HideInInspector] private Color handlesColor = Color.cyan;     // Used in custom editor
#pragma warning restore CS0414

        [HideInInspector] public ContactFilter2D blockableFilter;

        /// <summary>
        /// Values set to this property will always be clamped to positive value [0...Infinity).
        /// </summary>
        public float Radius 
        { 
            get { return radius; } 
            set { radius = Mathf.Max(value, 0.001f); } 
        }

        /// <summary>
        /// Values set to this property will always be clamped to [0..360].
        /// </summary>
        public float Angle
        {
            get { return angle; }
            set { angle = Mathf.Clamp(value, 0, 360); }
        }

        /// <summary>
        /// <para> Values set to this property will always be converted to be in range [0..360]. </para>
        /// <para> If a negative value is used, It will be recalculated to positive value (e.g. -90 becomes 270). </para>
        /// </summary>
        public float Rotation
        {
            get { return rotation; }
            set { rotation = ((360 + value) % 360); }
        }

        private Mesh viewMesh;



#if UNITY_EDITOR
        private void Reset()
        {
            viewMeshFilter = GetComponent<MeshFilter>();
        }
#endif

        private void Start()
        {
            viewMesh = new();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;

            blockableFilter.useLayerMask = true;
            blockableFilter.useTriggers = false;
        }

        private void Update()
        {
            if (drawViewMesh)
            {
                DrawFieldOfView();
            }
            else
            {
                viewMesh.Clear();
            }
        }



        #region Field of View Logic
        /// <summary>
        /// Find all components of given type in field of view.
        /// </summary>
        /// <typeparam name="T">                Any class inherit from <see cref="Component"/>. </typeparam>
        /// <param name="_targetLayers">        <see cref="LayerMask"/> of objects to retrieve component from. </param>
        /// <param name="_complexDetection">    If <see cref="SpriteRenderer"/> is attached, complex detection will use sprite's physics shape to detect visibility. 
        ///                                     Else, <see cref="Collider2D.bounds"/> is used instead.
        ///                                     Simple detection will only use <see cref="Transform.position"/>. </param>
        /// <returns>
        /// List of all components found.
        /// </returns>
        public List<T> FindAllInView<T>(LayerMask _targetLayers, bool _complexDetection = true) where T : Component
        {
            List<T> visibleTargets = new();
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, radius, _targetLayers);

            foreach (var collider in targetsInViewRadius)
            {
                // Skip any triggers
                if (collider.isTrigger) continue;

                // If no target points is in view angle
                var inViewAnglesPoints = TargetPointsInViewAngle(collider, _complexDetection);
                if (inViewAnglesPoints.Length == 0) continue;

                // If raycast blocked
                var notBlockedPoints = TargetPointsNotBlocked(inViewAnglesPoints);
                if (notBlockedPoints.Length == 0) continue;

                // If does not have component
                if (!collider.gameObject.TryGetComponent(out T asTargetComponent)) continue;

                // Add component to list if valid
                visibleTargets.Add(asTargetComponent);
            }

            return visibleTargets;
        }

        public Vector2 DirectionFromAngle(float _angleInDegrees, bool _angleIsGlobal)
        {
            if (!_angleIsGlobal)
            {
                _angleInDegrees -= transform.eulerAngles.z;
            }

            _angleInDegrees -= rotation;

            return new Vector2(
                Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad), 
                Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad)
            );
        }



        private Vector3[] TargetPointsInViewAngle(Collider2D _collider2D, bool _complex)
        {
            List<Vector3> inViewPoints = new();
            var targetPoints = GetTargetPoints(_collider2D, _complex);

            foreach (var point in targetPoints)
            {
                Vector2 directionToTarget = (point - (Vector2)transform.position).normalized;

                if (Vector2.Angle(Quaternion.Euler(0, 0, rotation) * transform.up, directionToTarget) <= angle / 2)
                {
                    inViewPoints.Add(point);
                }
            }

            return inViewPoints.ToArray();
        }

        private Vector3[] TargetPointsNotBlocked(Vector3[] _points)
        {
            List<Vector3> visiblePoints = new();
            List<RaycastHit2D> hits = new();

            foreach (var point in _points)
            {
                Vector2 directionToTarget = (point - transform.position).normalized;
                float distanceToTarget = Vector3.Distance(point, transform.position);

                if (Physics2D.Raycast(transform.position, directionToTarget, blockableFilter, hits, distanceToTarget) == 0) 
                {
                    visiblePoints.Add(point);
                }
            }

            return visiblePoints.ToArray();
        }

        private Vector2[] GetTargetPoints(Collider2D _collider2D, bool _complex)
        {
            // If simple, check only for Transform.position.
            if (!_complex) return new Vector2[1] { _collider2D.transform.position };

            SpriteRenderer spriteRenderer = _collider2D.gameObject.GetComponentInChildren<SpriteRenderer>();

            // If complex with sprite renderer attached, check for Sprite.PhysicShape vertices.
            if (spriteRenderer)
            {
                List<Vector2> points = new();

                spriteRenderer.sprite.GetPhysicsShape(0, points);

                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = spriteRenderer.transform.TransformPoint(points[i]);
                }

                return points.ToArray();
            }
            // If complex and no sprite renderer attached, check for Collider2D bounds extremes.
            else
            {
                return new Vector2[4]
                {
                _collider2D.bounds.max,
                _collider2D.bounds.min,
                new Vector2(_collider2D.bounds.min.x, _collider2D.bounds.max.y),
                new Vector2(_collider2D.bounds.min.y, _collider2D.bounds.max.x),
                };
            }
        }
        #endregion

        #region Field of View Mesh
        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(angle * meshResolution);

            // Fail-safe when mesh resolution is invalid.
            if (stepCount == 0)
            {
                viewMesh.Clear();
                return;
            }

            float stepAngleSize = angle / stepCount;

            List<Vector3> viewPoints = new();
            ViewCastInfo oldViewCast = new();

            // Generate vertices from ray casts.
            for (int i = 0; i <= stepCount; i++)
            {
                float a = -transform.eulerAngles.z - angle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(a);

                if (i > 0)
                {
                    // Check for edge cases.
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                        if (edge.pointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointA);
                        }
                        if (edge.pointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }

                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            // Apply local transform to vertices and triangles.
            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            // Apply calculated vertices & triangles to view mesh.
            viewMesh.Clear();

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
        {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector3 minPoint = Vector3.zero;
            Vector3 maxPoint = Vector3.zero;

            // We find the "corner" (the most extreme point from the direction of FOV origin) of the collider
            // by casting rays in between the last ray that hit (or not) and the current ray with the opposite hit status.
            // The next ray would replace onr of the original rays depends on whether it hit or not. Then repeat the process.
            for (int i = 0; i < edgeResolveIterations; i++)
            {
                float angle = (minAngle + maxAngle) / 2;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        private ViewCastInfo ViewCast(float _globalAngle)
        {
            Vector3 dir = DirectionFromAngle(_globalAngle, true);
            List<RaycastHit2D> hits = new();
            Physics2D.Raycast(transform.position, dir, blockableFilter, hits, radius);

            if (hits.Count > 0)
            {
                // We only take into account for the first hit since they are the closest.
                return new ViewCastInfo(true, hits[0].point, hits[0].distance, _globalAngle);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * radius, radius, _globalAngle);
            }
        }
        #endregion

        #region Static Functions

        /// <summary>
        /// Determines whether a specified point is within a defined field of view (FOV) cone.
        /// </summary>
        /// <param name="_point">       The point to check in world position. </param>
        /// <param name="_FOVCenter">   The center (origin) of the FOV in world position. </param>
        /// <param name="_FOVRadius">   The radius of the FOV. </param>
        /// <param name="_FOVAngle">    The total angle of the FOV. </param>
        /// <param name="_FOVRotation"> The global rotation of the FOV in degrees. Positive values indicate counter-clockwise rotation. </param>
        /// <returns>
        /// <see langword="true"/> if the point is within the FOV cone; otherwise <see langword="false"/>.
        /// </returns>
        public static bool InViewCone(Vector2 _point, Vector2 _FOVCenter, float _FOVRadius, float _FOVAngle, float _FOVRotation)
        {
            // If point is out of range.
            if (Vector2.Distance(_point, _FOVCenter) > _FOVRadius) return false;

            Vector2 directionToTarget = (_point - _FOVCenter).normalized;

            // If is not in view angle.
            if (Vector2.Angle(Quaternion.Euler(0, 0, _FOVRotation) * Vector2.up, directionToTarget) > _FOVAngle / 2) return false;

            return true;
        }

        /// <summary>
        /// Get 
        /// </summary>
        /// <param name="_angleInDegrees"> Angle from <see cref="Vector2.up"/> in degrees, counter-clockwise. </param>
        /// <returns>
        /// 
        /// </returns>
        public static Vector2 DirectionFromAngle(float _angleInDegrees)
        {
            return new Vector2(
                Mathf.Sin(_angleInDegrees * Mathf.Deg2Rad),
                Mathf.Cos(_angleInDegrees * Mathf.Deg2Rad)
            );
        }

        #endregion
    }
}
