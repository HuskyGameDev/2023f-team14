using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;

    [Header("Gravity Settings")]
    public float gravity = 9.81f;
    public float collisionOffset = 0.5f;

    [Header("Ground Settings")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.4f;

    private bool isGrounded;
    private Vector3 moveDirection;
    private Vector2 moveInput;
    private float yVelocity = 0;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        var mouseLook = transform.GetComponentInChildren<MouseLook>();
        if (mouseLook)
        {
            mouseLook.Initialize(controls);
        }

        // Subscribe to actions
        controls.std.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.std.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.std.Jump.performed += ctx => Jump();
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
        Move();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        if (isGrounded && yVelocity < 0)
        {
            yVelocity = -1f;
        }
    }

    private void Move()
    {
        // Calculate horizontal movement
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        moveDirection = transform.TransformDirection(move) * moveSpeed;

        // Handle gravity
        yVelocity -= gravity * Time.deltaTime;

        // Apply movement
        Vector3 velocity = moveDirection + Vector3.up * yVelocity;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        // Check for forward collisions before moving
        if (Physics.Raycast(transform.position, moveDirection, moveSpeed * Time.deltaTime + collisionOffset))
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        transform.position += velocity * Time.deltaTime;
    }

    private void Jump()
    {
        if (isGrounded)
        {
            yVelocity = Mathf.Sqrt(jumpForce * 2f * gravity);
        }
    }
}
