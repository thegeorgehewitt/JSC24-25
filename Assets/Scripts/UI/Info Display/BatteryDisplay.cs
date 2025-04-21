using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.UI.General;

namespace Custom.UI.HUD
{
    public class BatteryDisplay : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Transform batteryContainer;

        [Header("PREFABS")]
        [SerializeField] private GameObject batteryPrefab;

        [Header("ANIMATION")]
        [SerializeField] private float animationDuration;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float flashingValue;

        private int activeBars;

        private readonly List<ProgressBar> progressBars = new();
        private (int, int) animatingIndexRange;



        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            
        }



        #region Maximum Container
        private void AddProgressBar()
        {
            activeBars++;

            if (activeBars < progressBars.Count)
            {
                progressBars[activeBars - 1].gameObject.SetActive(true);
            }
            else
            {
                progressBars.Add(Instantiate(batteryPrefab, batteryContainer).GetComponent<ProgressBar>());
            }
        }

        private void RemoveProgressBar()
        {
            progressBars[activeBars - 1].gameObject.SetActive(false);

            activeBars--;
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
    }
}
