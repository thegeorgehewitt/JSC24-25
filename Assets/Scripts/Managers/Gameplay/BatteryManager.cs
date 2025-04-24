using System;

using UnityEngine;

namespace Custom.Manager
{
    public class BatteryManager : MonoBehaviour
    {
        public static BatteryManager Instance { get; private set; }

        public static event Action<int> OnMaxBatteryUpdated;
        public static event Action<int> OnCurrentBatteryUpdated;



        [SerializeField] private int maxAmount = 10;
        [SerializeField] private int currentAmount = 4;
        [SerializeField] private float refillDuration = 2.0f;

        private float elapsedTime;

        public static int CurrentAmount
        {
            get => Instance.currentAmount;
            set
            {
                value = Mathf.Clamp(value, 0, MaxAmount);

                if (Instance.currentAmount == value) return;

                Instance.currentAmount = value;
                OnCurrentBatteryUpdated?.Invoke(value);
            }
        }

        public static int MaxAmount
        {
            get => Instance.maxAmount;
            set
            {
                value = Mathf.Max(value, 0);

                if (Instance.maxAmount == value) return;

                Instance.maxAmount = value;
                OnMaxBatteryUpdated?.Invoke(value);
            }
        }

        public static float CurrentFilledAmount => Instance.elapsedTime / Instance.refillDuration;

        public static bool Full => CurrentAmount >= MaxAmount;



        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion
        }

        private void Update()
        {
            if (CurrentAmount < MaxAmount)
                UpdateCurrentFillAmount();
        }



        private void UpdateCurrentFillAmount()
        {
            elapsedTime += TimeManager.DeltaTime;

            if (elapsedTime > refillDuration)
            {
                elapsedTime = 0;

                CurrentAmount++;
            }
        }
    }
}
