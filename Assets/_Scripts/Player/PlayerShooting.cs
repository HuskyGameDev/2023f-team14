using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : Unity.Netcode.NetworkBehaviour
{
    public Shotgun shotgun;
    private Camera myCam;
    private PlayerCharacter pc;
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
        base.OnNetworkSpawn();
        //shotgun = Instantiate(shotgunPrefab);
        //shotgun.GetComponent<NetworkObject>().Spawn();
    }

    public void OnShoot()
    {
        if (!IsOwner) return;

        if (!shotgun) return;
        shotgun.FireServerRpc(myCam.transform.position, myCam.transform.forward, myCam.transform.right, myCam.transform.up);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
