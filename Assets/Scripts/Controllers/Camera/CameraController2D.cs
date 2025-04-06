using System;
using System.Collections;

using UnityEngine;

namespace Custom.Controller.General
{
    [ExecuteInEditMode]
    public class CameraController2D : MonoBehaviour
    {
        public static CameraController2D Instance { get; private set; }

        public static event Action OnTargetLocationReached;



        [SerializeField] private Camera mainCamera;

        /*
         * Tracking Controls
         * Allow mainCamera to attach to an object's transform.
         */
        [SerializeField] private Transform trackingTransform;

        [Tooltip("Toggle camera custom speed. Disable to snap camera to tracking transform.")]
        [SerializeField] private bool enableSnapping = false;

        [Tooltip("Ease curve used while enableSnapping is enabled.")]
        [SerializeField] private AnimationCurve snapEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Tooltip("Seconds until tracking transform is directly in the middle of camera view after changing tracking transform.")]
        [SerializeField] private float snapDuration = 0.5f;

        [Tooltip("Movement speed is calculated by multiplying this value with distance between camera and tracking transform.")]
        [Min(0)]
        [SerializeField] private float speedMultiplier = 0.5f;

        [Tooltip("Maximum tracking speed.")]
        [Min(0.01f)]
        [SerializeField] private float maxSpeed = 1.0f;

        [Tooltip("Maximum tracking speed. This is used to prevent infinitely small movement speed.")]
        [Min(0.01f)]
        [SerializeField] private float minSpeed = 0.01f;

        /*
         * Boundary Controls
         * Allow camera view to fit within a defined boundary.
         */
        [Tooltip("Should camera's view be forced to fit within a defined boundary?")]
        [SerializeField] public bool enableBounds = false;

        [Tooltip("Should camera's orthographic size be shrink to fit within the defined boundary?")]
        [SerializeField] public bool enableHardLock = false;

        [Tooltip("Camera's view will be forced to stay within this area.")]
        [SerializeField] public Rect outerBounds = new(0, 0, 20, 10);

        [SerializeField] [HideInInspector] private Bounds validBounds;

        /*
         * Panning and Zoom Controls
         */
        [SerializeField] private float minOrthographicSize = 1.0f;
        [SerializeField] private float maxOrthographicSize = 3.0f;

        private float zOffset;

        public static Camera MainCamera { get { return Instance?.mainCamera ?? Camera.main; } }

        public static bool EnableSnapping
        {
            get => Instance.enableSnapping;
            set => Instance.enableSnapping = value;
        }



#if UNITY_EDITOR
        private void Reset()
        {
            mainCamera = GetComponent<Camera>();
            if (!mainCamera) mainCamera = Camera.main;

            Screen.autorotateToLandscapeRight = true;
        }
#endif 

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

            if (!mainCamera)
            {
                enabled = false;
                return;
            }

