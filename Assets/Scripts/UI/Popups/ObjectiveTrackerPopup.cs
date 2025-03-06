using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Custom.Manager;

namespace Custom.UI
{
    public class ObjectiveTrackerPopup : MonoBehaviour
    {
        public static event Action OnObjectiveCompleted;

        public static ObjectiveTrackerPopup Instance {  get; private set; }

        private Vector3 originalScale;



        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI backgroundText;
        [SerializeField] private TextMeshProUGUI fillText;
        [SerializeField] private Image fillMask;

        [Header("GENERAL INFO")]
        [SerializeField] private string titleName = "Objective";
        [SerializeField] private int requiredValue = 0;

        [Header("ANIMATION")]
        [SerializeField] private Vector3 expandedScale = Vector3.one * 1.5f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float expandDuration = 0.5f;
        [SerializeField] private float holdDuration = 1.0f;

        private int currentValue;

        public int CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = Mathf.Clamp(value, 0, requiredValue);

                UpdateDisplays();

                if (currentValue >= requiredValue)
                {
                    OnObjectiveCompleted?.Invoke();
                }
            }
        }

        public int RequiredValue
        {
            get => requiredValue;
            set
            {
                requiredValue = Mathf.Max(value, 1);

                UpdateDisplays();

                if (currentValue >= requiredValue)
                {
                    OnObjectiveCompleted?.Invoke();
                }
            }
        }



        private void Awake()
        {
            #region Singleton
            if (!Instance)
                Instance = this;
            else
                Destroy(this);
            #endregion

            originalScale = transform.localScale;
        }

        private void Start()
        {
            title.text = titleName;
        }



        #region Display
        private Coroutine expandCoroutine;
        private float elapsedTime;

        private void UpdateDisplays()
        {
            backgroundText.text = $"{currentValue} / {requiredValue}";
            fillText.text = $"{currentValue} / {requiredValue}";

            fillMask.fillAmount = (float)currentValue / requiredValue;

            // Start animation.
            if (expandCoroutine != null)
                StopCoroutine(expandCoroutine);

            expandCoroutine = StartCoroutine(ExpandCoroutine());
        }

        private IEnumerator ExpandCoroutine()
        {
            while (elapsedTime < expandDuration)
            {
                elapsedTime += TimeManager.DeltaTime;
                transform.localScale = Vector3.Lerp(originalScale, expandedScale, easeCurve.Evaluate(elapsedTime / expandDuration));

                yield return null;
            }

            yield return new WaitForSeconds(holdDuration);

            while (elapsedTime > 0)
            {
                elapsedTime -= TimeManager.DeltaTime;
                transform.localScale = Vector3.Lerp(originalScale, expandedScale, easeCurve.Evaluate(elapsedTime / expandDuration));

                yield return null;
            }

            elapsedTime = 0;
            transform.localScale = originalScale;
        }
        #endregion
    }
}
