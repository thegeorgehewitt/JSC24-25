using UnityEngine;

using Custom.Manager;
using Unity.VisualScripting.Dependencies.Sqlite;

namespace Custom.Controller
{
    public class CharacterControlWalk : CharacterControlBase
    {
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



        private void FixedUpdate()
        {
            ExecuteMovement();                        
        }



        #region Movement
        private void ExecuteMovement()
        {
            if (attachedMotor.GetState("Rolling")) return;
            var direction = GetInputActionWithName("Horizontal").ReadValue<Vector2>();
            float maxSpeed = attachedMotor.IsGrounded ? groundMaxSpeed : airMaxSpeed;
            float targetSpeed = ((attachedMotor.velocity.x * direction.x > 0)               // If the character is moving in the same direction as input direction
                                ? Mathf.Max(maxSpeed, Mathf.Abs(attachedMotor.velocity.x))  // Target speed is the larger between max speed and current speed.
                                : maxSpeed) * direction.x;


            if (attachedMotor.IsOnWall) attachedMotor.velocity.x = 0;

            var stairHandler = GetComponent<StairHandler>();

            if (stairHandler == null)    Debug.Log("StairHandle is not set");
            
            if (stairHandler != null && stairHandler.IsOnSlope && attachedMotor.IsGrounded)
            {
                //Debug.DrawRay(attachedMotor.transform.position, stairHandler.GetComponent<Rigidbody2D>().transform.position, Color.green, 0.1f); ;

                

                if (direction.x == 0)
                {
                    attachedMotor.velocity.x = 0;
                    Debug.Log("Standing freeze on slope");
                    //attachedMotor.velocity.x = Mathf.MoveTowards(attachedMotor.velocity.x, 0, TimeManager.FixedDeltaTime);

                }
                else
                {
                    float acceleration = attachedMotor.IsGrounded ? groundAcceleration : airAcceleration;
                    Vector2 moveOnSlopeDir = stairHandler.SlopeDirection * -direction.x;
                    attachedMotor.velocity = Vector2.MoveTowards(attachedMotor.velocity, moveOnSlopeDir * 6, acceleration *TimeManager.FixedDeltaTime);                    
                }
                
            }
            else
            {
               
                // Decelerate character horizontal speed.
                if (direction.x == 0)
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
            }            
        }
        #endregion
    }
}
