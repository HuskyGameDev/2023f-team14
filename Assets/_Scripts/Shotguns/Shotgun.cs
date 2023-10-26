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
    private Ray[] gizmos;

    //TODO Finish implementing vv
    //run calcs using ammo and  other attachments
    private float Damage => 10;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    //TODO Spawn projectiles + hitscan from gun barrel.
    [ServerRpc]
    public void FireServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var client = NetworkManager.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerCharacter>();
        Camera clientCam = client.camera;
        Vector2[] spread = barrel.GetScreenPointSpread(ammoType.numPellets);

        gizmos = new Ray[spread.Length];
        for (int i = 0; i < spread.Length; i++)
        {
            gizmos[i] = clientCam.ScreenPointToRay(new Vector2(spread[i].x + (clientCam.pixelWidth / 2), spread[i].y + (clientCam.pixelHeight / 2)));
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
                    NetworkObject pellet = Instantiate<NetworkObject>(ammoType.pelletPrefab, gizmos[i].origin, Quaternion.Euler(gizmos[i].direction));
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
                    if (Physics.Raycast(gizmos[i], out RaycastHit hit, playerMask))
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
        if (gizmos == null) return;
        Gizmos.color = Color.red;
        for (int i = 0; i < gizmos.Length; i++)
        {
            Gizmos.DrawLine(gizmos[i].origin, gizmos[i].direction * 100);
        }
    }
}
