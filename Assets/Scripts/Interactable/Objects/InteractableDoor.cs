using System.Collections;

using UnityEngine;

using FunkyCode;

using Custom.Manager;

namespace Custom.Interactable
{
    using Interfaces;
    using System;
    using System.Xml.Linq;
    using UnityEngine.InputSystem;

    public class InteractableDoor : InteractableObject, IToggleable, IOverloadable, IAnimEvent, IPersistent
    {
        [Header("DOOR REFERENCES")]
        [SerializeField] private Collider2D doorCollider;
        [SerializeField] private Animator animator;
        public string key;

        [Header("OPEN & CLOSE")]
        [SerializeField] private bool open = false;

        [Header("DEADLOCK & UNLOCK")]
        [SerializeField] private bool deadlocked = false;
        [SerializeField] private Sprite deadlockedSprite;
        [SerializeField] private Sprite unlockedSprite;

        [Header("OVERLOAD")]
        [SerializeField] private bool overloaded = false;
        [SerializeField] private Sprite overloadedSprite;

        private Coroutine openCoroutine;



#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!doorCollider) return;

            Open(open);
        }
#endif

        private void Start()
        {
            UpdateStates();
        }



        private void SetOpen(bool _open)
        {
            doorCollider.enabled = !_open;

            UpdateStates();
        }

        private void UpdateStates(bool _open)
        {
            states.Clear();
            states.Add(open ? "Open" : "Closed");
            if (deadlocked) states.Add("Deadlocked");
        }

        private void UpdateStates()
        {
            states.Clear();
            states.Add(open ? "Open" : "Closed");
            if (deadlocked) states.Add("Deadlocked");
        }

        private void Open(bool _open)
        {
#if UNITY_EDITOR 
            if (!Application.isPlaying)
            {
                SetOpen(_open);
            }
            else if (isActiveAndEnabled)
#endif
            {
                UpdateStates();

                animator.SetBool("Open", _open);
                animator.SetBool("Close", !_open);
            }
        }


        public void Toggle()
        {
            if (deadlocked) return;

            if (animator.GetBool("Open") || animator.GetBool("Close")) return;

            open = !open;

            animator.SetBool("Open", false);
            animator.SetBool("Close", false);

            Open(open);
        }

        public void Overload()
        {
            if (overloaded) return;

            overloaded = true;

            open = true;

            spriteRenderer.sprite = overloadedSprite ? overloadedSprite : null;
            spriteRenderer.color = Color.white;
            // play destruction anim

            // AOE damage if not in interface

            UpdateStates();
        }

        public void ToggleDeadlock()
        {
            deadlocked = !deadlocked;

            spriteRenderer.sprite = deadlocked ? deadlockedSprite : unlockedSprite;

            SetOpen(open);
            UpdateStates();
        }

        public void AnimEvent()
        {
            SetOpen(open);

            animator.SetBool("Open", false);
            animator.SetBool("Close", false);
        }

        public void LoadData(PersistentData data)
        {
            if (data != null)
            {
                DoorData savedStateData = data.doorStates.Find(MatchesKey);

                if (savedStateData != default(DoorData) )
                {
                    if (open != savedStateData.doorOpenState)
                    {
                        Toggle();
                    }
                    if (deadlocked != savedStateData.doorDeadlockedState)
                    {
                        ToggleDeadlock();
                    }
                }
            }
        }

        public void SaveData(PersistentData data)
        {
            DoorData savedData = data.doorStates.Find(MatchesKey);

            if (savedData != default(DoorData))
            {
                savedData.doorOpenState = open;
                savedData.doorDeadlockedState = deadlocked;
            }
            else
            {
                data.doorStates.Add(new DoorData { Key = this.key, doorOpenState = open, doorDeadlockedState = deadlocked });
            }
        }

        public void GenerateGuid()
        {
            key = Guid.NewGuid().ToString();
        }

        protected bool MatchesKey(DoorData data)
        {
            if (data == null) return false;
            return data.Key == key;
        }
    }
}
