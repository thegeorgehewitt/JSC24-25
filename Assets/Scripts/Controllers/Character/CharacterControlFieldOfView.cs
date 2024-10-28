using UnityEngine;
using UnityEngine.Rendering.Universal;

using Custom.Utility;

namespace Custom.Controller
{
    public class CharacterControlFieldOfView : CharacterControlBase
    {
        [Header("REFERENCES")]
        [SerializeField] private FieldOfView fieldOfView;
        [SerializeField] private Light2D flashlight;

        [Header("FIELD OF VIEW")]
        [SerializeField] private float radius;
        [Range(0, 360)]
        [SerializeField] private float angle;
        [SerializeField] private LayerMask blockableLayers;

        public float Radius 
        { 
            get { return radius; } 
            set 
            { 
                radius = value;
                fieldOfView.Radius = value;
            } 
        }

        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                fieldOfView.Angle = value;
            }
        }



#if UNITY_EDITOR
        private void OnValidate()
        {
            #region FOV
            if (fieldOfView)
            {
                fieldOfView.Radius = radius;
                fieldOfView.Angle = angle;
                fieldOfView.blockableFilter.layerMask = blockableLayers;
            }
            #endregion

            #region Flashlight
            if (flashlight)
            {
                flashlight.lightType = Light2D.LightType.Point;
                flashlight.pointLightInnerAngle = fieldOfView.Angle;
                flashlight.pointLightOuterAngle = fieldOfView.Angle + 5.0f;
                flashlight.pointLightOuterRadius = fieldOfView.Radius;
            }
            #endregion
        }
#endif

        private void FixedUpdate()
        {
            Vector3 mouseWorldPos = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            float rotation = Vector2.SignedAngle(transform.up, mouseWorldPos - fieldOfView.transform.position);

            fieldOfView.Rotation = rotation;
            flashlight.transform.eulerAngles = new Vector3(0, 0, rotation);
        }
    }
}
