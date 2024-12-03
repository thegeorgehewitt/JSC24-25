using System.Collections.Generic;

using UnityEngine;

using FunkyCode.Utilities;
using Unity.VisualScripting;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;
using System.Collections;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightEventListener : MonoBehaviour
    {
        [Header("EVENT MODE")]
        [Tooltip("Enable this to avoid event overlap in scenes with multiple light sources. Disable for better performance.")]
        [SerializeField] private bool enableMultiLightCapture = true;
        [Tooltip("Whether visibility should be affected by distance from light sources.")]
        [SerializeField] private bool useDistance = false;
        [Tooltip("Weight of visible collision points evaluated by normalized distance from the closest light source.")]
        [SerializeField] private AnimationCurve distanceWeight = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Space(10)]
        [SerializeField] public float visibility = 0;

        private LightCollider2D lightCollider;
        private Polygon2 polygon;

        private Dictionary<Light2D, LightCollision2D> multiCollisionInfo = new();
        private List<Vector2> visiblePoints = new();
        private List<float> collisionPointsValue = new();

        private LightCollision2D? singleCollisionInfo = null;

        public Vector2[] VisiblePoints { get { return visiblePoints.ToArray(); } }



        private void OnEnable()
        {
            lightCollider = GetComponent<LightCollider2D>();

            lightCollider?.AddEvent(CollisionEvent);
        }

        private void OnDisable()
        {
            lightCollider?.RemoveEvent(CollisionEvent);
        }

        private void Update()
        {
            ResetValues();

            if (enableMultiLightCapture)
            {
                if (multiCollisionInfo.Count == 0) return;

                foreach (LightCollision2D info in multiCollisionInfo.Values)
                {
                    RecalculateCollisionPointsValue(info);
                }
            }
            else
            {
                if (singleCollisionInfo == null) return;

                RecalculateCollisionPointsValue(singleCollisionInfo.Value);
            }

            RecalculateVisibility();

            ClearCollisionInfoValues();
        }



        private void ResetValues()
        {
            polygon = lightCollider.mainShape.GetPolygonsLocal()[0];

            visiblePoints.Clear();
            collisionPointsValue = new(Enumerable.Repeat(0.0f, polygon.points.Length));

            visibility = 0;
        }

        private void ClearCollisionInfoValues()
        {
            multiCollisionInfo.Clear();
            singleCollisionInfo = null;
        }

        private void CollisionEvent(LightCollision2D collision)
        {
            // If light collider have no collision, cancel.
            if (collision.points == null) return;

            // Multi light capture
            if (enableMultiLightCapture)
            {
                // If light was still kept track of, update collision info.
                if (multiCollisionInfo.ContainsKey(collision.light))
                {
                    multiCollisionInfo[collision.light] = collision;
                }
                // If new light was detected, add new light to light collision list.
                else
                {
                    multiCollisionInfo.Add(collision.light, collision);
                }
            }
            // Single light capture
            else
            {
                if (singleCollisionInfo == null)
                {
                    singleCollisionInfo = collision;
                }
                else if (singleCollisionInfo.Value.points != null)
                {
                    if (collision.points.Count >= singleCollisionInfo.Value.points.Count)
                    {
                        singleCollisionInfo = collision;
                    }
                    else if (singleCollisionInfo.Value.light == collision.light)
                    {
                        singleCollisionInfo = collision;
                    }
                }
            }
        }

        private void RecalculateCollisionPointsValue(LightCollision2D _info)
        {
            // If no collision, cancel.
            if (_info.points.Count == 0) return;

            int pointsCount = polygon.points.Length;
            int pointsInView = _info.points.Count;

            // If no visible collision points, cancel.
            if (pointsInView == 0) return;

            // Loop through all points and calculate their value, then update them to collisionPointsValue.
            foreach (var pointInfo in _info.points)
            {
                float value;
                if (useDistance)
                {
                    float distance = Vector2.Distance(Vector2.zero, pointInfo.lightRelative);
                    value = Mathf.Clamp01(distanceWeight.Evaluate(distance / _info.light.size));
                }
                else
                {
                    value = 1;
                }

                collisionPointsValue[pointInfo.vertexIndex] = Mathf.Clamp01(collisionPointsValue[pointInfo.vertexIndex] + value);

                // Add to visible point list if not yet added.
                if (!visiblePoints.Contains(pointInfo.polygonRelative))
                {
                    visiblePoints.Add(pointInfo.polygonRelative);
                }
            }
        }

        private void RecalculateVisibility()
        {
            float totalValue = 0;
            foreach (var value in collisionPointsValue)
            {
                totalValue += value;
            }
            visibility = totalValue / polygon.points.Length;
        }
    }
}