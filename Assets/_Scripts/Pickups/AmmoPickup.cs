using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class AmmoPickup : NetworkPickupRespawnable
{
    [SerializeField]
    private uint ammoSupply;

    public override void PickUp(PlayerCharacter pc)
    {
        pc.gameObject.GetComponent<PlayerShooting>().Shotgun.AddAmmo(ammoSupply);
    }
}
