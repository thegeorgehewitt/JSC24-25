using Custom.Controller;
using Custom.Manager;
using Custom.Manager.EventHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Custom.Controller.CharacterControlDamageable;

namespace Custom.Checkpoint
{
    public class CheckpointManager : MonoBehaviour, IPersistent
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
            if (currentCheckpoint == Vector3.zero)
            {
                currentCheckpoint = PlayerMotorController.Instance.transform.position;
            }
            ReloadCheckpoint();
        }

        public void UpdateCheckpoint(Vector3 newCheckpoint)
        {
            currentCheckpoint = newCheckpoint;
        }

        public void ReloadCheckpoint()
        {
            PlayerMotorController.PauseMotor(false, true);
            SaveSystem.Instance.LoadGame();
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

        public void LoadData(PersistentData data)
        {
            if (data != null && data.checkpoint != Vector3.zero)
            {
                currentCheckpoint = data.checkpoint;
            }
        }

        public void SaveData(PersistentData data)
        {
            data.checkpoint = currentCheckpoint;
        }

        public void GenerateGuid()
        {
            // no implementation needed
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
