using System.Collections;
using UnityEngine;

namespace Custom.Effect
{
    [DisallowMultipleComponent]
    public class SpriteHologram : SpriteEffectBase
    {
        private IEnumerator EaseCore(float _targetValue, float _duration, AnimationCurve _curve)
        {
            float elapsedTime = 0;
            float orgAmount = matInstance.GetFloat("_Amount");

            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                matInstance.SetFloat("_Amount", Mathf.Lerp(orgAmount, _targetValue, _curve.Evaluate(elapsedTime / _duration * _curve.length)));
                yield return null;
            }
            matInstance.SetFloat("_Amount", _targetValue);
        }



        protected override IEnumerator EaseInCoroutine(float _duration, AnimationCurve _curve)
        {
            return EaseCore(1.0f, _duration, _curve);
        }

        protected override IEnumerator EaseOutCoroutine(float _duration, AnimationCurve _curve)
        {
            return EaseCore(0.0f, _duration, _curve);
        }
    }
}
