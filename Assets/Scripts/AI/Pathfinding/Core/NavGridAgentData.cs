using UnityEngine;

namespace Custom.AI.Pathfinding
{
    [CreateAssetMenu(fileName = "New Agent Data", menuName = "Custom/Pathfinding/Nav Grid Agent")]
    public class NavGridAgentData : ScriptableObject
    {
        [Header("GENERAL")]
        [Tooltip("Gravitational acceleration of this agent in world unit.")]
        public Vector2 gravityAccel = new (0, -9.8f);

        [Tooltip("Height of agent in world unit.")]
        public float height = 1.8f;

        [Tooltip("Width of agent in world unit.")]
        public float width = 0.8f;


        [Header("SPEED")]
        [Tooltip("World units traveled per second.")]
        public float speed = 2.0f;


        [Header("JUMPING")]
        [Tooltip("Maximum jump distance in Y axis of agent in world unit.")]
        public float jumpHeight = 5.0f;

        [Tooltip("Maximum jump distance in X axis of agent in world unit.")]
        public float jumpDistance = 5.0f;

        [Tooltip("Maximum drop distance in Y axis of agent in world unit.")]
        public float dropHeight = 5.0f;



        public Vector2 Size => new(width, height);

        public Vector2 Extents => Size * 0.5f;
    }
}
