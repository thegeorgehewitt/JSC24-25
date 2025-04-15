using UnityEngine;

using Custom.Manager.Audio;

namespace Custom.Controller
{
    public class CharacterControlJump : CharacterControlBase
    {
        private const string JUMP_KEY = "Jump";

        public override string[] InputActionKeysName
        {
            get => new string[] {
                JUMP_KEY,
            };
        }



        [SerializeField] private float jumpPower = 8f;

        private bool jumpAttempt;



        private void OnEnable()
        {
            GetInputActionWithName(JUMP_KEY).performed += _ => { jumpAttempt = true; };
            GetInputActionWithName(JUMP_KEY).canceled += _ => CancelJump();
        }

        private void OnDisable()
        {
            GetInputActionWithName(JUMP_KEY).performed -= _ => { jumpAttempt = true; };
            GetInputActionWithName(JUMP_KEY).canceled -= _ => CancelJump();
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

            attachedMotor.Animator.SetTrigger(JUMP_KEY);

            attachedMotor.SetState("JumpEndedEarly", false);
            attachedMotor.velocity += Vector2.up * jumpPower;
        }

        private void CancelJump()
        {
            attachedMotor.SetState("JumpEndedEarly", true);
        }
        #endregion

        public void PlayJumpSFX()
        {
            AudioManager.PlaySFX(SFXGroup.PlayerLanding, transform.position, 1.0f);
        }
    }
}

