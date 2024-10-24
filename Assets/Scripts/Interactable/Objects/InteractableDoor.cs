using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

using Custom.Manager;

namespace Custom.Interactable
{
    public class InteractableDoor : InteractableObject
    {
        [Header("DOOR REFERENCES")]
        [SerializeField] private Collider2D doorCollider;
        [SerializeField] private ShadowCaster2D shadowCaster;

        [Header("OPEN & CLOSE")]
        [SerializeField] private bool open = false;
        [SerializeField] private Color closedColor = Color.white;
        [SerializeField] private Color openedColor = Color.white / 2;
        [SerializeField] private float easeDuration = 0.1f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine openCoroutine;



#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            shadowCaster = GetComponentInChildren<ShadowCaster2D>();
        }

        private void OnValidate()
        {
            if (!doorCollider) return;

            Open(open);
        }
#endif

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }



        private void SetState(bool _open)
        {
            spriteRenderer.color = _open ? openedColor : closedColor;
            doorCollider.enabled = !_open;
            if (shadowCaster) shadowCaster.enabled = !_open;

            // TEMPORARY
            states = new List<string>{ _open ? "Unlocked" : "Locked" };
        }

        private void Open(bool _open)
        {
#if UNITY_EDITOR 
            if (!Application.isPlaying)
            {
                SetState(_open);
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

            SetState(_open);
        }



        public override void Interact()
        {
            open = !open;

            Open(open);
        }
    }
}
