using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerModel : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner && !IsServer) return;

        var list = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (var l in list)
        {
            l.enabled = false;
        }
        var c = gameObject.GetComponent<MeshRenderer>();
        if (c)
        {
            c.enabled = false;
        }
    }
}
