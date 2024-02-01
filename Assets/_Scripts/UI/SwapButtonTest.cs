using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapButtonTest : MonoBehaviour
{
    public Shotgun s;
    public void Swap()
    {
        s.SwapToServerRpc(AttachmentID.AmmoType_Fireball);
    }

}
