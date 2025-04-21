using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Interactable;
using Custom.Manager.EventHandling;
using static Custom.Interactable.InteractableElevator;

namespace Custom.UI
{
    public class ElevatorPopUp : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image[] arrowImages;
        [SerializeField] private GameObject mainDisplay;
        [SerializeField] private InteractableElevator connectedElevator;
        [SerializeField] private Image[] inputText;



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
            UpdateArrowPopup();

            SetPopupActive(false);
        }

        private void Start()
        {
            inputText[0].enabled = connectedElevator.IsTop ? false : true;
            inputText[1].enabled = connectedElevator.IsBottom ? false : true;
        }

        #endregion

        #region Display Popup

        private bool popupVisible = false;



        public void ShowPopup(bool _show)
        {
            if (popupVisible == _show) return;
            popupVisible = _show;

            SetPopupActive(_show);

            if (connectedElevator != null)
            {
                inputText[0].enabled = connectedElevator.IsTop ? false : true;
                inputText[1].enabled = connectedElevator.IsBottom ? false : true;
            }
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
            UpdateArrowPopup();
        }

        private void UpdateArrowPopup()
        {
            if (connectedElevator && !connectedElevator.Accessible)
            {
                arrowImages[0].color = connectedElevator.IsTop ? UnityEngine.Color.clear : UnityEngine.Color.red;
                arrowImages[1].color = connectedElevator.IsBottom ? UnityEngine.Color.clear : UnityEngine.Color.red;
            }
            else
            {
                arrowImages[0].color = connectedElevator.IsTop ? UnityEngine.Color.clear : UnityEngine.Color.white;
                arrowImages[1].color = connectedElevator.IsBottom ? UnityEngine.Color.clear : UnityEngine.Color.white;
            }
        }
        #endregion
    }
}
