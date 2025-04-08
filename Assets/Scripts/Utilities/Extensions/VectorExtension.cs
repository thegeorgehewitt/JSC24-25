using UnityEngine;

namespace Custom.Extensions
{
    public static class VectorExtension
    {
        /// <summary>
        /// Shift all components of the <paramref name="_vector"/> to the right and wrapped around. <br/>
        /// e.g: (x, y, z) -> (z, x, y).
        /// </summary>
        /// <param name="_vector"> The vector to apply swizzle to. </param>
        public static void Swizzle(this ref Vector2 _vector)
        {
            _vector = new Vector2(_vector.y, _vector.x);
        }

        /// <inheritdoc cref="Swizzle(Vector2)"/>
        public static void Swizzle(this ref Vector3 _vector)
        {
            _vector = new Vector3(_vector.z, _vector.x, _vector.y);
        }
    }
}