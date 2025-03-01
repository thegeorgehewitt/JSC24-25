using Custom.Controller;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform : MonoBehaviour
{
    private Collider2D platformCollider;
    private PlatformEffector2D effector;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        effector = GetComponent<PlatformEffector2D>();
    }

    public void DropThroughPlatform(Collider2D player)
    {
        Physics2D.IgnoreCollision(player, platformCollider, true);
        Invoke(nameof(ResetCollision), 2f);
    }

    private void ResetCollision()
    {
        Collider2D playerCollider = FindObjectOfType<PlayerController>().GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}
