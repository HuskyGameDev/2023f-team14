using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public sealed class NetworkProjectile : NetworkBehaviour
{
    public ulong ownerId = 0;
}

public interface IProjectile
{
    public abstract void Launch(Vector3 forward);
}
