using System.Collections;

using UnityEngine;
using UnityEngine.U2D;

namespace Custom.Controller
{
    [DisallowMultipleComponent]
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        [Header("REFERENCES")]
        [SerializeField] private Camera mainCamera;

        [Header("TRACKING")]
        [SerializeField] private Transform trackingTransform;
        [SerializeField] public Vector2 offset;
        [SerializeField] private float zDepth;

        [Header("ZOOM")]
        [SerializeField] private AnimationCurve defaultZoomEaseCurve;

        private PixelPerfectCamera pixelPerfectCamera;

        public bool PixelatedCamera { get { return pixelPerfectCamera && pixelPerfectCamera.isActiveAndEnabled; } }

        public static float Size { get { return Instance.mainCamera.orthographicSize; } }



        private void Awake()
        {
            #region Singleton
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            #endregion
        }

        private void Start()
        {
            if (!mainCamera) mainCamera = Camera.main;

            if (TryGetComponent(out PixelPerfectCamera asPixelPerfectCamera)) pixelPerfectCamera = asPixelPerfectCamera;
        }

        private void Update()
        {
            UpdatePosition();
        }



        #region Tracking

        private void UpdatePosition()
        {
            Vector3 trackingPosition = trackingTransform.position + (Vector3)offset;
            trackingPosition.z = -zDepth;

            transform.position = trackingPosition;
        }

        #endregion

        #region Zoom

        private Coroutine zoomCoroutine;



        public static void StartZoom(float _cameraSize, float _easeDuration)
        {
            StartZoom(_cameraSize, _easeDuration, Instance.defaultZoomEaseCurve);
        }

        public static void StartZoom(float _cameraSize, float _easeDuration, AnimationCurve _easeCurve)
        {
            Instance.StartZoomCore(_cameraSize, _easeDuration, _easeCurve);
        }

        public static void StopZoom()
        {
            Instance.StopZoomCore();
        }



        private void StartZoomCore(float _cameraSize, float _easeDuration, AnimationCurve _easeCurve)
        {
            StopZoomCore();

            if (PixelatedCamera)
            {
                zoomCoroutine = StartCoroutine(PixelPerfectZoomCoroutine(_cameraSize, _easeDuration, _easeCurve));
            }
            else
            {
                zoomCoroutine = StartCoroutine(DefaultZoomCoroutine(_cameraSize, _easeDuration, _easeCurve));
            }
        }

        private void StopZoomCore()
        {
            if (zoomCoroutine == null) return;

            StopCoroutine(zoomCoroutine);
        }

        private IEnumerator DefaultZoomCoroutine(float _targetSize, float _easeDuration, AnimationCurve _easeCurve)
        {
            float elapsedTime = 0;
            float orgSize = mainCamera.orthographicSize;

            while (elapsedTime <= _easeDuration)
            {
                elapsedTime += Time.deltaTime;

                mainCamera.orthographicSize = Mathf.Lerp(orgSize, _targetSize, _easeCurve.Evaluate(elapsedTime / _easeDuration * _easeCurve.length));
                yield return null;
            }

            mainCamera.orthographicSize = _targetSize;
        }

        private IEnumerator PixelPerfectZoomCoroutine(float _cameraSize, float _easeDuration, AnimationCurve _easeCurve)
        {
            float elapsedTime = 0;
            int orgSize = pixelPerfectCamera.refResolutionY;
            int targetCameraSize = (int)_cameraSize * 2 * pixelPerfectCamera.assetsPPU;

            while (elapsedTime <= _easeDuration)
            {
                elapsedTime += Time.deltaTime;

                pixelPerfectCamera.refResolutionY = (int)Mathf.Lerp(orgSize, targetCameraSize, _easeCurve.Evaluate(elapsedTime / _easeDuration * _easeCurve.length));
                yield return null;
            }

            pixelPerfectCamera.refResolutionY = targetCameraSize;
        }

        #endregion
    }
}
