using UnityEngine;

using Custom.Manager;
using Custom.Manager.Audio;

namespace Custom.Controller
{
    public class CharacterControlWalk : CharacterControlBase
    {
        private bool footstepToggle = false;
        [SerializeField] private Animator animator;
        
     

        public override string[] InputActionKeysName
        {
            get => new string[] {
                "Horizontal"
            };
        }



        [Header("GROUND MOVEMENT")]
        [SerializeField] private float groundMaxSpeed = 6.0f;
        [SerializeField] private float groundAcceleration = 30.0f;
        [SerializeField] private float groundDeceleration = 20.0f;

        [Header("AIR MOVEMENT")]
        [SerializeField] private float airMaxSpeed = 3.0f;
        [SerializeField] private float airAcceleration = 8.0f;
        [SerializeField] private float airDeceleration = 8.0f;


        private float footstepCooldown = 0.4f;
        private float footstepTimer = 0f;


        private void FixedUpdate()
        {
            ExecuteMovement();                        
        }



        #region Movement
        private void ExecuteMovement()
        {
            if (attachedMotor.GetState("Rolling")) return;
            var direction = GetInputActionWithName("Horizontal").ReadValue<float>();
            float maxSpeed = attachedMotor.IsGrounded ? groundMaxSpeed : airMaxSpeed;
            float targetSpeed = ((attachedMotor.velocity.x * direction > 0)               // If the character is moving in the same direction as input direction
                                ? Mathf.Max(maxSpeed, Mathf.Abs(attachedMotor.velocity.x))  // Target speed is the larger between max speed and current speed.
                                : maxSpeed) * direction;

            if (attachedMotor.IsOnWall) 
                attachedMotor.velocity.x = 0;

            // Decelerate character horizontal speed.
            if (direction == 0)
            {
                float deceleration = attachedMotor.IsGrounded ? groundDeceleration : airDeceleration;
                attachedMotor.velocity.x = Mathf.MoveTowards(attachedMotor.velocity.x, 0, deceleration * TimeManager.FixedDeltaTime);
            }
            // Accelerate character horizontal speed.
            else
            {
                float acceleration = attachedMotor.IsGrounded ? groundAcceleration : airAcceleration;
                attachedMotor.velocity.x = Mathf.MoveTowards(attachedMotor.velocity.x, targetSpeed, acceleration * TimeManager.FixedDeltaTime);
            }

            animator?.SetFloat("Horizontal Speed", Mathf.Abs(attachedMotor.velocity.x));

            if (attachedMotor.IsGrounded && Mathf.Abs(attachedMotor.velocity.x) > 0.1f)
            {
                footstepTimer += TimeManager.FixedDeltaTime;

                if (footstepTimer >= footstepCooldown)
                {
                    footstepTimer = 0f;
                    string clipName = footstepToggle ? "SFX_Footstep1" : "SFX_Footstep2";
                    SoundManager.Instance.PlaySFX(clipName, transform.position, 0.7f);
                    footstepToggle = !footstepToggle;

                }
            }
            else
            {
                footstepTimer = 0f;
            }

        }
        #endregion
    }
}
