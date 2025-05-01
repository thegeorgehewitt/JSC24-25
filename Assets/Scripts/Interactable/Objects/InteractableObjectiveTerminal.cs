using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

using Custom.Manager.EventHandling;
using Custom.Interactable.Interfaces;
using Custom.Manager;
using Custom.Manager.Audio;

namespace Custom.Interactable
{
    public class InteractableObjectiveTerminal : MonoBehaviour, IProximityInputReceiver, IPersistent
    {
        public class DataCollectedEvent { };
        public class DataUncollectedEvent { };
        public class TerminalLoadedEvent { };



        [SerializeField] private string key;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject popup;

        [Space]
        [SerializeField] private Sprite deactiveSprite;
        [SerializeField] private Sprite activeSprite;

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



        public void Interact()
        {
            if (!dataCollected)
            {
                dataCollected = true;

                // Run data collection animation here (or just change the renderer's sprite)
            }

            //foreach (var collider in colliders)
            //{
            //    collider.enabled = false;
            //}

            spriteRenderer.sprite = deactiveSprite;
            OnUnfocus();

            EventAggregator.Publish<DataCollectedEvent>(null);
        }



        #region IProximityInputReceiver
        public void OnInputReceived(Key _key, KeyPhase _phase)
        {
            if (dataCollected) return;

            if (_key == Key.E && _phase == KeyPhase.Released)
            {
                AudioManager.PlaySFX(SFXGroup.ObjectiveTerminal, transform.position, 1.0f);

                Interact();
            } 
        }

        public void OnFocus()
        {
            if (dataCollected) return;

            popup.SetActive(true);

            OutlineManager.Register(spriteRenderer);
        }

        public void OnUnfocus()
        {
            popup.SetActive(false);

            OutlineManager.Unregister(spriteRenderer);
        }
        #endregion

        #region IPersistent
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
                        spriteRenderer.sprite = activeSprite;

                        //foreach (var collider in colliders)
                        //{
                        //    collider.enabled = true;
                        //}

                        EventAggregator.Publish<DataUncollectedEvent>(null);
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
            EditorUtility.SetDirty(this);
        }

        protected bool MatchesKey(ObjectiveTerminalData data)
        {
            if (data == null) return false;
            return data.Key == key;
        }
        #endregion
    }
}