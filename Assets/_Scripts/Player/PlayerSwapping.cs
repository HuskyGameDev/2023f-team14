using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwapping : NetworkBehaviour
{
    [SerializeField]
    private GameObject swapUIPrefab;
    private PlayerInput playerInput;
    [SerializeField]
    private MouseLook mouseLook;
    private SwapUI swapUI;
    private Shotgun shotgun;

    private void Start()
    {
        shotgun = GetComponent<PlayerShooting>().Shotgun;
    }

    private void OnSwapOpen(InputAction.CallbackContext ctx)
    {
        if (swapUI.IsOpen) return;
        swapUI.Open();
        mouseLook.LockCursor(false);
    }

    private void OnSwapClose(InputAction.CallbackContext ctx)
    {
        swapUI.Close();
        mouseLook.LockCursor(true);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions["Swap"].started += OnSwapOpen;
        playerInput.actions["Swap"].canceled += OnSwapClose;

        swapUI = Instantiate(swapUIPrefab).GetComponent<SwapUI>();
    }
}
