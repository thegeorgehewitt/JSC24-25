using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IStair : MonoBehaviour
{
    public Transform entryPoint;
    public Transform exitPoint;
    private bool isNearStair = false;
    private GameObject player;

    void Update()
    {
        if (isNearStair)
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (player != null)
        {
            player.transform.position = exitPoint.position;
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
}
