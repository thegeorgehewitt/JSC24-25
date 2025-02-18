using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairDetector : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool isOnStair = false;
    private float originalGravity;
    public float stairSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        if (isOnStair)
        {
            rb.velocity = new Vector2(move * stairSpeed, stairSpeed * Mathf.Sign(move));
        }
    }

    public void DescendStair()
    {
        if (isOnStair)
        {
            rb.gravityScale = originalGravity;
            rb.velocity = new Vector2(rb.velocity.x, -stairSpeed);
            isOnStair = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Stairway"))
        {
            isOnStair = true;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Stairway"))
        {
            isOnStair = false;
            rb.gravityScale = originalGravity;
        }
    }
}