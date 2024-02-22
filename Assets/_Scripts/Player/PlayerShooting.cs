using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : NetworkBehaviour
{
    public Shotgun Shotgun;
    public ShotgunViewmodel Viewmodel;
    private Camera myCam;
    private PlayerCharacter pc;
    private PlayerInput playerInput;
    private void Awake()
    {
        pc = GetComponent<PlayerCharacter>();
        myCam = pc.GetComponentInChildren<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public override void OnNetworkSpawn()
    {
        playerInput = GetComponent<PlayerInput>();
        base.OnNetworkSpawn();
        if (IsServer)
            pc.OnSpawn += Shotgun.OnPlayerSpawn;
        if (IsOwner)
            playerInput.actions["Shoot"].performed += OnShoot;
        //shotgun = Instantiate(shotgunPrefab);
        //shotgun.GetComponent<NetworkObject>().Spawn();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (IsServer)
            pc.OnSpawn -= Shotgun.OnPlayerSpawn;
        if (IsOwner)
            playerInput.actions["Shoot"].performed -= OnShoot;
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) throw new MethodAccessException("Only the owner can call this method!");

        if (!Shotgun) throw new NullReferenceException("No Shotgun found!");
        Shotgun.FireServerRpc(myCam.transform.position, myCam.transform.forward, myCam.transform.right, myCam.transform.up);
        Viewmodel.Shoot();
        Recoil();
    }

    private void Recoil()
    {
        //TODO? 
    }
}
