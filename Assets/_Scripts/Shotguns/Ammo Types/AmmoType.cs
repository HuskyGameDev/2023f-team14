using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class AmmoType : MonoBehaviour, Attachment
{
    public int numPellets;
    public FireMode fireMode;
    public NetworkObject pelletPrefab;

    public abstract void AttachTo(Shotgun shotgun);
    public abstract void DetachFrom(Shotgun shotgun);
}

public enum FireMode
{
    Projectile,
    Hitscan
}
