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
    public float airMultiplier; // Increases move speed in air
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
    private Rigidbody rb;
    private Vector2 lateralMovementInput;

    private PlayerControls playerControls;

    private void Awake()
    {
        InitializeComponents();
        InitializeMouseLook();
        Debug.Log("Moving to sub");
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
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerControls = new PlayerControls();

        //DEBUGGING
        foreach (InputBinding ib in playerControls.bindings)
        {
            Debug.Log(ib.action + " " + ib.name);
        }
        //END DEBUGGING

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
        Debug.Log("Subbing...");
        playerControls.std.Move.performed += ctx => lateralMovementInput = ctx.ReadValue<Vector2>();
        playerControls.std.Move.performed += ctx => Debug.Log("Move!");
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
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void SpeedControl()
    {
        // Retrieves current velocity
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Produces coefficient for movement speed if airborne
        float speedMultiplier = 1;
        if (!isGrounded)
        {
            speedMultiplier = airMultiplier;
        }

        // Limit velocity
        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed * speedMultiplier;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void HandleMovement()
    {
        Vector3 movementDirection = new Vector3(lateralMovementInput.x, 0f, lateralMovementInput.y).normalized;
        Vector3 calculatedMoveVector = movementSpeed * speedCoefficient * playerOrientation.TransformDirection(movementDirection);


        rb.AddForce(calculatedMoveVector, ForceMode.Force);
    }

    private void Jump()
    {
        if (isGrounded && readyToJump)
        {
            // Zero out y velocity for consistent jumps
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // Add force on y-axis
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            readyToJump = false;
        }

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}