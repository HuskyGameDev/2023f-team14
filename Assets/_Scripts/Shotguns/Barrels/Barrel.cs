using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Barrel : MonoBehaviour, Attachment
{
    public abstract void AttachTo(Shotgun shotgun);
    public abstract void DetachFrom(Shotgun shotgun);
    public abstract Vector2[] GetScreenPointSpread(int numPellets);
}
