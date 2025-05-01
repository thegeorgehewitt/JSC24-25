using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Manager.EventHandling;
using Custom.Interactable.Interfaces;
using Custom.Manager;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using Custom.Manager.Audio;

namespace Custom.Interactable
{
    public class InteractableDoorControlTerminal : MonoBehaviour, IProximityInputReceiver, IPersistent
    {
        public class DoorTerminalLoadedEvent
        {
            public InteractableDoorControlTerminal terminal;
            public bool isUnlocked;

            public DoorTerminalLoadedEvent(InteractableDoorControlTerminal _terminal, bool _isUnlocked)
            {
                terminal = _terminal;
                isUnlocked = _isUnlocked;
            }
        };

        public class DoorUnlockedEvent
        {
            public InteractableDoorControlTerminal terminal;
            
            public DoorUnlockedEvent(InteractableDoorControlTerminal _terminal)
            {
                terminal = _terminal;
            }
        };


        [SerializeField] private string key;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject popup;

        [Space]
        [SerializeField] private Sprite deactiveSprite;
        [SerializeField] private Sprite activeSprite;

        private bool doorUnlocked;
        private Collider2D[] colliders;



        private void Awake()
        {
            colliders = GetComponentsInChildren<Collider2D>();
        }

        private void Start()
        {
            EventAggregator.Publish(new DoorTerminalLoadedEvent(this, false));
        }



        public void Interact()
        {
            if (!doorUnlocked)
            {
                doorUnlocked = true;
            }

            //foreach (var collider in colliders)
            //{
            //    collider.enabled = false;
            //}

            spriteRenderer.sprite = deactiveSprite;
            OnUnfocus();

            EventAggregator.Publish(new DoorUnlockedEvent(this));
        }



        #region IProximityInputReceiver
        public void OnInputReceived(Key _key, KeyPhase _phase)
        {
            if (doorUnlocked) return;

            if (_key == Key.E && _phase == KeyPhase.Released)
            {
                AudioManager.PlaySFX(SFXGroup.ObjectiveTerminal, transform.position, 1.0f);

                Interact();
            }
        }

        public void OnFocus()
        {
            if (doorUnlocked) return;

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
                    if (savedStateData.completed && !doorUnlocked)
                        Interact();

                    if (!savedStateData.completed && doorUnlocked)
                    {
                        doorUnlocked = false;
                        spriteRenderer.sprite = activeSprite;

                        //foreach (var collider in colliders)
                        //{
                        //    collider.enabled = true;
                        //}

                        EventAggregator.Publish(new DoorTerminalLoadedEvent(this, false));
                    }
                }
            }
        }

        public void SaveData(PersistentData data)
        {
            ObjectiveTerminalData savedStateData = data.objectiveStates.Find(MatchesKey);

            if (savedStateData != default(ObjectiveTerminalData))
            {
                savedStateData.completed = doorUnlocked;
            }
            else
            {
                data.objectiveStates.Add(new ObjectiveTerminalData { Key = this.key, completed = doorUnlocked });
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