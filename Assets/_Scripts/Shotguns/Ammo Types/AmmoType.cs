using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class AmmoType : MonoBehaviour, IAttachment
{
    /// <summary>
    /// The number of pellets to fire at a time.
    /// </summary>
    public uint numPellets;
    /// <summary>
    /// How much ammo the player can hold at any given time
    /// </summary>
    public uint MaxAmmo;
    public GameObject HitSpritePrefab;

    public abstract AttachmentID ID { get; }

    public abstract void AttachTo(Shotgun shotgun);
    public abstract void DetachFrom(Shotgun shotgun);
}
