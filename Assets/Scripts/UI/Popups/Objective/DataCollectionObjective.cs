using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Custom.Manager;
using Custom.Manager.Objective;
using Custom.Manager.EventHandling;
using static Custom.Interactable.InteractableObjectiveTerminal;

namespace Custom.UI
{
    public class DataCollectionObjective : ObjectiveBase
    {
        [Header("REFERENCES")]
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI backgroundText;
        [SerializeField] private TextMeshProUGUI fillText;
        [SerializeField] private Image fillMask;

        [Header("ANIMATION")]
        [SerializeField] private Vector3 expandedScale = Vector3.one * 1.5f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float expandDuration = 0.5f;
        [SerializeField] private float holdDuration = 1.0f;

        private Vector3 originalScale;

        private int currentValue;
        private int requiredValue;



        private void Awake()
        {
            originalScale = transform.localScale;
            title.text = label;

            EventAggregator.Subscribe<TerminalLoadedEvent>(OnTerminalLoaded);
            EventAggregator.Subscribe<DataCollectedEvent>(OnTerminalDataCollected);
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

        #region Callbacks
        private void OnTerminalLoaded()
        {
            requiredValue++;
        }

        private void OnTerminalDataCollected()
        {
            currentValue = Mathf.Clamp(currentValue + 1, 0, requiredValue);

            UpdateDisplays();

            if (currentValue >= requiredValue)
            {
                Complete();
            }
        }
        #endregion
    }
}
