using UnityEngine;

using Custom.Manager.Audio;

namespace Custom.Controller
{
    public class CharacterControlJump : CharacterControlBase
    {
        [SerializeField] private string[] jumpSFXNames = new string[] { "SFX_Jump_Land_03" };

        private const string JUMP_KEY = "Jump";

        public override string[] InputActionKeysName
        {
            get => new string[] {
                JUMP_KEY,
            };
        }



        [Header("JUMP")]
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

        public void PlayJumpSFX()
        {
            if (jumpSFXNames.Length > 0)
            {
                string clip = jumpSFXNames[Random.Range(0, jumpSFXNames.Length)];
                SoundManager.PlaySFX(clip, transform.position, 1.0f);
            }
        }
        #endregion
    }
}

