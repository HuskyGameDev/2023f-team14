using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoType_Fireball : ProjectileAmmoType
{
    public override AttachmentID ID => AttachmentID.AmmoType_Fireball;

    public override void AttachTo(Shotgun shotgun)
    {
    }

    public override void DetachFrom(Shotgun shotgun)
    {
        //throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }
}
