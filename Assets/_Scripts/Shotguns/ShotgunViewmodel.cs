using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunViewmodel : MonoBehaviour
{
    [SerializeField]
    private Shotgun shotgun;
    [SerializeField]
    private CameraRecoil playerCamera;

    void Awake()
    {
        shotgun.OnFire += Shoot;
    }

    void OnDestroy()
    {
        shotgun.OnFire -= Shoot;
    }

    public void Shoot()
    {
        MuzzleFlash();
        Recoil();
    }

    private void MuzzleFlash()
    {

    }

    private void Recoil()
    {
        playerCamera.Recoil();
    }

}
