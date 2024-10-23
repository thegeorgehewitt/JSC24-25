using System.Collections.Generic;

using UnityEngine;

using Custom.Attribute;

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
        public float radius = 5.0f;
        [Range(0, 360)]
        public float angle = 45.0f;
        [Range(0, 360)]
        [Tooltip("Counter clock-wise rotation offset from transform.up.")]
        public float rotation = 0.0f;

        [Header("DISPLAY")]
        [HideInInspector] public bool drawViewMesh = true;
        [HideInInspector] public float meshResolution = 3;
        [HideInInspector] public float edgeDistanceThreshold = 0.1f;
        [HideInInspector] public int edgeResolveIterations = 3;

        [HideInInspector] public bool preview = true;
        [HideInInspector] public Color handlesColor = Color.cyan;

        [HideInInspector] public ContactFilter2D blockableFilter;

        private Mesh viewMesh;



#if UNITY_EDITOR
        private void Reset()
        {
            viewMeshFilter = GetComponent<MeshFilter>();
        }
#endif

        private void Awake()
        {
            viewMesh = new();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;

            blockableFilter.useLayerMask = true;
            blockableFilter.useTriggers = false;
        }

        private void LateUpdate()
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
        /// Find all components of type T in field of view.
        /// </summary>
        /// <typeparam name="T">            Component type to return. </typeparam>
        /// <param name="_targetLayers">    <see cref="LayerMask"/> of objects to retrieve component from. </param>
        /// <returns>
        /// List of all targets found.
        /// </returns>
        public List<T> FindAllInView<T>(int _targetLayers)
        {
            List<T> visibleTargets = new();
            List<RaycastHit2D> hits = new();
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, radius, _targetLayers);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.up, directionToTarget) - rotation < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (Physics2D.Raycast(transform.position, directionToTarget, blockableFilter, hits, distanceToTarget) > 0) continue;
                    if (!target.TryGetComponent(out T asTargetComponent)) continue;

                    visibleTargets.Add(asTargetComponent);
                }
            }

            return visibleTargets;
        }

        /// <summary>
        /// Check if the given direction is contained within the vision cone.
        /// </summary>
        /// <param name="_direction">   The direction to check for. </param>
        /// <returns>
        /// True if the direction is within the defined cone of this field of view. Otherwise false.
        /// </returns>
        public bool InFieldOfView(Vector3 _direction)
        {
            Vector3 viewAngleFrom = DirectionFromAngle(-angle / 2, false);
            Vector3 viewAngleTo = DirectionFromAngle(angle / 2, false);

            return (Vector3.Cross(viewAngleFrom, _direction).z * Vector3.Cross(viewAngleFrom, viewAngleTo).z >= 0)
                && (Vector3.Cross(viewAngleTo, _direction).z * Vector3.Cross(viewAngleTo, viewAngleFrom).z >= 0);
        }

        public Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees -= transform.eulerAngles.z;
            }

            angleInDegrees -= rotation;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
        }

        #endregion



        #region Field of View Mesh

        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(angle * meshResolution);
            float stepAngleSize = angle / stepCount;

            List<Vector3> viewPoints = new();
            ViewCastInfo oldViewCast = new();

            // Generate view mesh from ray casts.
            for (int i = 0; i <= stepCount; i++)
            {
                float a = -transform.eulerAngles.z - angle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(a);

                if (i > 0)
                {
                    // Handle edge cases.
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
    }
}
