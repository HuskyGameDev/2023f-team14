using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Underbarrel : MonoBehaviour, Attachment
{
    public abstract void AttachTo(Shotgun shotgun);
    public abstract void DetachFrom(Shotgun shotgun);
}