using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class ProjectileAmmoType : AmmoType
{
    /// <summary>
    /// The pellet to be spawned when this ammo type is fired.
    /// </summary>
    public Projectile pelletPrefab;
}
