using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairHandler : MonoBehaviour
{
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private bool isOnSlope;
    private bool canWalkOnSlope;
    private float xInput;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;
    private Vector2 slopeNormalPerp;
    private Vector2 newVelocity;

    private float movespeed = 5;

    [SerializeField]
    private PhysicsMaterial2D noFriction;
    [SerializeField]
    private PhysicsMaterial2D fullFriction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();       
        cc = GetComponent<CapsuleCollider2D>(); 
    }

    private void Update()
    {
        CheckInput();
    }

    void FixedUpdate()
    {
        SlopeCheck();
        AdjustMovementOnSlope();
    }

    void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, cc.size.y / 2));
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);

        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            Debug.Log($"Slope Side Angle (Front): {slopeSideAngle}");
            Debug.DrawRay(slopeHitFront.point, slopeHitFront.normal, Color.red);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            Debug.Log($"Slope Side Angle (Back): {slopeSideAngle}");
            Debug.DrawRay(slopeHitBack.point, slopeHitBack.normal, Color.yellow);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            //isOnSlope = slopeDownAngle != 0 && slopeDownAngle <= maxSlopeAngle;
            //canWalkOnSlope = slopeDownAngle <= maxSlopeAngle;

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, hit.normal, Color.green);
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.Log($"Slope Angle (Vertical): {slopeDownAngle}, Can Walk: {canWalkOnSlope}");
        }
        else
        {
            isOnSlope = false;
            canWalkOnSlope = true;
        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && xInput == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
        }
    }


    private void AdjustMovementOnSlope()
    {
        float speed = rb.velocity.magnitude;
        newVelocity.Set(movespeed* slopeNormalPerp.x * -xInput, movespeed* slopeNormalPerp.y * -xInput);
        rb.velocity = newVelocity;


        Debug.Log($"Adjusting movement on slope: {rb.velocity}");
        Debug.DrawRay(transform.position, rb.velocity, Color.magenta);
    }
}
