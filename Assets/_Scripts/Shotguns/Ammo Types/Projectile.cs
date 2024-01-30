using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Projectile : NetworkBehaviour
{
    public ulong ownerId = 0;
    public abstract void Launch(Vector3 forward);
}
