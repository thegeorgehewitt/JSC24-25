using System.Collections;

using UnityEngine;
using Custom.Controller;
using Custom.Manager;

namespace Custom.UI
{
    public class VictoryPopup : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private GameObject mainDisplay;



        private void OnEnable()
        {
            ObjectiveTrackerPopup.OnObjectiveCompleted += () => SetPopupActive(true);
        }

        private void OnDisable()
        {
            ObjectiveTrackerPopup.OnObjectiveCompleted -= () => SetPopupActive(true);
        }

        private void Awake()
        {
            SetPopupActive(false);
        }



        private void SetPopupActive(bool _active)
        {
            mainDisplay.SetActive(_active);

            if (_active)
            {
                TimeManager.timeScale = 0;
                PlayerMotorController.PauseMotor(true);
            }
        }
    }
}
