using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Manager;
using Custom.Manager.Audio;

namespace Custom.Controller
{
    public class CharacterControlHorizontal : CharacterControlBase
    {
        private const string HORIZONTAL_KEY = "Horizontal";
        private const string SPRINT_KEY = "Sprint";


        public override string[] InputActionKeysName
        {
            get => new string[] {
                HORIZONTAL_KEY,
                SPRINT_KEY
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

        [Header("SPRINTING")]
        [Tooltip("If true, sprint key are toggled. Otherwise, held.")]
        [SerializeField] private bool toggleSprint = false;
        [Range(1, 3)]
        [SerializeField] private float sprintAccelerationMult = 1.5f;
        [Range(1, 3)]
        [SerializeField] private float sprintMaxSpeedMult = 1.5f;

        private bool sprinting;

        private bool footstepFlipFlop = false;
        private float footstepCooldown = 0.4f;
        private float footstepTimer = 0f;



        private void OnEnable()
        {
            GetInputActionWithName(SPRINT_KEY).started += OnSprintKeyPressed;
            GetInputActionWithName(SPRINT_KEY).canceled += OnSprintKeyReleased;
        }

        private void OnDisable()
        {
            GetInputActionWithName(SPRINT_KEY).started -= OnSprintKeyPressed;
            GetInputActionWithName(SPRINT_KEY).canceled -= OnSprintKeyReleased;
        }

        private void FixedUpdate()
        {
            ExecuteMovement();                        
        }



        #region Movement
        private void ExecuteMovement()
        {
            if (attachedMotor.GetState("Rolling")) return;

            var direction = GetInputActionWithName(HORIZONTAL_KEY).ReadValue<float>();
            float maxSpeed = attachedMotor.IsGrounded ? (sprinting ? sprintMaxSpeedMult : 1.0f) * groundMaxSpeed : airMaxSpeed;
            float targetSpeed = ((attachedMotor.velocity.x * direction > 0 && !attachedMotor.IsGrounded)    // If the character is moving in the same direction as input direction and not grounded
                                ? Mathf.Max(maxSpeed, Mathf.Abs(attachedMotor.velocity.x))                  // Target speed is the larger between max speed and current speed.
                                : maxSpeed) * direction;

            if (attachedMotor.IsNearWall) 
                attachedMotor.velocity.x = 0;

            // Decelerate character horizontal speed.
            if (direction == 0)
            {
                float deceleration = (attachedMotor.IsGrounded ? groundDeceleration : airDeceleration) * sprintAccelerationMult;
                attachedMotor.velocity.x = Mathf.MoveTowards(attachedMotor.velocity.x, 0, deceleration * TimeManager.FixedDeltaTime);
            }
            // Accelerate character horizontal speed.
            else
            {
                float acceleration = (attachedMotor.IsGrounded ? groundAcceleration : airAcceleration) * sprintAccelerationMult;
                attachedMotor.velocity.x = Mathf.MoveTowards(attachedMotor.velocity.x, targetSpeed, acceleration * TimeManager.FixedDeltaTime);
            }

            // Spawn footstep
            HandleFootstep();
        }

        private void HandleFootstep()
        {
            if (attachedMotor.IsGrounded && Mathf.Abs(attachedMotor.velocity.x) > 0.1f)
            {
                footstepTimer += TimeManager.FixedDeltaTime;

                if (footstepTimer >= footstepCooldown)
                {
                    footstepTimer = 0f;
                    string clipName = footstepFlipFlop ? "SFX_Footstep1" : "SFX_Footstep2";
                    SoundManager.PlaySFX(clipName, transform.position, 0.7f);

                    footstepFlipFlop = !footstepFlipFlop;
                }
            }
            else
            {
                footstepTimer = 0f;
            }
        }
        #endregion

        #region Sprinting
        private void OnSprintKeyPressed(InputAction.CallbackContext _context)
        {
            if (toggleSprint)
                sprinting = !sprinting;
            else
                sprinting = true;
        }

        private void OnSprintKeyReleased(InputAction.CallbackContext _context)
        {
            if (toggleSprint) return;

            sprinting = false;
        }
        #endregion
    }
}
