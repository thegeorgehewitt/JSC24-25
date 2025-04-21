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
        private bool crouching;



        void Start()
        {
            platformCollider = GetComponent<Collider2D>();
            platformCollider.usedByEffector = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            colliderCounter++;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            colliderCounter--;

            if (colliderCounter <= 0)
                platformCollider.isTrigger = false;
        }



        public void OnInputReceived(Key _key, KeyPhase _phase)
        {
            if (_key == Key.S)
            {
                if (_phase == KeyPhase.Pressed)
                    crouching = true;
                else if (_phase == KeyPhase.Released)
                    crouching = false;
            }
            else if (_key == Key.Space && crouching && _phase == KeyPhase.Pressed)
            {
                platformCollider.isTrigger = true;
                crouching = false;
            }
        }
    }
}
