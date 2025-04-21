using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Interactable;
using Custom.Controller;
using Custom.Scriptable.Interactable;
using Custom.Settings;
using Custom.Scriptable.Settings;

namespace Custom.UI.HUD
{
    public class InteractableObjectDisplayPopup : MonoBehaviour
    {
        public static InteractableObjectDisplayPopup Instance;

        [Header("REFERENCES")]
        [SerializeField] private Transform objectStateListHolder;
        [SerializeField] private Transform interactionListHolder;
        [SerializeField] private Transform objectPropertiesHolder;
        [Space]
        [SerializeField] private Image objectTypeDisplayBar;
        [SerializeField] private TextMeshProUGUI objectNameText;
        [SerializeField] private TextMeshProUGUI objectTypeText;
        [SerializeField] private Image objectIcon;
        [SerializeField] private TextMeshProUGUI objectDescription;
        [Space]
        [SerializeField] private InteractionDetailedInfoDisplay interactionDetailedInfoDisplay;

        [Header("PREFAB REFERENCES")]
        [SerializeField] private GameObject objectStatePrefab;
        [SerializeField] private GameObject interactionPrefab;
        [SerializeField] private GameObject objectPropertyPrefab;

        [Header("POPUP ANIMATION")]
        [SerializeField] private Image maskImage;
        [SerializeField] private float easeDuration = 0.1f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private int displayedOptions;
        private int activeOption = 0;

        private readonly List<InteractableObjectStateDisplay> objectStateDisplays = new();
        private readonly List<InteractionInfoDisplay> interactionDisplays = new();

        public static int ActiveOption
        {
            get => Instance.activeOption;
            set => Instance.activeOption = Mathf.Clamp(value, 0, Instance.displayedOptions - 1); 
        }

        public static int DisplayedOptions
        {
            get => Instance.displayedOptions;
        }



        private void OnEnable()
        {
            CharacterControlInteract.OnSelectNewInteraction += OnSelectNewInteraction;
            CharacterControlInteract.OnFocusNewInteractableObject += OnFocusNewInteractableObject;
            CharacterControlInteract.OnUnfocusInteractableObject += OnUnfocusInteractableObject;
        }

        private void OnDisable()
        {
            CharacterControlInteract.OnSelectNewInteraction -= OnSelectNewInteraction;
            CharacterControlInteract.OnFocusNewInteractableObject -= OnFocusNewInteractableObject;
            CharacterControlInteract.OnUnfocusInteractableObject -= OnUnfocusInteractableObject;
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

            maskImage.fillAmount = 0;
        }

        private void Update()
        {
            Core_DisplayInfo(currentObject);
        }



        #region Callbacks - CharacterControlInteract
        private void OnSelectNewInteraction(int _activeOption)
        {
            ActiveOption = _activeOption;
        }

        private void OnFocusNewInteractableObject(InteractableObject _object)
        {
            Core_DisplayInfo(_object);
        }

        private void OnUnfocusInteractableObject(InteractableObject _object)
        {
            ShowPopup(false);

            currentObject = null;
        }
        #endregion

        #region Display Popup
        private InteractableObject currentObject;

        private void Core_DisplayInfo(InteractableObject _object)
        {
            if (!_object) return;

            if (currentObject != _object)
            {
                activeOption = 0;
                currentObject = _object;
                displayedOptions = _object.InteractionData.Length;
            }

            UpdateObjectDetailsDisplay(_object.ObjectData);
            UpdateInteractionDisplays(_object);
            UpdateObjectStateDisplays(_object);

            Core_ShowPopup(true);
        }

        private void UpdateObjectDetailsDisplay(InteractableObjectData _data)
        {
            objectNameText.text = _data.objectName.ToUpper();

            objectTypeText.text = _data.type.ToString();
            objectTypeText.color = GetTypeColorFromType(_data.type);

            objectTypeDisplayBar.color = GetTypeColorFromType(_data.type);

            objectIcon.sprite = _data.icon;

            objectDescription.text = _data.ParsedDescription;
        }

