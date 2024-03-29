using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisableMeshRendererForOwner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

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
