using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Movement parameters
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float shootForce = 100.0f;
    public float shootDuration = 0.2f;

    // Ground checking parameters
    [Header("Ground Settings")]
    public Transform groundCheck; // The point from where we'll check for ground
    public LayerMask groundLayer; // Which layer represents the ground
    public float groundDistance = 0.4f; // Radius of the ground check sphere

    private bool isGrounded;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private PlayerControls controls;

    private void Awake()
    {
        // Initializing components
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        // Try to get the MouseLook script from child components and initialize it
        var mouseLook = transform.GetComponentInChildren<MouseLook>();
        if (mouseLook)
        {
            mouseLook.Initialize(controls);
        }

        // Subscribe to inputs
        controls.std.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // Lateral movement
        controls.std.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.std.Jump.performed += ctx => Jump(); // Jumping
        controls.std.Shoot.performed += ctx => Shoot();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        CheckGround();
    }

    private void FixedUpdate()
    {
        // Handle player movement in fixed intervals for physics consistency
        Move();
    }

    private void CheckGround()
    {
        // Perform a sphere check to see if player is touching ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

    private void Move()
    {
        // Calculate player's horizontal movement based on input
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        moveDirection = transform.TransformDirection(move) * moveSpeed;

        // Set the velocity of the player
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    private void Jump()
    {
        // Allow the player to jump if they are on the ground
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), rb.velocity.z);
        }
    }

    private void Shoot()
    {
        if (!isGrounded)
        {
            Vector3 moveDirection = transform.forward;

            rb.AddForce(-moveDirection * shootForce, ForceMode.Impulse);
        }
    }
}
