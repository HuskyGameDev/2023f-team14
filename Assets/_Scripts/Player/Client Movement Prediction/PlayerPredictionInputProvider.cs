using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPredictionInputProvider : MonoBehaviour, PRN.IInputProvider<PlayerMovementInput>
{
    [SerializeField]
    private MouseLook mouseLook;
    private PlayerMovementInput input;
    private PlayerControls playerControls;
    private bool pendingJump = false;

    void Awake()
    {
        InitializeComponents();
        SubscribeToInputEvents();
        InitializeMouseLook();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void SubscribeToInputEvents()
    {
        playerControls.std.Move.performed += ctx => input.lateralMovement = ctx.ReadValue<Vector2>();
        playerControls.std.Move.canceled += ctx => input.lateralMovement = Vector2.zero;
        playerControls.std.Jump.performed += ctx => pendingJump = true;
    }

    private void InitializeComponents()
    {
        playerControls = new PlayerControls();
        //playerCamera = transform.GetComponentInChildren<Camera>();
    }

    private void InitializeMouseLook()
    {
        if (mouseLook)
        {
            mouseLook.Initialize(playerControls);
            return;
        }
        Debug.LogError("No camera bound to player!");
    }

    public PlayerMovementInput GetInput()
    {
        input.jump = pendingJump;
        pendingJump = false;
        return input;
    }
}
