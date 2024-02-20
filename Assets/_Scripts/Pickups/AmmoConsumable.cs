using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class AmmoConsumable : NetworkBehaviour, IPickup
{
    [SerializeField]
    private uint ammoSupply;

    public void PickUp(PlayerCharacter pc)
    {
        pc.gameObject.GetComponent<PlayerShooting>().Shotgun.AddAmmo(ammoSupply);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) PickUp(other.GetComponent<PlayerCharacter>());
    }
}
