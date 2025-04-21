using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.UI.General;
using Custom.Manager;

namespace Custom.UI.HUD
{
    public class BatteryDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Transform batteryContainer;

        [Header("PREFABS")]
        [SerializeField] private GameObject batteryPrefab;

        [Header("ANIMATION")]
        [SerializeField] private float animationDuration = 1;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float flashingValue;

        private int maxBars;

        private readonly List<ProgressBar> progressBars = new();
        private (int, int) animatingIndexRange;



        private void OnEnable()
        {
            BatteryManager.OnCurrentBatteryUpdated += FillBatteriesTo;
            BatteryManager.OnMaxBatteryUpdated += OnMaxBatteryUpdated;
        }

        private void OnDisable()
        {
            BatteryManager.OnCurrentBatteryUpdated -= FillBatteriesTo;
            BatteryManager.OnMaxBatteryUpdated -= OnMaxBatteryUpdated;
        }

        private void Start()
        {
            for (int i = 0; i < BatteryManager.MaxAmount; i++)
            {
                AddProgressBar();
            }

            FillBatteriesTo(BatteryManager.CurrentAmount);
        }

        private void Update()
        {
            if (!BatteryManager.Full)
            {
                UpdateFillAmount();
            }
        }



        #region Maximum Container
        private void AddProgressBar()
        {
            maxBars++;

            if (maxBars < progressBars.Count)
            {
                progressBars[maxBars - 1].gameObject.SetActive(true);
            }
            else
            {
                progressBars.Add(Instantiate(batteryPrefab, batteryContainer).GetComponent<ProgressBar>());
            }
        }

        private void RemoveProgressBar()
        {
            progressBars[maxBars - 1].gameObject.SetActive(false);

            maxBars--;
        }
        #endregion

        #region Current Container
        private void FillBatteriesTo(int _amount)
        {
            for (int i = 0; i < _amount; i++)
            {
                progressBars[i].FillPercentage = 1;
                progressBars[i].FillAlpha = 1;
            }
            for (int i = _amount; i < maxBars; i++)
            {
                progressBars[i].FillPercentage = 0;
                progressBars[i].FillAlpha = 1;
            }

            if (_amount < maxBars)
                progressBars[_amount].FillAlpha = 0.2f;
        }



        private void UpdateFillAmount()
        {
            progressBars[BatteryManager.CurrentAmount].FillPercentage = BatteryManager.CurrentFilledAmount;
        }
        #endregion

        #region Animation
        private Coroutine flashingCoroutine;



        private void StartAnimateInRange(int _startIndex, int _endIndex)
        {
            animatingIndexRange = (_startIndex, _endIndex);

            if (flashingCoroutine != null)
                StopCoroutine(flashingCoroutine);

            flashingCoroutine = StartCoroutine(FlashingCoroutine());
        }

        private IEnumerator FlashingCoroutine()
        {
            float elapsedTime = 0;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                for (int i = animatingIndexRange.Item1; i <= animatingIndexRange.Item2; i++)
                {

                }

                yield return null;
            }
        }
        #endregion

        #region Callbacks
        private void OnMaxBatteryUpdated(int _amount)
        {
            if (maxBars > _amount)
                for (int i = maxBars; i > _amount; i--)
                {
                    RemoveProgressBar();
                }
            else
                for (int i = maxBars; i < _amount; i++)
                {
                    AddProgressBar();
                }
        }
        #endregion
    }
}
