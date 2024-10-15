using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Custom.Controller;
using Custom.Manager;

namespace Custom.UI
{
    public class LevelEndPopup : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Image maskImage;

        [Header("POPUP")]
        [SerializeField] private float easeDuration = 0.1f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);



        private void OnEnable()
        {
            CharacterControlDamageable.OnMotorDamaged += OnMotorDamaged;
        }

        private void OnDisable()
        {
            CharacterControlDamageable.OnMotorDamaged -= OnMotorDamaged;
        }

        private void Awake()
        {
            maskImage.fillAmount = 0;
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

            // Pause Controls
            if (_show)
            {
                TimeManager.timeScale = 0;
                PlayerController.PauseMotor();
            }
        }

        private IEnumerator PopupCoroutine(bool _show)
        {
            float elapsedTime = 0;
            float targetAmount = _show ? 1 : 0;
            float orgAmount = maskImage.fillAmount;

            while (elapsedTime < easeDuration)
            {
                elapsedTime += Time.deltaTime;
                maskImage.fillAmount = Mathf.Lerp(orgAmount, targetAmount, easeCurve.Evaluate(elapsedTime / easeDuration));
                yield return null;
            }

            maskImage.fillAmount = targetAmount;
        }

        #endregion



        private void OnMotorDamaged()
        {
            ShowPopup(true);
        }
    }
}
