using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier; // Increases movespeed in air
    public float groundDrag; // Slows character when grounded

    [Header("Ground Check Settings")]
    public Transform groundSphere;
    public LayerMask groundLayer;
    public float groundCheckRadius;

    [Header("Miscellaneous")]
    public Camera playerCamera;
    public MouseLook mouseLook;

    private bool isGrounded;
    private bool readyToJump;
    private float speedCoefficient = 10; // Makes character movement more snappy
    private Transform playerOrientation;
    private Rigidbody rigidbody;
    private Vector2 lateralMovementInput;

    private PlayerControls playerControls;

    private void Awake()
    {
        InitializeComponents();
        InitializeMouseLook();
        SubscribeToInputEvents();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        DetectGround();
        ApplyDrag();
        SpeedControl();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void InitializeComponents()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;
        playerControls = new PlayerControls();
        playerOrientation = transform.Find("Orientation");
    }

    private void InitializeMouseLook()
    {
        if (mouseLook)
        {
            mouseLook.Initialize(playerControls);
        }
    }

    private void SubscribeToInputEvents()
    {
        playerControls.std.Move.performed += ctx => lateralMovementInput = ctx.ReadValue<Vector2>();
        playerControls.std.Move.canceled += ctx => lateralMovementInput = Vector2.zero;
        playerControls.std.Jump.performed += ctx => Jump();
    }

    private void DetectGround()
    {
        isGrounded = Physics.CheckSphere(groundSphere.position, groundCheckRadius, groundLayer);
    }

    private void ApplyDrag()
    {
        if (isGrounded)
        {
            rigidbody.drag = groundDrag;
        }
        else
        {
            rigidbody.drag = 0;
        }
    }

    private void SpeedControl()
    {
        // Retireves current velocity
        Vector3 flatVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        // Produces coefficient for movementspeed if airborn
        float speedMultiplier = 1;
        if (!isGrounded)
        {
            speedMultiplier = airMultiplier;
        }

        // Limit velocity
        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed * speedMultiplier;
            rigidbody.velocity = new Vector3(limitedVelocity.x, rigidbody.velocity.y, limitedVelocity.z);
        }
    }

    private void HandleMovement()
    {
        Vector3 movementDirection = new Vector3(lateralMovementInput.x, 0f, lateralMovementInput.y).normalized;
        Vector3 calculatedMoveVector = playerOrientation.TransformDirection(movementDirection) * movementSpeed * speedCoefficient;


        rigidbody.AddForce(calculatedMoveVector, ForceMode.Force);
    }

    private void Jump()
    {
        if (isGrounded && readyToJump)
        {
            // Zero out y velocity for consistent jumps
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

            // Add force on y-axis
            rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            readyToJump = false;
        }

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
