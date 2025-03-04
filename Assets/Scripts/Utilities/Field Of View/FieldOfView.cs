using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.Utility
{
    public static class FieldOfView
    {
        /// <summary>
        /// Find all components of type <typeparamref name="T"/> in view cone.
        /// <para> <b>NOTE:</b>  </para>
        /// </summary>
        /// <typeparam name="T">            Any class inherit from <see cref="Component"/>. </typeparam>
        /// <param name="_viewCone">        </param>
        /// <param name="_targetLayers">    Layers of objects to retrieve component from. </param>
        /// <param name="_blockFilter">     </param>
        /// <param name="_type">            See <see cref="DetectionType"/> for more information. </param>
        /// <returns>
        /// List of all components found.
        /// </returns>
        public static List<T> FindAllInViewCone<T>(ViewCone _viewCone, int _targetLayers, ContactFilter2D _blockFilter, DetectionType _type = DetectionType.Dynamic) 
            where T : Component
        {
            List<T> visibleTargets = new();
            Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(_viewCone.Origin, _viewCone.Radius, _targetLayers);

            foreach (var collider in targetsInViewRadius)
            {
                Vector2[] targetPoints = GetTargetPoints(collider);

                // Skip any triggers
                if (collider.isTrigger) continue;

                // If no target points is in view angle
                var inViewAnglesPoints = GetPointsInViewCone(_viewCone, targetPoints);
                if (inViewAnglesPoints.Length == 0) continue;

                // If raycast blocked
                var notBlockedPoints = GetPointsNotBlocked(_viewCone.Origin, inViewAnglesPoints, _blockFilter);
                if (notBlockedPoints.Length == 0) continue;

                // If does not have component
                if (!collider.gameObject.TryGetComponent(out T asTargetComponent)) continue;

                // Add component to list if valid
                visibleTargets.Add(asTargetComponent);
            }

            return visibleTargets;
        }

        /// <summary>
        /// Get all points from a list of points captured by given <paramref name="_viewCone"/>.
        /// </summary>
        /// <param name="_viewCone"> Defines a view cone data. </param>
        /// <param name="_points">   An array of points to check for. </param>
        /// <returns>
        /// An array of all points captured in <paramref name="_viewCone"/>.
        /// </returns>
        public static Vector3[] GetPointsInViewCone(ViewCone _viewCone, params Vector3[] _points)
        {
            List<Vector3> inViewPoints = new();

            foreach (var point in _points)
            {
                Vector2 directionToTarget = (point - (Vector3)_viewCone.Origin).normalized;

                if (Vector2.Angle(Quaternion.Euler(0, 0, _viewCone.Rotation) * Vector2.up, directionToTarget) <= _viewCone.Angle / 2)
                {
                    inViewPoints.Add(point);
                }
            }

            return inViewPoints.ToArray();
        }

        /// <inheritdoc cref="GetPointsInViewCone(ViewCone, Vector3[])"/>
        public static Vector3[] GetPointsInViewCone(ViewCone _viewCone, params Vector2[] _points)
        {
            return GetPointsInViewCone(_viewCone, _points.Select(e => (Vector3)e).ToArray());
        }

        /// <summary>
        /// Get all points from a list of points not blocked by any object filtered by <paramref name="_contactFilter"/>, 
        /// cast to a world point position.
        /// </summary>
        /// <param name="_center">          Center of the object to cast check rays from. </param>
        /// <param name="_points">          An array of points to check for. </param>
        /// <param name="_contactFilter">   Contact filter for blockable objects. </param>
        /// <returns>
        /// An array of all points not blocked from <paramref name="_center"/>.
        /// </returns>
        public static Vector3[] GetPointsNotBlocked(Vector3 _center, Vector3[] _points, ContactFilter2D _contactFilter)
        {
            List<Vector3> visiblePoints = new();
            List<RaycastHit2D> hits = new();

            foreach (var point in _points)
            {
                Vector2 directionToTarget = (point - _center).normalized;
                float distanceToTarget = Vector3.Distance(point, _center);

                if (Physics2D.Raycast(_center, directionToTarget, _contactFilter, hits, distanceToTarget) == 0)
                {
                    visiblePoints.Add(point);
                }
            }

            return visiblePoints.ToArray();
        }

        /// <summary>
        /// Get all target points of the target collider to check for.
        /// </summary>
        /// <param name="_collider2D">      The collider to retrieve points from. </param>
        /// <param name="_detectionType">   See <see cref="DetectionType"/> for more info. </param>
        /// <returns>
        /// An array containing all points to check for.
        /// </returns>
        public static Vector2[] GetTargetPoints(Collider2D _collider2D, DetectionType _detectionType = DetectionType.Dynamic)
        {
            switch (_detectionType)
            {
                case DetectionType.TransformPosition:
                    return new Vector2[1] { _collider2D.transform.position };


                case DetectionType.ColliderBounds:
                    return new Vector2[4] {
                        _collider2D.bounds.max,
                        _collider2D.bounds.min,
                        new(_collider2D.bounds.min.x, _collider2D.bounds.max.y),
                        new(_collider2D.bounds.min.y, _collider2D.bounds.max.x),
                    };


                case DetectionType.SpritePhysicsShape:
                    SpriteRenderer spriteRenderer = _collider2D.gameObject.GetComponentInChildren<SpriteRenderer>();

                    if (!spriteRenderer) return null;

                    List<Vector2> totalPoints = new();
                    for (int i = 0; i < spriteRenderer.sprite.GetPhysicsShapeCount(); i++)
                    {
                        List<Vector2> shapePoints = new();
                        spriteRenderer.sprite.GetPhysicsShape(0, shapePoints);

                        totalPoints.AddRange(shapePoints);
                    }

                    return totalPoints.Select(e => (Vector2)spriteRenderer.transform.TransformPoint(e)).ToArray();


                case DetectionType.Dynamic:
                    return 
                        GetTargetPoints(_collider2D, DetectionType.SpritePhysicsShape) ??
                        GetTargetPoints(_collider2D, DetectionType.ColliderBounds);

                default: return null;
            }
        }

        public static Vector2 DirectionFromAngle(ViewCone _viewCone, Transform _transform, float _localAngleInDegrees)
        {
            _localAngleInDegrees -= _transform.eulerAngles.z + _viewCone.Rotation;

            return new Vector2(
                Mathf.Sin(_localAngleInDegrees * Mathf.Deg2Rad),
                Mathf.Cos(_localAngleInDegrees * Mathf.Deg2Rad)
            );
        }
    }



    public enum DetectionType
    {
        /// <summary>
        /// Detect based on target's <see cref="Transform.position"/>.
        /// </summary>
        TransformPosition,

        /// <summary>
        /// Detect based on target's <see cref="Collider2D.bounds"/>. <br/>
        /// If no Collider2D is found, returns null.
        /// </summary>
        ColliderBounds,

        /// <summary>
        /// Detect based on target's <see cref="SpriteRenderer"/>.<see cref="Sprite.GetPhysicsShape(int, List{Vector2})"/>. <br/>
        /// If no SpriteRenderer is found, returns null.
        /// </summary>
        SpritePhysicsShape,

        /// <summary>
        /// Prioritize <see cref="DetectionType.SpritePhysicsShape"/> then <see cref="DetectionType.ColliderBounds"/>.
        /// </summary>
        Dynamic,
    }
}
