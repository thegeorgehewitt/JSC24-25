using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.Decorative
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractCursor : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private LineRenderer lineRenderer;

        [Header("TARGET CURSOR")]
        [SerializeField] private float cursorMargin = 0.3f;
        [SerializeField] private float zoomDuration = 0.1f;
        [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine zoomCoroutine;

        private Vector2 sliceToBoundsRatio;
        private Vector2 currentTargetSize;

        private Material lineRendererMat;

        private List<SpriteRenderer> childRenderers = new();



#if UNITY_EDITOR
        private void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            lineRenderer = GetComponentInChildren<LineRenderer>();
        }
#endif

        private void Awake()
        {
            childRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();

            sliceToBoundsRatio = spriteRenderer.bounds.size / spriteRenderer.size;
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;

            lineRendererMat = lineRenderer.material;
        }



        private IEnumerator ZoomCoroutine(Vector2 _targetSize)
        {
            float elapsedTime = 0;
            Vector2 orgSize = spriteRenderer.size;

            while (elapsedTime < zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                spriteRenderer.size = Vector2.Lerp(orgSize, _targetSize, zoomCurve.Evaluate(elapsedTime / zoomDuration));
                yield return null;
            }

            spriteRenderer.size = _targetSize;
        }



        public void SetSize(Vector2 _size)
        {
            if (currentTargetSize == _size) return;

            if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);

            currentTargetSize = _size;

            var targetSize = (_size + (Vector2.one * cursorMargin)) / sliceToBoundsRatio;
            zoomCoroutine = StartCoroutine(ZoomCoroutine(targetSize));
        }

        public void SetPosition(Vector2 _target)
        {
            transform.position = _target;
        }

        public void SetLinePosition(Vector2 _origin, Vector2 _target)
        {
            lineRenderer.SetPosition(0, _origin);
            lineRenderer.SetPosition(1, _target);
        }

        public void SetColor(Color _color)
        {
            foreach (var renderer in childRenderers)
            {
                renderer.color = _color;
            }

            lineRenderer.startColor = _color;
            lineRenderer.endColor = _color;
        }

        public void SetLineFadeAmount(float _amount)
        {
            lineRendererMat.SetFloat("_Fade_Amount", _amount);
        }

        public void SetLineActive(bool _active)
        {
            lineRenderer.enabled = _active;
        }
    }
}