            zOffset = mainCamera.transform.position.z;
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                SetTargetTransform(trackingTransform);
            }

            RecalculateValidBounds();
            RecalculateZoomConstrains();
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                PlayUpdate();
            }
            else
            {
                EditorUpdate();
            }
        }



        private void EditorUpdate()
        {
            RecalculateValidBounds();
            RecalculateZoomConstrains();
        }

        private void PlayUpdate()
        {
            RecalculateValidBounds();
            RecalculateZoomConstrains();

            if (!isChasing && enableSnapping)
            {
                mainCamera.transform.position = GetTargetPosition();
            }

            // This should always be handled last.
            ClipToBounds();
        }



        #region Tracking
        private Coroutine moveToTransformCoroutine;

        private bool isChasing;



        /// <inheritdoc cref="SetTargetTransform_Core(Transform)"/>
        public static void SetTargetTransform(Transform _newTransform)
        {
            Instance.SetTargetTransform_Core(_newTransform);
        }



        /// <summary>
        /// Set current target transform to a new transform or leave it as null to disable tracking.
        /// </summary>
        /// <param name="_newTransform">    Target transform to track.      </param>
        /// <param name="_enableSnapping">    Can camera pan to end lock on?  </param>
        private void SetTargetTransform_Core(Transform _newTransform)
        {
            trackingTransform = _newTransform;

            // If null, disable tracking.
            if (!_newTransform)
            {
                if (moveToTransformCoroutine != null) StopCoroutine(moveToTransformCoroutine);
                return;
            }

            if (enableSnapping)
            {
                if (moveToTransformCoroutine != null) StopCoroutine(moveToTransformCoroutine);

                moveToTransformCoroutine = StartCoroutine(SnappingCoroutine());
            }
            else
            {
                if (moveToTransformCoroutine != null) StopCoroutine(moveToTransformCoroutine);

                moveToTransformCoroutine = StartCoroutine(MoveTowardsTargetPosition(GetTargetPosition()));
            }
        }



        private IEnumerator MoveTowardsTargetPosition(Vector3 _targetPosition)
        {
            isChasing = true;

            float distance = 1;
            while (distance > 0)
            {
                distance = Vector3.Distance(mainCamera.transform.position, _targetPosition);
                float speed = Mathf.Clamp(distance * speedMultiplier, minSpeed, maxSpeed);

                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, _targetPosition, speed);

                yield return null;
            }

            mainCamera.transform.position = _targetPosition;
            isChasing = false;

            OnTargetLocationReached?.Invoke();
        }

        private IEnumerator SnappingCoroutine()
        {
            isChasing = true;

            float elapsedTime = 0;
            Vector3 orgPosition = mainCamera.transform.position;
            while (elapsedTime < snapDuration)
            {
                elapsedTime += Time.deltaTime;
                mainCamera.transform.position = Vector3.Lerp(orgPosition, GetTargetPosition(), snapEaseCurve.Evaluate(elapsedTime / snapDuration));

                yield return null;
            }

            mainCamera.transform.position = GetTargetPosition();
            isChasing = false;

            OnTargetLocationReached?.Invoke();
        }

        private Vector3 GetTargetPosition()
        {
            if (!trackingTransform) return transform.position;

            Vector3 trackingPosition = trackingTransform.position;

            if (enableBounds) trackingPosition = validBounds.ClosestPoint(trackingPosition);

            trackingPosition.z = zOffset;

            return trackingPosition;
        }
        #endregion

        #region Bounds Restriction
        public Bounds OuterBounds { get { return new Bounds(outerBounds.center - outerBounds.size / 2, outerBounds.size); } }
        public Bounds ValidBounds { get { return validBounds; } }



        public static void SetOuterBounds(Rect _bounds)
        {
            Instance.outerBounds = _bounds;

            Instance.RecalculateValidBounds();
        }

        public static void SetOuterBounds(Bounds _bounds)
        {
            SetOuterBounds(new Rect(_bounds.center, _bounds.size));
        }



        private void RecalculateValidBounds()
        {
            Vector2 cameraSize = 2 * mainCamera.orthographicSize * new Vector2(mainCamera.aspect, 1.0f);
            validBounds = new Bounds
            {
                center = outerBounds.center - outerBounds.size / 2,
                size = Vector2.Max(outerBounds.size - cameraSize, Vector2.zero)
            };
        }

        private void ClipToBounds()
        {
            if (!enableBounds) return;

            Vector3 clippedPosition = validBounds.ClosestPoint(mainCamera.transform.position);
            clippedPosition.z = zOffset;

            mainCamera.transform.position = clippedPosition;
        }
        #endregion

        #region Zoom
        private float WorldToScreenRatio { get { return Screen.height / mainCamera.orthographicSize; } }

        private void RecalculateZoomConstrains()
        {
            if (!enableBounds || !enableHardLock) return;

            float minBoundsSide = Mathf.Min(outerBounds.height / 2.0f, outerBounds.width / 2.0f / mainCamera.aspect);

            maxOrthographicSize = Mathf.Clamp(maxOrthographicSize, 0.0f, minBoundsSide);
            minOrthographicSize = Mathf.Min(minOrthographicSize, maxOrthographicSize);

            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minOrthographicSize, maxOrthographicSize);
        }
        #endregion
    }
}
