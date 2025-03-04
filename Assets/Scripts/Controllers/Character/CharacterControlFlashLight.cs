using UnityEngine;
using UnityEngine.InputSystem;

using FunkyCode;

namespace Custom.Controller
{
    public class CharacterControlFlashLight : CharacterControlBase
    {
        public override string[] InputActionKeysName 
        { 
            get => new string[] 
            { 
                "Toggle"
            }; 
        }



        [Header("REFERENCES")]
        [SerializeField] private Light2D flashLight;
        [SerializeField] private Light2D proximityLight;

        [Header("FLASHLIGHT")]
        [SerializeField] private bool activated;
        [SerializeField] private float radius = 15.0f;
        [Range(0, 360)]
        [SerializeField] private float angle = 60.0f;

        [Header("PROXIMITY LIGHT")]
        [SerializeField] private float offAlpha = 0.1f;
        [SerializeField] private float onAlpha = 1.0f;


        public float Radius 
        {
            get => radius;
            set 
            { 
                radius = value;
                flashLight.size = value;
            }
        }

        public float Angle
        {
            get => angle;
            set
            {
                angle = value;
                flashLight.spotAngleInner = value;
                flashLight.spotAngleOuter = value;
            }
        }



#if UNITY_EDITOR
        private void OnValidate()
        {
            #region Flashlight
            if (flashLight)
            {
                flashLight.lightType = Light2D.LightType.Sprite;
                flashLight.size = radius;
                flashLight.spotAngleInner = angle;
                flashLight.spotAngleOuter = angle;

                SetActiveState(activated);
            }
            #endregion
        }
#endif

        private void OnEnable()
        {
            GetInputActionWithName("Toggle").performed += OnToggle;
        }

        private void OnDisable()
        {
            GetInputActionWithName("Toggle").performed -= OnToggle;
        }

        private void FixedUpdate()
        {
            Vector3 mouseWorldPos = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            float rotation = Vector2.SignedAngle(transform.up, mouseWorldPos - flashLight.transform.position);

            flashLight.transform.localEulerAngles = new Vector3(0, 0, rotation);
        }



        private void OnToggle(InputAction.CallbackContext _context)
        {
            activated = !activated;

            SetActiveState(activated);
        }

        private void SetActiveState(bool _state)
        {
            flashLight.enabled = activated;

            proximityLight.eventPresetId = activated ? 1 : 0;
            proximityLight.color.a = activated ? onAlpha : offAlpha;
        }
    }
}
