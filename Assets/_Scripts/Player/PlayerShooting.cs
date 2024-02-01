using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : Unity.Netcode.NetworkBehaviour
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
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Shoot"].performed += OnShoot;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //shotgun = Instantiate(shotgunPrefab);
        //shotgun.GetComponent<NetworkObject>().Spawn();
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;

        if (!Shotgun) return;
        Shotgun.FireServerRpc(myCam.transform.position, myCam.transform.forward, myCam.transform.right, myCam.transform.up);
        Viewmodel.Shoot();
        Recoil();
    }

    private void Recoil()
    {
        //TODO? 
    }
}
