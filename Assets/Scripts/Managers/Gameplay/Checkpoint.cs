using Custom.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Custom.Checkpoint
{
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        private BoxCollider2D col;

        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                CheckpointManager.Instance.UpdateCheckpoint(transform.position);
                SaveSystem.Instance.SaveGame();
            }
        }
    }
}

