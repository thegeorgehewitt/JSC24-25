using UnityEngine;
using UnityEngine.UI;

using Custom.Controller;
using Custom.Manager;
using System.Collections;
using Unity.VisualScripting;

namespace Custom.Interactable.Character.Enemy
{
    public abstract class InteractableEnemyBase : InteractableCharacterBase
    {
        [Header("DETECTION")]
        [Tooltip(
            "The minimum visibility value of character motor before being detected in enemy's FOV.\n" +
            "NOTE: Proximity check does NOT take visibility values into account.")]
        [Range(0, 1)]
        [SerializeField] protected float minVisibilityDetectLevel = 0.2f;
        [Tooltip("The base multiplier for detection meter alteration when a player is detected.")]
        [SerializeField] protected float baseDetectRate = 1.0f;
        [Tooltip("The base multiplier for detection meter alteration when a player is not detected.")]
        [SerializeField] protected float baseIgnoreRate = 0.5f;

        [Header("LOCK ON")]
        [SerializeField] protected float lockOnDuration = 1.0f;
        [Range(0, 1)]
        [SerializeField] protected float normalizedLockOnDistance = 0.5f;

        [Header("HACKING AND COOLDOWN")]
        private float hackedCooldownTime = 3.0f;
        private Coroutine cooloffCoroutine;
        private bool IsHacked => cooloffCoroutine != null;

        // FOR TESTING ONLY
        [Header("DETECTION METER DISPLAY")]
        [SerializeField] private Canvas detectionMeterCanvas;
        [SerializeField] private Image fillMaskImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private Color alertColor = Color.red;

        protected AcquireTargetResult<CharacterMotor2D> lastScanResult;

        protected float currentDetectionLevel;
        protected bool playerAlerted;

        protected float elapsedLockOn;
        protected bool playerLockedOn;

        public float LockOnDistance => normalizedLockOnDistance * radius;

        public Comparer.CompareCharacterMotor2D DefaultComparer => new(transform.position);



        protected override void Update()
        {
            base.Update();

            UpdateCurrentTarget();
            UpdateDetectionMeter();
        }



        protected virtual void OnPlayerDetected(CharacterMotor2D _newTarget) { }

        protected virtual void OnPlayerAlerted() { }

        protected virtual void OnPlayerLockedOn() { }

        protected virtual void OnPlayerIgnored() { }

        protected virtual void OnPlayerLost() { }

        protected virtual void OnEnemyHacked() { }
        
        protected virtual void OnHackedEnded() { }



        private void UpdateCurrentTarget()
        {
            if (IsHacked) return;

            lastScanResult = AcquireTarget(DefaultComparer);

            if (lastScanResult.target && (lastScanResult.target.Visibility > minVisibilityDetectLevel || lastScanResult.proximityChecked))
            {
                OnPlayerDetected(lastScanResult.target);
            }
            else
            {
                OnPlayerLost();
            }
        }

        private void UpdateDetectionMeter()
        {
            if (IsHacked) return;


            if (lastScanResult.target)
            {
                float normDis = Vector3.Distance(lastScanResult.target.transform.position, transform.position) / radius;
                float visibility = lastScanResult.proximityChecked ? 1 : lastScanResult.target.Visibility;

                currentDetectionLevel += TimeManager.DeltaTime * baseDetectRate * visibility * (1.0f - Mathf.Clamp01(normDis));
            }
            else
            {
                currentDetectionLevel -= TimeManager.DeltaTime * baseIgnoreRate;
            }

            currentDetectionLevel = Mathf.Clamp01(currentDetectionLevel);

            fillMaskImage.fillAmount = currentDetectionLevel;
            fillImage.color = Color.Lerp(Color.white, alertColor, currentDetectionLevel);

            // Update Alert Level
            if (currentDetectionLevel == 1)
            {
                // Alert
                if (!playerAlerted)
                {
                    playerAlerted = true;
                    OnPlayerAlerted();
                }

                // Lock on
                elapsedLockOn += TimeManager.DeltaTime;

                if (!playerLockedOn && elapsedLockOn >= lockOnDuration)
                {
                    playerLockedOn = true;
                    OnPlayerLockedOn();
                }
            }
            else if (playerAlerted && currentDetectionLevel < 1)
            {
                elapsedLockOn = 0;

                playerAlerted = false;
                playerLockedOn = false;

                OnPlayerIgnored();
            }

            detectionMeterCanvas.enabled = currentDetectionLevel > 0;
        }

        public void Hacked()
        {
            OnEnemyHacked();
            
            if (cooloffCoroutine != null)
            {
                StopCoroutine(cooloffCoroutine);
            }
            
            cooloffCoroutine = StartCoroutine(HackedCooloff());
        }

        private void HackedEnded()
        {
            OnHackedEnded();
        }

        IEnumerator HackedCooloff()
        {
            yield return new WaitForSeconds(hackedCooldownTime);

            HackedEnded();

            cooloffCoroutine = null;
        }
    }
}
