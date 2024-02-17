using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPredictionInputProvider : MonoBehaviour, PRN.IInputProvider<PlayerMovementInput>
{
    [SerializeField]
    private PlayerControlsMonoBehaviour[] monoScripts;
    [SerializeField]
    private PlayerControlsNetworkBehaviour[] networkScripts;
    public readonly Dictionary<Type, IRequirePlayerControls> externalScripts = new();
    private PlayerMovementInput input;


    private PlayerControls playerControls;
    private bool pendingJump = false;

    void Awake()
    {

        InitializeComponents();
        SubscribeToInputEvents();
        InitializeExternalScripts();
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

    private void InitializeExternalScripts()
    {
        foreach (var script in monoScripts)
        {
            script.Initialize(playerControls);
            externalScripts.Add(script.GetType(), script);
        }
        foreach (var script in networkScripts)
        {
            script.Initialize(playerControls);
            externalScripts.Add(script.GetType(), script);
        }
    }

    public PlayerMovementInput GetInput()
    {
        input.jump = pendingJump;
        if (externalScripts.TryGetValue(typeof(MouseLook), out IRequirePlayerControls t))
        {
            input.forward = ((MouseLook)t).Forward;
            input.right = ((MouseLook)t).Right;
        }
        else Debug.LogError("Could not access MouseLook component!");
        pendingJump = false;
        return input;
    }
}

public interface IRequirePlayerControls
{
    public void Initialize(PlayerControls pc);
}

public abstract class PlayerControlsMonoBehaviour : MonoBehaviour, IRequirePlayerControls
{
    public abstract void Initialize(PlayerControls pc);
}

public abstract class PlayerControlsNetworkBehaviour : Unity.Netcode.NetworkBehaviour, IRequirePlayerControls
{
    public abstract void Initialize(PlayerControls pc);
}