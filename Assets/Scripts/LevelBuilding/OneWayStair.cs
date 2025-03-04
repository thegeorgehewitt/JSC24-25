using UnityEngine;

namespace Custom.LevelBuilding
{
    public class OneWayStair : MonoBehaviour
    {
        [SerializeField] public Transform entryPoint;
        [SerializeField] public Transform exitPoint;

        private bool isNearStair = false;
        private GameObject player;



        void Update()
        {
            if (isNearStair)
            {
                TeleportPlayer();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isNearStair = true;
                player = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isNearStair = false;
                player = null;
            }
        }



        private void TeleportPlayer()
        {
            if (player != null)
            {
                player.transform.position = exitPoint.position;
            }
        }
    }
}
