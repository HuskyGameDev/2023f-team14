using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlyActiveForOwner : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        gameObject.SetActive(IsOwner);
        if (!IsOwner) return;
    }
}
