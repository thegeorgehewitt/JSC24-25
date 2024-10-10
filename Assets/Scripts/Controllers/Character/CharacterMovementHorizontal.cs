using UnityEngine;

namespace Custom.Controller
{
    public class CharacterMovementHorizontal : CharacterMovementBase
    {
        [Header("SPEED")]
        [SerializeField] private float maxSpeed = 5.0f;
        [SerializeField] private float acceleration = 8.0f;
        [SerializeField] private float deceleration = 10.0f;




        private void Update()
        {
            var direction = InputAction.ReadValue<Vector2>();
            attachedMotor.Rigidbody2D.velocity = new Vector2(direction.x * maxSpeed, attachedMotor.Rigidbody2D.velocity.y);
        }
    }
}
