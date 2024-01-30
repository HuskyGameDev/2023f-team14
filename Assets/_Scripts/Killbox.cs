using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Killbox : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer) Destroy(this);
        base.OnNetworkSpawn();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerCharacter>().Kill();
        }
    }
}
