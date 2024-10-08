using System.Collections;

using UnityEngine;

using Custom.Attribute;

namespace Custom.Effect
{
    public abstract class SpriteEffectBase : MonoBehaviour
    {
        // To edit serialized properties hidden in inspector,
        // assign references directly in the script's inspector.

        [HideInInspector] public SpriteRenderer spriteRenderer;

        [ReadOnly]
        [SerializeField] protected Material effectMaterial;
        [SerializeField] protected AnimationCurve defaultCurve = AnimationCurve.Linear(0, 0, 1, 1);

        protected Material matInstance;
        private Coroutine easeCoroutine;



        protected void Reset()
        {
            if (TryGetComponent(out SpriteRenderer asSpriteRenderer))
            {
                spriteRenderer = asSpriteRenderer;
            }
            else
            {
                enabled = false;
            }
        }

        protected void Start()
        {
            matInstance = Instantiate(effectMaterial);
            spriteRenderer.material = matInstance;
        }



        protected abstract IEnumerator EaseInCoroutine(float _duration, AnimationCurve _curve);
        protected abstract IEnumerator EaseOutCoroutine(float _duration, AnimationCurve _curve);

        private void StopCoroutine()
        {
            if (easeCoroutine != null) StopCoroutine(easeCoroutine);
        }



        public void EaseIn(float _duration)
        {
            StopCoroutine();
            easeCoroutine = StartCoroutine(EaseInCoroutine(_duration, defaultCurve));
        }

        public void EaseIn(float _duration, AnimationCurve _curve)
        {
            StopCoroutine();
            easeCoroutine = StartCoroutine(EaseInCoroutine(_duration, _curve));
        }

        public void EaseOut(float _duration)
        {
            StopCoroutine();
            easeCoroutine = StartCoroutine(EaseOutCoroutine(_duration, defaultCurve));
        }

        public void EaseOut(float _duration, AnimationCurve _curve)
        {
            StopCoroutine();
            easeCoroutine = StartCoroutine(EaseOutCoroutine(_duration, _curve));
        }
    }
}