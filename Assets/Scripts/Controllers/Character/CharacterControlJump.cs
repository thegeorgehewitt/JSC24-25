using Custom.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    public class CharacterControlJump : CharacterControlBase
    {
        [Header("JUMP")]
        [SerializeField] private float jumpPower = 8f;

        private bool jumpAttempt;



        private void OnEnable()
        {
            InputAction.performed += _ => { jumpAttempt = true; };
        }

        private void OnDisable()
        {
            InputAction.performed -= _ => { jumpAttempt = true; };
        }

        private void FixedUpdate()
        {
            ExecuteMovement();
        }



        #region Movement

        private void ExecuteMovement()
        {
            if (!jumpAttempt) return;
            jumpAttempt = false;

            if (!attachedMotor.IsGrounded) return;

            attachedMotor.velocity += Vector2.up * jumpPower;
        }

        #endregion
    }
}

