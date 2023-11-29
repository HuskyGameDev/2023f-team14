using System;
using System.Collections;
using System.Collections.Generic;
using PRN;

//using System.Numerics;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : Unity.Netcode.NetworkBehaviour
{

    [SerializeField]
    private PlayerPredictionProcessor processor;
    [SerializeField]
    private PlayerPredictionInputProvider inputProvider;
    [SerializeField]
    private PlayerPredictionStateConsistencyChecker consistencyChecker;


    private CharacterController controller;
    private Ticker ticker;
    private NetworkHandler<PlayerMovementInput, PlayerMovementState> networkHandler;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ticker = new(TimeSpan.FromSeconds(1 / (float)NetworkManager.Singleton.NetworkTickSystem.TickRate));
        NetworkRole role;

        if (IsServer)
            role = IsOwner ? NetworkRole.HOST : NetworkRole.SERVER;
        else
            role = IsOwner ? NetworkRole.OWNER : NetworkRole.GUEST;

        networkHandler = new(
            role: role,
            ticker: ticker,
            processor: processor,
            inputProvider: inputProvider,
            consistencyChecker: consistencyChecker
        );

        networkHandler.onSendStateToClient += SendStateClientRpc;
        networkHandler.onSendInputToServer += SendInputServerRpc;
    }

    private void FixedUpdate()
    {
        ticker.OnTimePassed(TimeSpan.FromSeconds(Time.fixedDeltaTime));
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        networkHandler.Dispose();
    }

    [ServerRpc]
    private void SendInputServerRpc(PlayerMovementInput input)
    {
        networkHandler.OnOwnerInputReceived(input);
    }

    [ClientRpc]
    public void SendStateClientRpc(PlayerMovementState state)
    {
        networkHandler.OnServerStateReceived(state);
    }

    //SERVER ONLY
    public void SetPosition(Vector3 pos)
    {
        if (!IsServer) return;
        controller.enabled = false;
        controller.transform.position = pos;
        controller.enabled = true;
    }
}


