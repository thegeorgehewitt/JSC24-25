using UnityEngine;

using Custom.Controller;

namespace Custom.Interactable.Character.Enemy
{
    public abstract class InteractableEnemyBase : InteractableCharacterBase
    {
        [Tooltip(
            "The minimum visibility value of character motor before being detected in enemy's FOV.\n" +
            "NOTE: Proximity check does NOT take visibility values into account.")]
        [Range(0, 1)]
        [SerializeField] protected float minDetectLevel = 0.2f;

        protected AcquireTargetResult<CharacterMotor2D> lastScanResult;

        public Comparer.CompareCharacterMotor2D DefaultComparer => new(transform.position);



        protected virtual void OnPlayerDetected() { }
        protected virtual void OnPlayerLost() { }



        protected void UpdateCurrentTarget()
        {
            var newScanResult = AcquireTarget(DefaultComparer);

            if (newScanResult.target == lastScanResult.target) return;

            lastScanResult = newScanResult;

            if (lastScanResult.target)
            {
                if (lastScanResult.target.Visibility > minDetectLevel || lastScanResult.proximityChecked)
                {
                    OnPlayerDetected();
                }
            }
            else
            {
                OnPlayerLost();
            }
        }
    }
}
