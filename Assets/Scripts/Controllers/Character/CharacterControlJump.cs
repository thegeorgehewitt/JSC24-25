using UnityEngine;

namespace Custom.Controller
{
    public class CharacterControlJump : CharacterControlBase
    {
        public override string[] InputActionKeysName
        {
            get => new string[] {
                "Jump",
            };
        }



        [Header("JUMP")]
        [SerializeField] private float jumpPower = 8f;

        private bool jumpAttempt;



        private void OnEnable()
        {
            GetInputActionWithName("Jump").performed += _ => { jumpAttempt = true; };
        }

        private void OnDisable()
        {
            GetInputActionWithName("Jump").performed -= _ => { jumpAttempt = true; };
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

