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
        pc = GetComponent<PlayerCharacter>();
        myCam = pc.GetComponent<Camera>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnShoot()
    {
        if (!IsOwner) return;
        shotgun.FireServerRpc(myCam.transform.position, myCam.transform.forward, myCam.transform.right, myCam.transform.up);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
