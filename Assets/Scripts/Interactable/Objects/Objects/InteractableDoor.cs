using System.Collections;

using UnityEngine;

using FunkyCode;

using Custom.Manager;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableDoor : InteractableObject, IToggleable, IOverloadable, IAnimEvent
    {
        [Header("DOOR REFERENCES")]
        [SerializeField] private Collider2D doorCollider;
        [SerializeField] private Animator animator;

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

                animator.SetTrigger(_open ? "Open" : "Close");
            }
        }


        public void Toggle()
        {
            if (deadlocked) return;

            open = !open;

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

            animator.ResetTrigger("Open");
            animator.ResetTrigger("Close");
        }
    }
}
