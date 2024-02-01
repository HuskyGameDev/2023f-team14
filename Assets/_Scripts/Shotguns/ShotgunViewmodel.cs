using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunViewmodel : MonoBehaviour
{
    [SerializeField]
    private Shotgun shotgun;

    public void Shoot()
    {
        MuzzleFlash();
    }

    private void MuzzleFlash()
    {
    }

}
