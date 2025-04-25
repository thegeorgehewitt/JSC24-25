using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Interactable;
using Custom.Manager.EventHandling;
using static Custom.Interactable.InteractableExitElevator;

namespace Custom.UI
{
    public class ExitElevatorPopUp : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image[] arrowImages;
        [SerializeField] private Image[] inputText;
        [SerializeField] private GameObject mainDisplay;
        [SerializeField] private MovementDirection playerExitTo;

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
            UpdateArrowPopup(false);

            SetPopupActive(false);
        }

        private void Start()
        {
            inputText[0].enabled = playerExitTo == MovementDirection.Up ? true : false;
            inputText[1].enabled = playerExitTo == MovementDirection.Down ? true : false;
        }

        #endregion


        #region Display Popup

        private bool popupVisible = false;



        public void ShowPopup(bool _show)
        {
            if (popupVisible == _show) return;
            popupVisible = _show;

            SetPopupActive(_show);

            inputText[0].enabled = playerExitTo == MovementDirection.Up ? true : false;
            inputText[1].enabled = playerExitTo == MovementDirection.Down ? true : false;
        }

        private void SetPopupActive(bool _active)
        {
            foreach (var obj in inputText)
            {
                obj.gameObject.SetActive(_active);
            }
        }

        private void UpdatePopup(UpdateElevatorUI _event)
        {
            UpdateArrowPopup(_event.Access);
        }

        private void UpdateArrowPopup(bool access)
        {
            if (access)
            {
                arrowImages[0].color = playerExitTo == MovementDirection.Up ? Color.white : Color.clear;
                arrowImages[1].color = playerExitTo == MovementDirection.Down ? Color.white : Color.clear;
            }
            else
            {
                arrowImages[0].color = playerExitTo == MovementDirection.Up ? Color.red : Color.clear;
                arrowImages[1].color = playerExitTo == MovementDirection.Down ? Color.red : Color.clear;
            }
        }
        #endregion
    }
}