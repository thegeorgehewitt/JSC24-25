using UnityEngine;

namespace Custom.Controller
{
    public class CharacterControlJump : CharacterControlBase
    {
        [SerializeField] private Animator animator;

        [SerializeField] private string[] jumpSFXNames = new string[] { "SFX_Jump_Land_03" };

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
            GetInputActionWithName("Jump").canceled += _ => CancelJump();
        }

        private void OnDisable()
        {
            GetInputActionWithName("Jump").performed -= _ => { jumpAttempt = true; };
            GetInputActionWithName("Jump").canceled -= _ => CancelJump();
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

            if (animator) { animator.SetTrigger("Jump"); }

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
                Debug.Log("PlayJumpSFX: " + clip);
                SoundManager.Instance.PlaySFX(clip, transform.position, 1.0f);
            }
        }
        #endregion
    }
}

