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


    [ServerRpc]
    public void FireServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var client = NetworkManager.ConnectedClients[OwnerClientId].PlayerObject.GetComponent<PlayerCharacter>();
        Camera clientCam = client.GetComponentInChildren<Camera>();
        Vector2[] spread = barrel.GetScreenPointSpread(ammoType.numPellets);
        Ray r;
        RaycastHit hit;
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
                    r = clientCam.ScreenPointToRay(spread[i]);
                    NetworkObject pellet = Instantiate<NetworkObject>(ammoType.pelletPrefab, r.origin, Quaternion.Euler(r.direction));
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
                    Gizmos.color = Color.red;
                    r = clientCam.ScreenPointToRay(spread[i]);
                    Gizmos.DrawRay(r);

                    //Actually do a raycast.
                    if (Physics.Raycast(r, out hit))
                    {
                        pc = hit.transform.gameObject.GetComponent<PlayerCharacter>();

                        //Hit!
                        if (pc.team.Value != pc.team.Value)
                            pc.Hit(OwnerClientId, Damage);
                    }
                }
                break;
            default:
                break;
        }
    }
}
