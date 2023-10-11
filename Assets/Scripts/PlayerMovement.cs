using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Movement parameters
    [Header("Movement Settings")]
    public float walkingSpeed = 25.0f;
    public float jumpPower = 7.0f;
    public float maxForce = 100;

    // Ground detection parameters
    [Header("Ground Detection Settings")]
    public Transform groundDetectionPoint; // The point from where we'll check for ground
    public LayerMask groundLayerMask;      // Which layer represents the ground
    public float groundCheckRadius = 0.4f; // Radius of the ground check sphere

    // Other parameters
    [Header("Miscellaneous Settings")]
    public float knockbackForce = 50.0f;
    public Camera playerCamera;

    private bool isGrounded;
    private Rigidbody rb;
    private Vector2 movementInputVector;
    private PlayerControls playerControls;

    private void Awake()
    {
        // Initializing components
        rb = GetComponent<Rigidbody>();
        playerControls = new PlayerControls();

        // Try to get the MouseLook script from child components and initialize it
        var mouseLookComponent = transform.GetComponentInChildren<MouseLook>();
        if (mouseLookComponent){ mouseLookComponent.Initialize(playerControls); }

        // Subscribe to input actions
        playerControls.std.Move.performed += ctx => movementInputVector = ctx.ReadValue<Vector2>();
        playerControls.std.Move.canceled += ctx => movementInputVector = Vector2.zero;
        playerControls.std.Jump.performed += ctx => Jump();
        playerControls.std.Shoot.performed += ctx => KnockbackShot();
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
        DetectGroundStatus();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void DetectGroundStatus()
    {
        // Check if player is touching ground using a sphere detection
        isGrounded = Physics.CheckSphere(groundDetectionPoint.position, groundCheckRadius, groundLayerMask);
    }

    private void HandleMovement()
    {
        // Calculate horizontal movement vector based on user input
        Vector3 rawMovementVector = new Vector3(movementInputVector.x, 0f, movementInputVector.y).normalized;

        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = transform.TransformDirection(rawMovementVector) * walkingSpeed;

        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);

        // Update player's velocity based on movement direction
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Jump()
    {
        // Execute jump if the player is grounded
        if (isGrounded)
        {
            Vector3 jumpVelocity = new Vector3(rb.velocity.x, Mathf.Sqrt(jumpPower * -2f * Physics.gravity.y), rb.velocity.z);
            rb.AddForce(jumpVelocity, ForceMode.Impulse);
        }
    }

    private void KnockbackShot()
    {
        /*if (!isGrounded)
        {
            Vector3 recoilDirection = -playerCamera.transform.forward;
            rb.AddForce(recoilDirection * knockbackForce, ForceMode.Impulse);
        }*/
    }
}
