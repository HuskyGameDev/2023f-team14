using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shotgun : Unity.Netcode.NetworkBehaviour
{
    [Header("Attachments")]
    [SerializeField]
    private Barrel barrel;
    [SerializeField]
    private Underbarrel underbarrel;
    [SerializeField]
    private Accessory accessory;
    [SerializeField]
    private AmmoType ammoType;
    [Header("Miscellaneous")]
    [SerializeField]
    private LayerMask playerMask;
    [SerializeField]
    private Transform modelBarrelEnd;
    private Ray[] pelletRays;
    private float MaxHitDistance => 45;

    //TODO Finish implementing vv
    //run calcs using ammo and  other attachments
    private float Damage => 10;


    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }


    //TODO Spawn projectiles + hitscan from gun barrel.
    [ServerRpc]
    public void FireServerRpc(Vector3 pos, Vector3 forward, Vector3 right, Vector3 up, ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != OwnerClientId) return;

        var client = NetworkManager.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerCharacter>();
        Camera clientCam = client.camera;
        Vector2[] spread = barrel.GetPelletSpread(ammoType.numPellets);
        Vector3 pelletDir;


        /* Calculates the rays that the pellet spread creates */
        pelletRays = new Ray[spread.Length];
        for (int i = 0; i < spread.Length; i++)
        {
            pelletDir = Vector3.RotateTowards(forward, right * Mathf.Sign(spread[i].x), Mathf.Abs(spread[i].x), 0f);
            pelletDir = Vector3.RotateTowards(pelletDir, up * Mathf.Sign(spread[i].y), Mathf.Abs(spread[i].y), 0f);
            //pelletRays[i] = new Ray(modelBarrelEnd.position, pelletDir);
            pelletRays[i] = new Ray(pos, pelletDir);
        }

        PlayerCharacter pc;
        switch (ammoType.fireMode)
        {
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            case FireMode.Projectile:
                for (int i = 0; i < spread.Length; i++)
                {
                    NetworkObject pellet = Instantiate<NetworkObject>(ammoType.pelletPrefab, pelletRays[i].origin, Quaternion.Euler(pelletRays[i].direction));
                    pellet.SpawnWithOwnership(OwnerClientId);
                }
                break;


            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            case FireMode.Hitscan:
                for (int i = 0; i < spread.Length; i++)
                {

                    //Actually do a raycast.
                    if (Physics.Raycast(pelletRays[i], out RaycastHit hit, playerMask))
                    {
                        pc = hit.transform.gameObject.GetComponent<PlayerCharacter>();

                        //Hit!
                        if (pc.team.Value != client.team.Value)
                            pc.Hit(OwnerClientId, Damage);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (pelletRays == null || !IsServer) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < pelletRays.Length; i++)
        {
            Gizmos.DrawRay(pelletRays[i].origin, pelletRays[i].direction * MaxHitDistance);
        }
    }
}
