using UnityEngine;

namespace Custom.Controller
{
    public class CameraController : MonoBehaviour
    {
        [Header("REFERENCES")]
        [SerializeField] private Camera mainCamera;

        [Header("TRACKING")]
        [SerializeField] private Transform trackingTransform;
        [SerializeField] private Vector2 offset;

        public Transform TrackingTransform
        {
            get { return trackingTransform; }
            set
            {

            }
        }



        private void OnEnable()
        {
            PlayerController.OnControlledMotorChanged += OnControlledMotorChanged;
        }

        private void OnDisable()
        {
            PlayerController.OnControlledMotorChanged -= OnControlledMotorChanged;
        }

        private void Awake()
        {
            if (!mainCamera) mainCamera = Camera.main;
        }

        private void Update()
        {
            UpdateTrackPosition();
        }



        private void UpdateTrackPosition()
        {
            if (!trackingTransform) return;

            Vector3 trackPos = trackingTransform.position + (Vector3)offset;
            trackPos.z = -10;

            transform.position = trackPos;
        }

        private void OnControlledMotorChanged(CharacterMotor2D _motor)
        {
            trackingTransform = _motor.transform;
        }
    }
}
