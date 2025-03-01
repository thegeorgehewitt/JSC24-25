using Custom.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StairHandler : MonoBehaviour
{
    [SerializeField] private float slopeCheckDistance = 0.1f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject parentBody;

    [SerializeField] PhysicsMaterial2D noneFriction;
    [SerializeField] PhysicsMaterial2D maxFriction;

    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private CharacterMotor2D characterMotor2DRef;
    private InputAction PlayerIA;
    private PlayerInput playerInput;

    public bool IsOnSlope { get; private set; }

    public Vector2 SlopeDirection { get; private set; } = Vector2.right;
    private Vector2 slopeNormalPerp;
    private float slopeDownAngle;
    private float lastSlopeAngle;

    private void Start()
    {
        rb = parentBody.GetComponent<Rigidbody2D>();
        cc = parentBody.GetComponent<CapsuleCollider2D>();
        characterMotor2DRef = GetComponent<CharacterMotor2D>();
        playerInput = GetComponent<PlayerInput>();
        
    }

    private void FixedUpdate()
    {
        
        SlopeCheck();
    }

    public void ChangePlayerFriction()
    {        
    }
    private void SlopeCheck()
    {
        Vector2 checkPos = (Vector2)transform.position - new Vector2(0.0f, cc.size.y / 2);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);
        if (hit)
        {
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            IsOnSlope = slopeDownAngle > 0f && slopeDownAngle < maxSlopeAngle;
            SlopeDirection = slopeNormalPerp;

            Debug.DrawRay(hit.point, slopeNormalPerp * 2f, Color.red, 0.1f);

        }
        else
        {
            IsOnSlope = false;
            SlopeDirection = Vector2.right;
        }
    }
}
