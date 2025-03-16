using Custom.Controller;
using Custom.Manager.EventHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Custom.Controller.CharacterControlDamageable;

namespace Custom.Checkpoint
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance;

        [SerializeField] private Vector3 currentCheckpoint;

        private void OnEnable()
        {
            EventAggregator.Subscribe<DeathEvent>(OnDeath);
        }

        private void OnDisable()
        {
            EventAggregator.Unsubscribe<DeathEvent>(OnDeath);
        }

        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion
        }

        private void Start()
        {
            currentCheckpoint = PlayerMotorController.Instance.transform.position;
        }

        public void UpdateCheckpoint(Vector3 newCheckpoint)
        {
            currentCheckpoint = newCheckpoint;
        }

        public void ReloadCheckpoint()
        {
            PlayerMotorController.PauseMotor(false, true);
            PlayerMotorController.Instance.transform.position = currentCheckpoint;
        }

        private void OnDeath(DeathEvent _event)
        {
            StartCoroutine("HandleDeath");
        }

        private IEnumerator HandleDeath()
        {
            PlayerMotorController.PauseMotor(true);

            yield return new WaitForSeconds(1);

            ReloadCheckpoint();
        }
    }
}
