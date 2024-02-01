using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoType_Stock : HitscanAmmoType
{
    public override AttachmentID ID => AttachmentID.AmmoType_Stock;

    public override void AttachTo(Shotgun shotgun)
    {
    }

    public override void DetachFrom(Shotgun shotgun)
    {
    }
}
