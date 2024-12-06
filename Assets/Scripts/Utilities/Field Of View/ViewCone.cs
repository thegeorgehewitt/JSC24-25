using UnityEngine;

namespace Custom.Utility
{
    /// <summary>
    /// Defines a view cone to pass into FOV functions.
    /// </summary>
    [System.Serializable]
    public struct ViewCone
    {
        [SerializeField] private float radius;
        [Range(0, 360)]
        [SerializeField] private float angle;
        [Range(0, 360)]
        [SerializeField] private float rotation;
        [SerializeField] private Vector2 origin;

        /// <summary>
        /// World point at which the view cone is projected from.
        /// </summary>
        public Vector3 Origin
        {
            get => origin;
            set => origin = value;
        }

        /// <summary>
        /// World rotation counter-clockwise in degrees.
        /// <para> 
        /// <b>NOTE:</b> Values set to this property will always be converted to be in range [0..360]. <br/>
        /// If a negative value is used, It will be recalculated to positive value (e.g. -90 becomes 270). 
        /// </para>
        /// </summary>
        public float Rotation
        {
            get => rotation;
            set => rotation = ((360 + value) % 360);
        }

        /// <summary>
        /// Maximum range of the view cone in world unit.
        /// <para> <b>NOTE:</b> Values set to this property will always be clamped to positive value [0...Infinity). </para>
        /// </summary>
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(value, 0);
        }

        /// <summary>
        /// Covered angle of the view cone in degrees.
        /// <para> <b>NOTE:</b> Values set to this property will always be clamped to [0..360]. </para>
        /// </summary>
        public float Angle
        {
            get => angle;
            set => angle = Mathf.Clamp(value, 0, 360);
        }
    }
}