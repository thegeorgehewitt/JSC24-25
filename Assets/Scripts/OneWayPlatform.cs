using Custom.Controller;
using Custom.Interactable.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class OneWayPlatform : MonoBehaviour, IProximityInputReceiver
{
    private Collider2D platformCollider;
    private PlatformEffector2D effector;

    private int colliderCounter;



    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        effector = GetComponent<PlatformEffector2D>();
    }

    public void DropThroughPlatform()
    {
        platformCollider.isTrigger = true;

        colliderCounter = 0;
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

    public void OnInputReceived(Key _key)
    {
        if (_key == Key.S)
        {
            DropThroughPlatform();
        }
    }
            
}
