using UnityEngine;
using UnityEngine.InputSystem;

using Custom.Interactable.Interfaces;

namespace Custom.LevelBuilding
{
    [RequireComponent(typeof(Collider2D), typeof(PlatformEffector2D))]
    public class OneWayPlatform : MonoBehaviour, IProximityInputReceiver
    {
        private Collider2D platformCollider;

        private int colliderCounter;



        void Start()
        {
            platformCollider = GetComponent<Collider2D>();
            platformCollider.usedByEffector = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            colliderCounter++;

            Debug.Log($"Overlap with: {collision.name} + {collision.GetType()}");
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            colliderCounter--;

            Debug.Log($"End Overlap with: {collision.name} + {collision.GetType()}");

            if (colliderCounter <= 0)
                platformCollider.isTrigger = false;
        }



        public void DropThroughPlatform()
        {
            platformCollider.isTrigger = true;
        }

        public void OnInputReceived(Key _key)
        {
            if (_key == Key.S)
            {
                DropThroughPlatform();
            }
        }
    }
}
