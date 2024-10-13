using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Custom.Attribute;
using System.Linq;

namespace Custom.Decorative
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class InteractCursor : MonoBehaviour
    {
        [Header("REFERENCES")]
        [ReadOnly]
        [SerializeField] private new SpriteRenderer renderer;

        [Header("ZOOM")]
        [SerializeField] private float cursorMargin = 0.2f;
        [SerializeField] private float zoomDuration = 0.5f;
        [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine zoomCoroutine;
        private Vector2 sliceToBoundsRatio;
        private Vector2 currentTargetSize;

        private List<SpriteRenderer> renderers = new();



#if UNITY_EDITOR
        private void Reset()
        {
            renderer = GetComponent<SpriteRenderer>();
        }
#endif

        private void Awake()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>().ToList();

            sliceToBoundsRatio = renderer.bounds.size / renderer.size;
            renderer.drawMode = SpriteDrawMode.Sliced;
        }



        private IEnumerator ZoomCoroutine(Vector2 _targetSize)
        {
            float elapsedTime = 0;
            Vector2 orgSize = renderer.size;

            while (elapsedTime < zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                renderer.size = Vector2.Lerp(orgSize, _targetSize, zoomCurve.Evaluate(elapsedTime / zoomDuration));
                yield return null;
            }

            renderer.size = _targetSize;
        }



        public void SetSize(Vector2 _size)
        {
            if (currentTargetSize == _size) return;

            if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);

            currentTargetSize = _size;

            var targetSize = (_size + (Vector2.one * cursorMargin)) / sliceToBoundsRatio;
            zoomCoroutine = StartCoroutine(ZoomCoroutine(targetSize));
        }

        public void SetPosition(Vector2 _position)
        {
            transform.position = _position;
        }

        public void SetColor(Color _color)
        {
            foreach (var renderer in renderers)
            {
                renderer.color = _color;
            }
        }
    }
}
