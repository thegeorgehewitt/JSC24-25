using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom.Controller
{
    public class CharacterControlCrouch : CharacterControlBase
    {
        private const string CROUCH_KEY = "Crouch";

        public override string[] InputActionKeysName
        {
            get => new string[] {
                CROUCH_KEY
            };
        }



        [Tooltip("If true, crouch will automatically occurred to avoid overlapping colliders.")]
        [SerializeField] private bool autoCrouch = true;
        [Tooltip("If true, crouch key are toggled. Otherwise, held.")]
        [SerializeField] private bool toggleCrouch = false;
        [Range(0, 1)]
        [SerializeField] private float heightMult = 0.5f;
        [Range(0, 1)]
        [SerializeField] private float speedMult = 0.6f;

        private bool crouching;



        private void OnEnable()
        {
            GetInputActionWithName(CROUCH_KEY).started += OnStartHoldingKey;
            GetInputActionWithName(CROUCH_KEY).canceled += OnEndHoldingKey;
        }

        private void OnDisable()
        {
            GetInputActionWithName(CROUCH_KEY).started -= OnStartHoldingKey;
            GetInputActionWithName(CROUCH_KEY).canceled -= OnEndHoldingKey;
        }

        private void FixedUpdate()
        {
            bool isCrouching = (crouching && attachedMotor.IsGrounded)
                            || attachedMotor.GetState("Rolling") 
                            || (attachedMotor.IsOnCeiling && attachedMotor.HeightMult < 1.0f && autoCrouch);

            attachedMotor.Animator.SetBool("Crouch", isCrouching);
            attachedMotor.SetHeightMult(isCrouching ? heightMult : 1.0f);

            if (isCrouching && attachedMotor.IsGrounded && !attachedMotor.GetState("Rolling"))
            {
                attachedMotor.velocity.x *= speedMult;
            }
        }



        private void OnStartHoldingKey(InputAction.CallbackContext _callback)
        {
            if (toggleCrouch)
                crouching = !crouching;
            else
                crouching = true;
        }

        private void OnEndHoldingKey(InputAction.CallbackContext _callback)
        {
            if (toggleCrouch) return;

            crouching = false;
        }
    }
}
