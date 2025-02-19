using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Custom.Controller;
using Custom.Manager;
using Custom.Manager.EventHandling;

using static Custom.Interactable.InteractableElevator;
using Custom.Interactable;
using System;

namespace Custom.UI
{
    public class ElevatorPopUp : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image[] arrowImages;
        [SerializeField] private GameObject mainDisplay;
        [SerializeField] private InteractableElevator connectedElevator;

        [Header("POPUP")]
        [SerializeField] private float easeDuration = 0.1f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private void OnEnable()
        {
            EventAggregator.Subscribe<ElevatorStartOverlapEvent>(OnStartOverlap);
            EventAggregator.Subscribe<ElevatorEndOverlapEvent>(OnEndOverlap);
            EventAggregator.Subscribe<UpdateElevatorUI>(UpdatePopup);
        }

        private void OnDisable()
        {
            EventAggregator.Unsubscribe<ElevatorStartOverlapEvent>(OnStartOverlap);
            EventAggregator.Unsubscribe<ElevatorEndOverlapEvent>(OnEndOverlap);
            EventAggregator.Unsubscribe<UpdateElevatorUI>(UpdatePopup);
        }

        private void Awake()
        {
            SetPopupActive(false);
        }

        private void Start()
        {
            arrowImages[0].color = connectedElevator.IsTop ? Color.black : Color.blue;
            arrowImages[1].color = connectedElevator.IsBottom ? Color.black : Color.blue;
        }

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
            float elapsedTime = 0;
            float targetAmount = _show ? 1 : 0;
            float orgAmount = arrowImages[0].fillAmount;

            if (_show)
            {
                mainDisplay.SetActive(true);
            }

            if (connectedElevator != null && Array.IndexOf(connectedElevator.States, "Access Denied") > -1)
            {
                arrowImages[0].color = connectedElevator.IsTop ? Color.black : Color.red;
                arrowImages[1].color = connectedElevator.IsBottom ? Color.black : Color.red;
            }
            else
            {
                arrowImages[0].color = connectedElevator.IsTop ? Color.black : Color.blue;
                arrowImages[1].color = connectedElevator.IsBottom ? Color.black : Color.blue;
            }

            while (elapsedTime < easeDuration)
            {
                elapsedTime += Time.deltaTime;
                foreach (Image maskImage in arrowImages)
                {
                    maskImage.fillAmount = Mathf.Lerp(orgAmount, targetAmount, easeCurve.Evaluate(elapsedTime / easeDuration));
                }
                yield return null;
            }

            SetPopupActive(_show);
        }

        private void SetPopupActive(bool _active)
        {
            foreach (Image maskImage in arrowImages)
            {
                maskImage.fillAmount = _active ? 1 : 0;
            }

            mainDisplay.SetActive(_active);
        }

        private void UpdatePopup(UpdateElevatorUI _event)
        {
            if (mainDisplay.activeSelf)
            {
                if (connectedElevator != null && Array.IndexOf(connectedElevator.States, "Access Denied") > -1)
                {
                    arrowImages[0].color = connectedElevator.IsTop ? Color.black : Color.red;
                    arrowImages[1].color = connectedElevator.IsBottom ? Color.black : Color.red;
                }
                else
                {
                    arrowImages[0].color = connectedElevator.IsTop ? Color.black : Color.blue;
                    arrowImages[1].color = connectedElevator.IsBottom ? Color.black : Color.blue;
                }
            }
        }

        #endregion

        private void OnStartOverlap(ElevatorStartOverlapEvent _event)
        {
            ShowPopup(true);
        }

        private void OnEndOverlap(ElevatorEndOverlapEvent _event)
        {
            ShowPopup(false);
        }
    }
}
