using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : Unity.Netcode.NetworkBehaviour
{
    public Shotgun shotgun;
    private Camera myCam;
    private PlayerCharacter pc;
    private void Awake()
    {
        myCam = GetComponentInChildren<Camera>();
        pc = GetComponent<PlayerCharacter>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnShoot()
    {
        if (!IsOwner || !IsClient || !IsHost) return;

        shotgun.FireServerRpc();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
