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
                fieldOfView.radius = value;
            } 
        }

        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                fieldOfView.angle = value;
            }
        }



        private void Start()
        {
            fieldOfView.radius = radius;
            fieldOfView.angle = angle;
            fieldOfView.blockableFilter.layerMask = blockableLayers;
        }

        private void Update()
        {
            Vector3 mouseWorldPos = CameraController.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            fieldOfView.rotation = Vector2.SignedAngle(fieldOfView.transform.up, mouseWorldPos - fieldOfView.transform.position);
        }
    }
}
