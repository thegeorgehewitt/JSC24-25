using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    public class CharacterMovementJump : CharacterMovementBase
    {
        [Header("GROUND CHECK")]
        [SerializeField] private Collider2D groundCheck;
        [SerializeField] private LayerMask groundLayers;

        [Header("PROPERTIES")]
        [SerializeField] private float jumpHeight = 5;

        private ContactFilter2D contactFilter;
        private Collider2D[] contacts;

        public bool IsGrounded { get { return groundCheck.OverlapCollider(contactFilter, contacts) > 0; } }



        private void OnEnable()
        {
            InputAction.performed += context => OnPerformed(context);
        }

        private void OnDisable()
        {
            InputAction.performed -= context => OnPerformed(context);
        }

        private void Awake()
        {
            #region Setup Contact Filter
            contactFilter.layerMask = groundLayers;
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = true;
            contactFilter.useDepth = false;
            #endregion
        }



        #region Input Handeling

        private void OnPerformed(InputAction.CallbackContext _context)
        {
            if (_context.performed) Jump();
        }

        #endregion

        #region Movement

        private void Jump()
        {
            if (!attachedMotor) return;

            attachedMotor.Rigidbody2D.velocity += Vector2.up * jumpHeight;
        }

        #endregion
    }
}

