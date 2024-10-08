using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Interactable;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        [Header("REFERENCE")]
        [SerializeField] private new Rigidbody2D rigidbody;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerInput inputMapping;

        [Header("MOVEMENT")]
        [SerializeField] private float jumpHeight;
        [SerializeField] private float movementSpeed;



        private void Reset()
        {
            if (!rigidbody) rigidbody = GetComponent<Rigidbody2D>();
            rigidbody.excludeLayers |= gameObject.layer;

            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion
        }

        private void Update()
        {
            HandleInput();
            HandleInteraction();
        }

        private void LateUpdate()
        {
            ExecuteMovement();
        }



        #region Input

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                scanningInteractable = true;
                scanningInteractableAttempt = true;
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                scanningInteractable = false;
                scanningInteractableAttempt = true;
            }

            direction = Input.GetAxisRaw("Horizontal");
        }

        #endregion

        #region Movement

        private float direction;

        private void Jump()
        {
            rigidbody.velocity += Vector2.up * jumpHeight;
        }

        private void ExecuteMovement()
        {
            rigidbody.velocity = new Vector2(direction * movementSpeed, rigidbody.velocity.y);
        }

        #endregion

        #region Interaction

        private bool scanningInteractableAttempt;
        private bool scanningInteractable;

        private float originalCameraZoom;



        private void HandleInteraction()
        {
            if (!scanningInteractableAttempt) return;

            if (scanningInteractable)
            {
                originalCameraZoom = CameraController.Size;

                CameraController.StartZoom(CameraController.Size + 3.0f, 0.5f);
                InteractablesManager.ToggleSpriteEffects(true);
            }
            else
            {
                CameraController.StartZoom(originalCameraZoom, 0.5f);
                InteractablesManager.ToggleSpriteEffects(false);
            }

            scanningInteractableAttempt = false;
        }

        #endregion
    }
}
