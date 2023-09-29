using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;
    public float gravity = -9.8f;

    [Header("Ground Settings")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.4f;

    private bool isGrounded;
    private Vector3 velocity;
    private Rigidbody rb;
    private Vector2 moveInput;
    private PlayerControls controls;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        ApplyGravity();
        Move();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -0.5f;
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
            rb.MovePosition(transform.position + velocity * Time.deltaTime);
        }
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        moveDirection = transform.TransformDirection(moveDirection);
        Vector3 moveVelocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
