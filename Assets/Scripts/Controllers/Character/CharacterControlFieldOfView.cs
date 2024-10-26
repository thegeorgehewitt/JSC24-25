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



        private void Start()
        {
            fieldOfView.Radius = radius;
            fieldOfView.Angle = angle;
            fieldOfView.blockableFilter.layerMask = blockableLayers;
        }

        private void Update()
        {
            Vector3 mouseWorldPos = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            fieldOfView.Rotation = Vector2.SignedAngle(fieldOfView.transform.up, mouseWorldPos - fieldOfView.transform.position);
        }
    }
}