        private void UpdateInteractionDisplays(InteractableObject _object)
        {
            interactionDetailedInfoDisplay.gameObject.SetActive(_object.InteractionData.Length > 0);
            interactionListHolder.gameObject.SetActive(_object.InteractionData.Length > 0);

            for (int i = 0; i < _object.InteractionData.Length; i++)
            {
                if (i >= interactionDisplays.Count)
                {
                    var go = Instantiate(interactionPrefab, interactionListHolder);
                    interactionDisplays.Add(go.GetComponent<InteractionInfoDisplay>());
                }
                else
                {
                    interactionDisplays[i].gameObject.SetActive(true);
                }

                if (activeOption == i)
                {
                    interactionDetailedInfoDisplay.UpdateDisplay(_object.InteractionData[i]);
                    interactionDisplays[i].DisplayInfo(_object.InteractionData[i], InteractionState.Selected);
                }
                else
                {
                    interactionDisplays[i].DisplayInfo(_object.InteractionData[i], InteractionState.Normal);
                }
            }

            for (int i = _object.InteractionData.Length; i < interactionDisplays.Count; i++)
            {
                interactionDisplays[i].gameObject.SetActive(false);
            }
        }

        private void UpdateObjectStateDisplays(InteractableObject _object)
        {
            for (int i = 0; i < _object.States.Length; i++)
            {
                if (i >= objectStateDisplays.Count)
                {
                    var go = Instantiate(objectStatePrefab, objectStateListHolder);
                    objectStateDisplays.Add(go.GetComponent<InteractableObjectStateDisplay>());
                }
                else
                {
                    objectStateDisplays[i].SetActive(true);
                }

                objectStateDisplays[i].DisplayInfo(_object.States[i]);
            }

            for (int i = _object.States.Length; i < objectStateDisplays.Count; i++)
            {
                objectStateDisplays[i].SetActive(false);
            }
        }

        private Color GetTypeColorFromType(InteractableObjectType _type)
        {
            switch (_type)
            {
                case InteractableObjectType.Friendly:
                    return VisualSettings.ColorPalette.GetUIColor(UIElementGroup.FriendlyPrimary);

                case InteractableObjectType.Neutral:
                    return VisualSettings.ColorPalette.GetUIColor(UIElementGroup.NeutralPrimary);

                case InteractableObjectType.Hostile:
                    return VisualSettings.ColorPalette.GetUIColor(UIElementGroup.HostilePrimary);

                case InteractableObjectType.Chaotic:
                    return VisualSettings.ColorPalette.GetUIColor(UIElementGroup.ChaoticPrimary);

                default: return Color.white;
            }
        }



        public static void DisplayInfo(InteractableObject _object)
        {
            Instance?.Core_DisplayInfo(_object);
        }
        #endregion

        #region Animation
        private bool popupVisible;
        private Coroutine popupCoroutine;

        private void Core_ShowPopup(bool _visible)
        {
            if (popupVisible == _visible) return;
            popupVisible = _visible;

            if (popupCoroutine != null) StopCoroutine(popupCoroutine);

            popupCoroutine = StartCoroutine(PopupCoroutine(_visible));
        }

        private IEnumerator PopupCoroutine(bool _visible)
        {
            float elapsedTime = 0;
            float startAmount = maskImage.fillAmount;
            float targetAmount = _visible ? 1 : 0;

            while (elapsedTime < easeDuration)
            {
                elapsedTime += Time.deltaTime;
                maskImage.fillAmount = Mathf.Lerp(startAmount, targetAmount, easeCurve.Evaluate(elapsedTime / easeDuration));
                yield return null;
            }

            maskImage.fillAmount = targetAmount;
        }

        public static void ShowPopup(bool _state)
        {
            Instance?.Core_ShowPopup(_state);
        }
        #endregion
    }
}
