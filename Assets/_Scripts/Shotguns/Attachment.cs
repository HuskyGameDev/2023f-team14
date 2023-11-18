using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AttachmentID : uint
{
    Barrel_Stock,
    Underbarrel_Stock,
    Accessory_Stock,
    AmmoType_Stock,
}

public interface IAttachment
{
    public void AttachTo(Shotgun shotgun);
    public void DetachFrom(Shotgun shotgun);
}
