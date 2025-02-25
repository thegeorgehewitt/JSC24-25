using System.Collections.Generic;

using UnityEngine;

using Custom.Attribute;

namespace Custom.Utility
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FieldOfViewDisplay : MonoBehaviour
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



        [SerializeField] public ViewCone viewCone;

        [Header("DISPLAY")]
        [ReadOnly]
        [SerializeField] public MeshFilter viewMeshFilter;
        [SerializeField] public bool useWorldSpace = true;
        [SerializeField] private float meshResolution = 3;
        [SerializeField] private float edgeDistanceThreshold = 0.1f;
        [SerializeField] private int edgeResolveIterations = 3;
        [SerializeField] public ContactFilter2D blockableFilter;

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
            DrawFieldOfView();
        }



        #region Field of View Mesh
        private void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewCone.Angle * meshResolution);

            // Fail-safe when mesh resolution is invalid.
            if (stepCount == 0)
            {
                viewMesh.Clear();
                return;
            }

            float stepAngleSize = viewCone.Angle / stepCount;

            List<Vector3> viewPoints = new();
            ViewCastInfo oldViewCast = new();

            // Generate vertices from ray casts.
            for (int i = 0; i <= stepCount; i++)
            {
                float a = -transform.eulerAngles.z - viewCone.Angle / 2 + stepAngleSize * i;
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

            vertices[0] = useWorldSpace ? transform.InverseTransformPoint(viewCone.Origin) : viewCone.Origin;
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

        private ViewCastInfo ViewCast(float _localAngle)
        {
            List<RaycastHit2D> hits = new();
            Vector3 origin = useWorldSpace ? viewCone.Origin : transform.TransformPoint(viewCone.Origin);
            Vector3 dir = FieldOfView.DirectionFromAngle(viewCone, transform, _localAngle);

            Physics2D.Raycast(origin, dir, blockableFilter, hits, viewCone.Radius);

            if (hits.Count > 0)
            {
                // We only take into account for the first hit since they are the closest.
                return new ViewCastInfo(true, hits[0].point, hits[0].distance, _localAngle);
            }
            else
            {
                return new ViewCastInfo(false, origin + dir * viewCone.Radius, viewCone.Radius, _localAngle);
            }
        }
        #endregion
    }
}
