using System.Collections.Generic;

using UnityEngine;

namespace Custom.Controller
{
    public class SlopeHandler : MonoBehaviour
    {
        [Space(10)]
        [SerializeField] private float slopeCheckDistance = 0.1f;
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private GameObject parentBody;

        private CapsuleCollider2D cc;
        private ContactFilter2D contactFilter;

        private Vector2 slopeNormalPerp;
        private float slopeDownAngle;

        public bool IsOnSlope { get; private set; }

        public Vector2 SlopeDirection { get; private set; } = Vector2.right;



        private void Awake()
        {
            contactFilter.useTriggers = false;
            contactFilter.useLayerMask = true;
            contactFilter.layerMask = groundLayers;
        }

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
            List<RaycastHit2D> results = new();

            Debug.DrawRay(_start, _dir * _distance, Color.red, Time.fixedDeltaTime);

            if (Physics2D.Raycast(_start, _dir, contactFilter, results, _distance) > 0)
            {
                slopeDownAngle = Vector2.Angle(Vector2.up, results[0].normal);
                slopeNormalPerp = -Vector2.Perpendicular(results[0].normal).normalized;

                IsOnSlope = slopeDownAngle > 0f && slopeDownAngle < maxSlopeAngle;
                SlopeDirection = slopeNormalPerp;

                Debug.DrawRay(results[0].point - slopeNormalPerp, slopeNormalPerp * 2.0f, Color.magenta, Time.fixedDeltaTime);
            }
            else
            {
                IsOnSlope = false;
                SlopeDirection = Vector2.right;
            }
        }
    }
}
