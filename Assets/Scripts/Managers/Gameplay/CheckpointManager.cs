using Custom.Controller;
using Custom.Manager;
using Custom.Manager.EventHandling;
using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Custom.Controller.CharacterControlDamageable;

namespace Custom.Checkpoint
{
    public class CheckpointManager : MonoBehaviour, IPersistent
    {
        public static CheckpointManager Instance;

        [SerializeField] private float deathDelayDuration = 2f;
        [SerializeField] private float respawnDelayDuration = 1.4f;

        [SerializeField] private Vector3 startingCheckpoint;
        [SerializeField] private Vector3 currentCheckpoint;

        public class ReloadEvent
        {
            public Vector3 Checkpoint { get; }

            public ReloadEvent(Vector3 _checkpoint)
            {
                Checkpoint = _checkpoint;
            }
        }

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
            if ( startingCheckpoint == default )
            {
                Debug.Log("Set Starting Checkpoint at player intial load position.");
            }
            StartCoroutine(HandleReload());
        }

        public void UpdateCheckpoint(Vector3 newCheckpoint)
        {
            //if (currentCheckpoint == newCheckpoint) return;
            currentCheckpoint = newCheckpoint;
            SaveSystem.Instance.SaveGame();
        }

        public void ResetCheckpoint()
        {
            currentCheckpoint = startingCheckpoint;
        }

        private void OnDeath(DeathEvent _event)
        {
            StartCoroutine(HandleDeath());
        }

        public void ReloadCheckpoint()
        {
            StartCoroutine(HandleReload());
        }

        private IEnumerator HandleDeath()
        {
            PlayerMotorController.PauseMotor(true, true, false);

            yield return new WaitForSeconds(deathDelayDuration);

            StartCoroutine(HandleReload());
        }

        public IEnumerator HandleReload()
        {
            EventAggregator.Publish(new ReloadEvent(currentCheckpoint));

            yield return new WaitForSeconds(respawnDelayDuration);

            PlayerMotorController.PauseMotor(false, true);
        }

        public void LoadData(PersistentData data)
        {
            if (data != default && data.checkpoint != default)
            {
                currentCheckpoint = data.checkpoint;
            }
            else
            {
                currentCheckpoint = startingCheckpoint;
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
    }
}
