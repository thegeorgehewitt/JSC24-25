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

        private List<InteractableObjectStateDisplay> objectStates = new();
        private List<InteractionInfoDisplay> interactions = new();



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

            /// TESTING ///
            var go = Instantiate(interactionPrefab, interactionListHolder);
            interactions.Add(go.GetComponent<InteractionInfoDisplay>());
            /// TESTING ///

            maskImage.fillAmount = 0;
        }



        #region Display Popup

        private bool popupVisible;
        private Coroutine popupCoroutine;



        private void Core_DisplayInfo(InteractableObject _object)
        {
            interactions[0].DisplayInfo(_object.InteractionData);

            for (int i = 0; i < _object.States.Length; i++)
            {
                if (i >= objectStates.Count)
                {
                    var go = Instantiate(objectStatePrefab, objectStateListHolder);
                    objectStates.Add(go.GetComponent<InteractableObjectStateDisplay>());
                }
                else
                {
                    objectStates[i].SetActive(true);
                }

                objectStates[i].DisplayInfo(_object.States[i]);
            }

            for (int i = _object.States.Length; i < objectStates.Count; i++)
            {
                objectStates[i].SetActive(false);
            }

            objectName.text = _object.ObjectData.objectName.ToUpper();

            Core_ShowPopup(true);
        }

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
            Instance.Core_ShowPopup(_state);
        }

        public static void DisplayInfo(InteractableObject _object)
        {
            Instance.Core_DisplayInfo(_object);
        }

        #endregion
    }
}
