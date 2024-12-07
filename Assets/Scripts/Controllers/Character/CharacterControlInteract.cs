using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Interactable;
using Custom.Decorative;
using Custom.UI;

namespace Custom.Controller
{
    public class CharacterControlInteract : CharacterControlBase
    {
        public override string[] InputActionKeysName
        {
            get => new string[] {
                "Interact",
                "Scroll"
            };
        }



        public static event Action OnInteractObjectOutOfRange;
        public static event Action OnVisionBlocked;
        public static event Action<InteractableObject> OnHoverNewInteractableObject;

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



        private void OnEnable()
        {
            GetInputActionWithName("Interact").performed += _ => Interact();
            GetInputActionWithName("Scroll").performed += OnScroll;
        }

        private void OnDisable()
        {
            GetInputActionWithName("Interact").performed -= _ => Interact();
            GetInputActionWithName("Scroll").performed -= OnScroll;
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
            UpdateInteractionPopup();
            UpdateDefaultValues();
        }



        #region Actions
        private int activeOption;
        private float scrollValue;

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
                hoverObject.Interact(activeOption);
            }
        }

        private void OnScroll(InputAction.CallbackContext _context)
        {
            if (!hoverObject) return;

            var value = _context.ReadValue<float>();

            // If scroll in the opposite direction from current direction, reset scroll value.
            if (value * scrollValue < 0)
            {
                scrollValue = 0;
            }

            scrollValue += value;

            // When rounded to correct value, update current active option.
            if (Mathf.Abs(scrollValue) >= 1)
            {
                int roundedScrollValue = Mathf.RoundToInt(scrollValue);
                scrollValue = 0;
                activeOption += roundedScrollValue;
                activeOption = Mathf.Clamp(activeOption, 0, hoverObject.InteractionData.Length - 1);

                // Call to interactable object display.
                InteractableObjectDisplayPopup.ActiveOption = activeOption;
            }
        }

        private void UpdateDefaultValues()
        {
            if (!hoverObject)
            {
                // We reset the scroll value and activeOption to discard changes from last hovered object.
                scrollValue = 0;
                activeOption = 0;

                InteractableObjectDisplayPopup.ActiveOption = activeOption;
            }
        }
        #endregion

        #region Hover Object
        private InteractableObject hoverObject;
        private bool outOfRange;
        private bool blockedVision;
        private ContactFilter2D contactFilter;
        private List<RaycastHit2D> interactRayHits = new();

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

            if (blockedVision)
            {
                UpdateInteractCursor(targetPos, hitPos.point);
            }
            else
            {
                UpdateInteractCursor(targetPos, targetPos);
            }
        }

        private void UpdateInteractionPopup()
        {
            if (hoverObject)
            {
                InteractableObjectDisplayPopup.DisplayInfo(hoverObject);
            }
            else
            {
                InteractableObjectDisplayPopup.ShowPopup(false);
            }
        }

        private void UpdateInteractCursor(Vector3 _cursorPos, Vector3 _lineEndPos)
        {
            interactCursor.SetLinePosition(interactRayOrigin.position, _lineEndPos);

            interactCursor.SetPosition(_cursorPos);
            interactCursor.SetColor(outOfRange ? outOfRangeColor : inRangeColor);
            interactCursor.SetLineFadeAmount(outOfRange ? 1f : 0f);

            if (hoverObject)
            {
                interactCursor.SetLineActive(true);
                interactCursor.SetSize(hoverObject.ObjectBoundsSize);
            }
            else
            {
                interactCursor.SetLineActive(outOfRange);
                interactCursor.SetSize(Vector2.one * defaultCursorSize);
            }
        }
        #endregion

        #region CharacterControlBase
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
        #endregion
    }
}
