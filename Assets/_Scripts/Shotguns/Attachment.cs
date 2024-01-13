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
    /// <summary>
    /// This is called whenever this attachment is attached to a shotgun.
    /// </summary>
    /// <param name="shotgun">The shotgun attached to</param>
    public void AttachTo(Shotgun shotgun);
    /// <summary>
    /// This is called whenever this attachment is detached from a shotgun.
    /// </summary>
    /// <param name="shotgun">The shotgun detached from</param>
    public void DetachFrom(Shotgun shotgun);
}
