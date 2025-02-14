using Custom.Controller;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(other, platformCollider, false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(other, platformCollider, false);
        }
    }

    public void DropThroughPlatform(Collider2D player)
    {
        Physics2D.IgnoreCollision(player, platformCollider, true);
        Invoke(nameof(ResetCollision), 0.5f);
    }

    private void ResetCollision()
    {
        Collider2D playerCollider = FindObjectOfType<PlayerController>().GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}