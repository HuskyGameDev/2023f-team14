using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Attachment
{
    public void AttachTo(Shotgun shotgun);
    public void DetachFrom(Shotgun shotgun);
}
