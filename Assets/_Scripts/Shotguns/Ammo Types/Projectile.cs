using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkProjectile : NetworkBehaviour
{
    public ulong ownerId = 0;
}

public abstract class Projectile
{
    public abstract void Launch(Vector3 forward);
}
