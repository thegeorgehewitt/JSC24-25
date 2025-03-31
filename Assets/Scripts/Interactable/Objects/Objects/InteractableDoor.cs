using System.Collections;

using UnityEngine;

using FunkyCode;

using Custom.Manager;

namespace Custom.Interactable
{
    using Interfaces;

    public class InteractableDoor : InteractableObject, IToggleable, IOverloadable
    {
        [Header("DOOR REFERENCES")]
        [SerializeField] private Collider2D doorCollider;
        [SerializeField] private LightCollider2D lightCollider;

        [Header("OPEN & CLOSE")]
        [SerializeField] private bool open = false;
        [SerializeField] private Color closedColor = Color.white;
        [SerializeField] private Color openedColor = Color.white / 2;
        [SerializeField] private float easeDuration = 0.1f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private string soundName;

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
            SoundManager.Instance.PlaySFX(soundName, this.transform.position, 1f);
            
            spriteRenderer.color = _open ? openedColor : closedColor;
            doorCollider.enabled = !_open;
            if (lightCollider) lightCollider.enabled = !_open;


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
                if (openCoroutine != null) StopCoroutine(openCoroutine);

                openCoroutine = StartCoroutine(OpenCoroutine(_open));
            }
        }

        private IEnumerator OpenCoroutine(bool _open)
        {
            float elapsedTime = 0;
            Color orgColor = spriteRenderer.color;
            Color targetColor = _open ? openedColor : closedColor;

            while (elapsedTime < easeDuration)
            {
                elapsedTime += TimeManager.DeltaTime;
                spriteRenderer.color = Color.Lerp(orgColor, targetColor, easeCurve.Evaluate(elapsedTime / easeDuration));

                yield return null;
            }

            SetOpen(_open);
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
    }
}
