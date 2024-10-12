using UnityEngine;

namespace Custom.Controller
{
    public class CharacterControlWalk : CharacterControlBase
    {
        [Header("GROUND MOVEMENT")]
        [SerializeField] private float groundMaxSpeed = 5.0f;
        [SerializeField] private float groundAcceleration = 8.0f;
        [SerializeField] private float groundDeceleration = 10.0f;

        [Header("AIR MOVEMENT")]
        [SerializeField] private float airMaxSpeed = 3.0f;
        [SerializeField] private float airAcceleration = 4.0f;
        [SerializeField] private float airDeceleration = 5.0f;




        private void FixedUpdate()
        {
            ExecuteMovement();
        }



        #region Movement

        private void ExecuteMovement()
        {
            if (attachedMotor.GetState("Dashing")) return;

            var direction = InputAction.ReadValue<Vector2>();
            float maxSpeed = attachedMotor.IsGrounded ? groundMaxSpeed : airMaxSpeed;
            float targetSpeed = ((attachedMotor.velocity.x * direction.x > 0)               // If the character is moving in the same direction as input direction
                                ? Mathf.Max(maxSpeed, Mathf.Abs(attachedMotor.velocity.x))  // Target speed is the larger between max speed and current speed.
                                : maxSpeed) * direction.x;

            // Decelerate character horizontal speed.
            if (direction.x == 0)
            {
                float deceleration = attachedMotor.IsGrounded ? groundDeceleration : airDeceleration;
                attachedMotor.velocity.x = Mathf.MoveTowards(attachedMotor.velocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            // Accelerate character horizontal speed.
            else
            {
                float acceleration = attachedMotor.IsGrounded ? groundAcceleration : airAcceleration;
                attachedMotor.velocity.x = Mathf.MoveTowards(attachedMotor.velocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion
    }
}
