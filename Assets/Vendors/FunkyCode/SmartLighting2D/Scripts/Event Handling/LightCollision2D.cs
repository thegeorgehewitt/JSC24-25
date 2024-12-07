using System.Collections.Generic;

using UnityEngine;

namespace FunkyCode
{
	public struct LightCollision2D
	{
		public struct PointRelative
		{
			/// <summary>
			/// The index of polygon vertex this point belongs to.
			/// </summary>
			public int vertexIndex;
            /// <summary>
            /// The position of this point relative to the light source.
            /// </summary>
            public Vector2 lightRelative;
            /// <summary>
            /// The position of this point relative to the collider polygon origin.
            /// </summary>
            public Vector2 polygonRelative;

			public PointRelative(
				int _vertexIndex, 
				Vector2 _lightRelative, 
				Vector2 _polygonRelative)
			{
                this.vertexIndex = _vertexIndex;
				this.lightRelative = _lightRelative;
				this.polygonRelative = _polygonRelative;
            }
        }

		public enum State
		{
			OnCollision,
			OnCollisionEnter,
			OnCollisionExit
		}

		public Light2D light;
		public LightCollider2D collider;

		public List<PointRelative> points;
		public State state;

		public LightCollision2D(State state)
		{
            this.light = null;
			this.collider = null;
			this.points = null;
			this.state = state;
		}
    }
}