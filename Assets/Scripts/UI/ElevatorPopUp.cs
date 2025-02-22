using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Custom.Controller;
using Custom.Manager;
using Custom.Manager.EventHandling;

using static Custom.Interactable.InteractableElevator;
using Custom.Interactable;
using System;
using TMPro;
using System.Drawing;

namespace Custom.UI
{
    public class ElevatorPopUp : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image[] arrowImages;
        [SerializeField] private GameObject mainDisplay;
        [SerializeField] private InteractableElevator connectedElevator;
        [SerializeField] private TextMeshProUGUI[] inputText;

        [Header("POPUP")]
        [SerializeField] private float easeDuration = 0.1f;

        #region SetUp

        private void OnEnable()
        {
            EventAggregator.Subscribe<UpdateElevatorUI>(UpdatePopup);
        }

        private void OnDisable()
        {
            EventAggregator.Unsubscribe<UpdateElevatorUI>(UpdatePopup);
        }

        private void Awake()
        {
            if (connectedElevator != null && Array.IndexOf(connectedElevator.States, "Access Denied") > -1)
            {
                arrowImages[0].color = connectedElevator.IsTop ? UnityEngine.Color.black : UnityEngine.Color.red;
                arrowImages[1].color = connectedElevator.IsBottom ? UnityEngine.Color.black : UnityEngine.Color.red;
            }
            else
            {
                arrowImages[0].color = connectedElevator.IsTop ? UnityEngine.Color.black : UnityEngine.Color.blue;
                arrowImages[1].color = connectedElevator.IsBottom ? UnityEngine.Color.black : UnityEngine.Color.blue;
            }

            inputText = new TextMeshProUGUI[arrowImages.Length];

            for (int i = 0; i < arrowImages.Length; i++)
            {
                inputText[i] = arrowImages[i].GetComponentInChildren<TextMeshProUGUI>();
            }

            SetPopupActive(false);
        }

        private void Start()
        {
            inputText[0].alpha = connectedElevator.IsTop ? 0 : 1;
            inputText[1].alpha = connectedElevator.IsBottom ? 0 : 1;
        }

        #endregion

        #region Display Popup

        private bool popupVisible = false;
        private Coroutine popupCoroutine;



        public void ShowPopup(bool _show)
        {
            if (popupVisible == _show) return;
            popupVisible = _show;

            if (popupCoroutine != null) StopCoroutine(popupCoroutine);

            popupCoroutine = StartCoroutine(PopupCoroutine(_show));
        }

        private IEnumerator PopupCoroutine(bool _show)
        {
            SetPopupActive(_show);

            if (connectedElevator != null)
            {
                inputText[0].alpha = connectedElevator.IsTop ? 0 : 1;
                inputText[1].alpha = connectedElevator.IsBottom ? 0 : 1;
            }

            yield return null;
        }

        private void SetPopupActive(bool _active)
        {
            foreach (TextMeshProUGUI text in inputText)
            {
                text.gameObject.SetActive(_active);
            }
        }

        private void UpdatePopup(UpdateElevatorUI _event)
        {
            if (connectedElevator != null && Array.IndexOf(connectedElevator.States, "Access Denied") > -1)
            {
                arrowImages[0].color = connectedElevator.IsTop ? UnityEngine.Color.clear : UnityEngine.Color.red;
                arrowImages[1].color = connectedElevator.IsBottom ? UnityEngine.Color.clear : UnityEngine.Color.red;
            }
            else
            {
                arrowImages[0].color = connectedElevator.IsTop ? UnityEngine.Color.clear : UnityEngine.Color.blue;
                arrowImages[1].color = connectedElevator.IsBottom ? UnityEngine.Color.clear : UnityEngine.Color.blue;
            }
        }

        #endregion
    }
}
