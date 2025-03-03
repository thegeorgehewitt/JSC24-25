using UnityEngine;

namespace Custom.Controller
{
    public class SlopeHandler : MonoBehaviour
    {
        [SerializeField] private float slopeCheckDistance = 0.1f;
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private GameObject parentBody;

        [SerializeField] PhysicsMaterial2D noneFriction;
        [SerializeField] PhysicsMaterial2D maxFriction;

        private CapsuleCollider2D cc;

        private Vector2 slopeNormalPerp;
        private float slopeDownAngle;

        public bool IsOnSlope { get; private set; }

        public Vector2 SlopeDirection { get; private set; } = Vector2.right;



        private void Start()
        {
            cc = parentBody.GetComponent<CapsuleCollider2D>();
        }

        private void FixedUpdate()
        {
            SlopeCheck();
        }



        private void SlopeCheck()
        {
            Vector2 checkPos = (Vector2)transform.position - new Vector2(0.0f, cc.bounds.size.y / 2.0f);

            SlopeCheckDirectional(checkPos, Vector2.down, slopeCheckDistance);
        }

        private void SlopeCheckDirectional(Vector2 _start, Vector2 _dir, float _distance)
        {
            RaycastHit2D hit = Physics2D.Raycast(_start, _dir, _distance, groundLayer);

            Debug.DrawRay(_start, _dir * _distance, Color.red, Time.fixedDeltaTime);

            if (hit)
            {
                slopeDownAngle = Vector2.Angle(Vector2.up, hit.normal);
                slopeNormalPerp = -Vector2.Perpendicular(hit.normal).normalized;

                IsOnSlope = slopeDownAngle > 0f && slopeDownAngle < maxSlopeAngle;
                SlopeDirection = slopeNormalPerp;

                Debug.DrawRay(hit.point - slopeNormalPerp, slopeNormalPerp * 2.0f, Color.magenta, Time.fixedDeltaTime);
            }
            else
            {
                IsOnSlope = false;
                SlopeDirection = Vector2.right;
            }
        }
    }
}
