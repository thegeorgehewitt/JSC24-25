using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Interactable;

namespace Custom.UI
{
    public class InteractableObjectDisplayPopup : MonoBehaviour
    {
        public static InteractableObjectDisplayPopup Instance;

        [Header("REFERENCES")]
        [SerializeField] private GameObject objectStatePrefab;
        [SerializeField] private GameObject interactionPrefab;
        [SerializeField] private Transform objectStateListHolder;
        [SerializeField] private Transform interactionListHolder;
        [SerializeField] private TextMeshProUGUI objectName;

        [Header("POPUP")]
        [SerializeField] private Image maskImage;
        [SerializeField] private float easeDuration = 0.1f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private int displayedOptions;
        private int activeOption = 0;

        private List<InteractableObjectStateDisplay> objectStateDisplays = new();
        private List<InteractionInfoDisplay> interactionDisplays = new();

        public static int ActiveOption
        {
            get => Instance.activeOption;
            set => Instance.activeOption = Mathf.Clamp(value, 0, Instance.displayedOptions - 1); 
        }

        public static int DisplayedOptions
        {
            get => Instance.displayedOptions;
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



        #region Display Popup
        private InteractableObject currentObject;

        private void Core_DisplayInfo(InteractableObject _object)
        {
            if (currentObject != _object)
            {
                currentObject = _object;
                displayedOptions = _object.InteractionData.Length;
            }

            objectName.text = _object.ObjectData.objectName.ToUpper();

            UpdateInteractionDisplays(_object);
            UpdateObjectStateDisplays(_object);

            Core_ShowPopup(true);
        }

        private void UpdateInteractionDisplays(InteractableObject _object)
        {
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

                InteractionState state = (activeOption == i) ? InteractionState.Selected : InteractionState.Normal;
                interactionDisplays[i].DisplayInfo(_object.InteractionData[i], state);
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
