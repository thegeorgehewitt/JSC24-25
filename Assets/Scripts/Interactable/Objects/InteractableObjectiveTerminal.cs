using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Manager.EventHandling;
using Custom.Interactable.Interfaces;

namespace Custom.Interactable
{
    public class InteractableObjectiveTerminal : MonoBehaviour, IProximityInputReceiver, IPersistent
    {
        public class DataCollectedEvent { };
        public class TerminalLoadedEvent { };



        [SerializeField] private string key;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private bool dataCollected;
        private Collider2D[] colliders;



        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        private void Start()
        {
            EventAggregator.Publish<TerminalLoadedEvent>(null);
        }



        public void OnInputReceived(Key _key, KeyPhase _phase)
        {
            if (dataCollected) return;

            if (_key == Key.E && _phase == KeyPhase.Released)
            {
                Interact();
            } 
        }

        public void Interact()
        {
            if (!dataCollected)
            {
                dataCollected = true;

                // Run data collection animation here (or just change the renderer's sprite)
            }

            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            EventAggregator.Publish<DataCollectedEvent>(null);
        }

        public void LoadData(PersistentData data)
        {
            if (data != null)
            {
                ObjectiveTerminalData savedStateData = data.objectiveStates.Find(MatchesKey);

                if (savedStateData != default(ObjectiveTerminalData))
                {
                    if (savedStateData.completed && !dataCollected) 
                        Interact();

                    if (!savedStateData.completed && dataCollected)
                    {
                        dataCollected = false;
                        spriteRenderer.color = new Color(0.4f, 0.7f, 0.4f);

                        foreach (var collider in colliders)
                        {
                            collider.enabled = true;
                        }
                    }
                }
            }
        }

        public void SaveData(PersistentData data)
        {
            ObjectiveTerminalData savedStateData = data.objectiveStates.Find(MatchesKey);

            if (savedStateData != default(ObjectiveTerminalData))
            {
                savedStateData.completed = dataCollected;
            }
            else
            {
                data.objectiveStates.Add(new ObjectiveTerminalData { Key = this.key, completed = dataCollected });
            }
        }

        public void GenerateGuid()
        {
            key = System.Guid.NewGuid().ToString();
        }

        protected bool MatchesKey(ObjectiveTerminalData data)
        {
            if (data == null) return false;
            return data.Key == key;
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }
    }
}