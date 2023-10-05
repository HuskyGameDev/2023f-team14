using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float jumpForce = 5.0f;

    [Header("Ground Settings")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.4f;

    private bool isGrounded;
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
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
        Move();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

    private void Move()
    {
        // Calculate horizontal movement
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        moveDirection = transform.TransformDirection(move) * moveSpeed;

        // Set horizontal velocity
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y), rb.velocity.z);
        }
    }
}
