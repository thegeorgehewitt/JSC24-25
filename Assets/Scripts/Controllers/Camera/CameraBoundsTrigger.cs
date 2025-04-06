using UnityEngine;

namespace Custom.Controller.General
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraBoundsTrigger : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D bounds;
        [SerializeField] private LayerMask triggerLayers;



        private void Awake()
        {
            if (!bounds) bounds = GetComponent<BoxCollider2D>();

            bounds.isTrigger = true;
            bounds.includeLayers = triggerLayers;
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            CameraController2D.SetOuterBounds(bounds.bounds);
        }
    } 
}
