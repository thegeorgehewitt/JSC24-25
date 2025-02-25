using UnityEngine;
using UnityEngine.UI;

namespace Custom.AI.Pathfinding
{
    public class NavGridObstacle2D : MonoBehaviour
    {
        [SerializeField] private BoundsType type;

        [SerializeField] private Collider2D colliderBounds;
        [SerializeField] private Image imageBounds;

        private PhysicsShape2D bounds;

        public PhysicsShape2D Bounds => bounds;



        private void Start()
        {
            bounds.shapeType = PhysicsShapeType2D.Polygon;
        }



        public void UpdateBounds()
        {
            switch (type)
            {
                case BoundsType.Collider2D:

                    break;

                case BoundsType.SpritePhysicalShape:

                    break;

                case BoundsType.SpriteBounds:

                    break;

                default: break;
            }
        }
    }



    public enum BoundsType
    {
        Collider2D,

        SpritePhysicalShape,

        SpriteBounds,
    }
}
