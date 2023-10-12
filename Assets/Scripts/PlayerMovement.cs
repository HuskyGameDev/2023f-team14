using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed;
    public float jumpStrength;
    public float groundDrag;

    [Header("Ground Check Settings")]
    public Transform groundSphere;
    public LayerMask groundLayer;
    public float groundCheckRadius;

    [Header("Miscellaneous")]
    public Camera playerCamera;
    public MouseLook mouseLook;

    private bool isGrounded;
    private float speedCoefficient = 10;
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
        Vector3 flatVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        // Limit velocity
        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
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
        if (isGrounded)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, Mathf.Sqrt(jumpStrength * -2f * Physics.gravity.y), rigidbody.velocity.z);
        }
    }
}
