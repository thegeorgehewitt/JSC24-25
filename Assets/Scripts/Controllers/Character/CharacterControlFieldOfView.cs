using UnityEngine;

using Custom.Utility;

namespace Custom.Controller
{
    public class CharacterControlFieldOfView : CharacterControlBase
    {
        [Header("REFERENCES")]
        [SerializeField] private FieldOfView fieldOfView;

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
            if (!fieldOfView) return;

            fieldOfView.Radius = radius;
            fieldOfView.Angle = angle;
            fieldOfView.blockableFilter.layerMask = blockableLayers;
        }
#endif

        private void Update()
        {
            Vector3 mouseWorldPos = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            fieldOfView.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(transform.up, mouseWorldPos - fieldOfView.transform.position));
        }
    }
}
