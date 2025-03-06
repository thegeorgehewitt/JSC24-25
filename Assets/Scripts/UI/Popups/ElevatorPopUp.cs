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
        [SerializeField] private TextMeshProUGUI[] inputText;



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



        public void ShowPopup(bool _show)
        {
            if (popupVisible == _show) return;
            popupVisible = _show;

            SetPopupActive(_show);

            if (connectedElevator != null)
            {
                inputText[0].alpha = connectedElevator.IsTop ? 0 : 1;
                inputText[1].alpha = connectedElevator.IsBottom ? 0 : 1;
            }
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
                arrowImages[0].color = connectedElevator.IsTop ? UnityEngine.Color.clear : UnityEngine.Color.blue;
                arrowImages[1].color = connectedElevator.IsBottom ? UnityEngine.Color.clear : UnityEngine.Color.blue;
            }
        }
        #endregion
    }
}
