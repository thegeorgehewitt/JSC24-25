using System;

using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Interactable;
using Custom.Decorative;
using Custom.UI;
using System.Collections.Generic;

namespace Custom.Controller
{
    public class CharacterControlInteract : CharacterControlBase
    {
        public static Action OnInteractObjectOutOfRange;
        public static Action OnVisionBlocked;
        public static Action<InteractableObject> OnHoverNewInteractableObject;

        [Header("INTERACT")]
        [SerializeField] private Transform interactRayOrigin;
        [SerializeField] private float interactRange = 5f;
        [SerializeField] private float interactSnapRadius = 0.4f;
        [SerializeField] private LayerMask interactableLayers;
        [SerializeField] private LayerMask blockableLayers;

        [Header("INTERACT CURSOR")]
        [SerializeField] private InteractCursor interactCursor;
        [SerializeField] private float defaultCursorSize = 0.5f;
        [SerializeField] private Color outOfRangeColor = Color.red;
        [SerializeField] private Color inRangeColor = Color.cyan;

        private InteractableObject hoverObject;
        private bool outOfRange;
        private bool blockedVision;
        private ContactFilter2D contactFilter;
        private List<RaycastHit2D> interactRayHits = new();



        private void OnEnable()
        {
            InputAction.performed += _ => Interact();
        }

        private void OnDisable()
        {
            InputAction.performed -= _ => Interact();
        }

        private void Awake()
        {
            #region Setup Contact Filter
            contactFilter.layerMask = blockableLayers;
            contactFilter.useLayerMask = true;
            contactFilter.useTriggers = false;
            #endregion
        }

        private void FixedUpdate()
        {
            UpdateHoverInteractableObject();
        }



        private void Interact()
        {
            if (!hoverObject) return;

            if (outOfRange)
            {
                OnInteractObjectOutOfRange?.Invoke();
            }
            else if (blockedVision)
            {
                OnVisionBlocked?.Invoke();
            }
            else
            {
                hoverObject.Interact();
            }
        }

        private void UpdateHoverInteractableObject()
        {
            var mousePosWorld = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var overlapCols = Physics2D.OverlapCircleAll(mousePosWorld, interactSnapRadius, interactableLayers);
            var rayDirection = interactCursor.transform.position - interactRayOrigin.position;

            if (overlapCols.Length <= 0)
            {
                hoverObject = null;
            }
            else
            {
                foreach (var collider in overlapCols)
                {
                    if (!collider.transform.TryGetComponent(out InteractableObject asInteractable)) continue;
                    
                    hoverObject = asInteractable;
                    OnHoverNewInteractableObject?.Invoke(hoverObject);

                    break;
                }
            }

            var targetPos = hoverObject ? hoverObject.InteractPosition : mousePosWorld;
            var distance = Vector2.Distance(interactRayOrigin.position, interactCursor.transform.position);

            RaycastHit2D hitPos = new();
            if (Physics2D.Raycast(interactRayOrigin.position, rayDirection, contactFilter, interactRayHits, distance) > 0)
            {
                hitPos = interactRayHits[0];
            }

            blockedVision = hoverObject ? (hitPos.transform != hoverObject.transform && hitPos) : hitPos;
            outOfRange = distance > interactRange;

            #region Interact Cursor & Interactable Object Display Popup
            if (hoverObject)
            {
                interactCursor.SetLineActive(true);
                interactCursor.SetSize(hoverObject.SpriteRenderer.bounds.size);
                InteractableObjectDisplayPopup.DisplayInfo(hoverObject);
            }
            else
            {
                interactCursor.SetLineActive(outOfRange);
                interactCursor.SetSize(Vector2.one * defaultCursorSize);
                InteractableObjectDisplayPopup.ShowPopup(false);
            }

            if (blockedVision)
            {
                interactCursor.SetLinePosition(interactRayOrigin.position, hitPos.point);
            }
            else
            {
                interactCursor.SetLinePosition(interactRayOrigin.position, targetPos);
            }

            interactCursor.SetPosition(targetPos);
            interactCursor.SetColor(outOfRange ? outOfRangeColor : inRangeColor);
            interactCursor.SetLineFadeAmount(outOfRange ? 1f : 0f);
            #endregion
        }



        protected override void OnActivate()
        {
            interactCursor.gameObject.SetActive(true);
            enabled = true;
        }

        protected override void OnDeactivate()
        {
            interactCursor.gameObject.SetActive(false);
            enabled = false;
        }
    }
}
