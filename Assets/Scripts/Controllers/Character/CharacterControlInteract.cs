using System;

using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Interactable;
using Custom.Decorative;

namespace Custom.Controller
{
    public class CharacterControlInteract : CharacterControlBase
    {
        [Header("INTERACT")]
        [SerializeField] private Transform interactRayOrigin;
        [SerializeField] private float interactRange = 5f;
        [SerializeField] private float interactSnapRadius = 0.4f;
        [SerializeField] private LayerMask interactableLayers;

        [Header("INTERACT CURSOR")]
        [SerializeField] private InteractCursor interactCursor;
        [SerializeField] private float defaultCursorSize = 0.5f;
        [SerializeField] private Color outOfRangeColor = Color.red;
        [SerializeField] private Color inRangeColor = Color.blue;

        private InteractableObject hoverObject;
        private bool outOfRange;

        public static Action OnInteractObjectOutOfRange;



        private void OnEnable()
        {
            InputAction.performed += _ => Interact();
        }

        private void OnDisable()
        {
            InputAction.performed -= _ => Interact();
        }

        private void FixedUpdate()
        {
            var mousePosWorld = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var overlapCols = Physics2D.OverlapCircleAll(mousePosWorld, interactSnapRadius, interactableLayers);

            if (overlapCols.Length <= 0)
            {
                interactCursor.SetPosition(mousePosWorld);
                interactCursor.SetSize(Vector2.one * defaultCursorSize);
                hoverObject = null;
            }
            else
            {
                foreach (var collider in overlapCols)
                {
                    if (!collider.transform.TryGetComponent(out InteractableObject asInteractable)) continue;
                    
                    hoverObject = asInteractable;
                    interactCursor.SetPosition(asInteractable.transform.position);
                    interactCursor.SetSize(asInteractable.SpriteRenderer.bounds.size);
                    break;
                }
            }

            outOfRange = Vector2.Distance(interactRayOrigin.position, interactCursor.transform.position) > interactRange;

            interactCursor.SetColor(outOfRange ? outOfRangeColor : inRangeColor);
        }



        private void Interact()
        {
            if (!hoverObject) return;

            if (outOfRange)
            {
                OnInteractObjectOutOfRange?.Invoke();
            }
            else
            {
                hoverObject.Interact();
            }
        }



        protected override void OnActivate()
        {
            interactCursor.gameObject.SetActive(true);
            enabled = true;

            Cursor.visible = false;
        }

        protected override void OnDeactivate()
        {
            interactCursor.gameObject.SetActive(false);
            enabled = false;

            Cursor.visible = true;
        }
    }
}
