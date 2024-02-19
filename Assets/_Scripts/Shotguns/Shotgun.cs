using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Shotgun : NetworkBehaviour
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

    private readonly Dictionary<AttachmentID, IAttachment> availableAttachments = new();
    private Ray[] pelletRays;
    private float MaxHitDistance => 45;

    //TODO Finish implementing vv
    //run calcs using ammo and  other attachments
    private float Damage => 10;

    public Action OnFire;
    private AttachmentDepot depot;


    // Start is called before the first frame update
    private void Start()
    {
        depot = new();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    [SerializeField]
    private AmmoType extra;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        availableAttachments.Add(barrel.ID, barrel);
        availableAttachments.Add(underbarrel.ID, underbarrel);
        availableAttachments.Add(accessory.ID, accessory);
        availableAttachments.Add(ammoType.ID, ammoType);

        //! TESTING PURPOSES ONLY -- REMOVE BEFORE RELEASE
        availableAttachments.Add(AttachmentID.AmmoType_Fireball, extra);
    }


    //TODO Render projectiles + hitscan from gun barrel.
    /// <summary>
    /// Fire's a player's shotgun.
    /// </summary>
    /// <param name="pos">the position of the player's camera</param>
    /// <param name="forward">the player's forward vector</param>
    /// <param name="right">the player's right vector</param>
    /// <param name="up">the player's up vector</param>
    [ServerRpc]
    public void FireServerRpc(Vector3 pos, Vector3 forward, Vector3 right, Vector3 up, ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != OwnerClientId) return;

        PlayerCharacter client = NetworkManager.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject.GetComponent<PlayerCharacter>();
        Camera clientCam = client.camera;
        Vector2[] spread = barrel.GetPelletSpread(ammoType.numPellets);
        Vector3 pelletDir;
        PlayerCharacter pc;


        /* Calculates the rays that the pellet spread creates */
        pelletRays = new Ray[spread.Length];
        for (int i = 0; i < spread.Length; i++)
        {
            pelletDir = Vector3.RotateTowards(forward, right * Mathf.Sign(spread[i].x), Mathf.Abs(spread[i].x), 0f);
            pelletDir = Vector3.RotateTowards(pelletDir, up * Mathf.Sign(spread[i].y), Mathf.Abs(spread[i].y), 0f);
            pelletRays[i] = new Ray(pos, pelletDir);
        }

        /* If the ammo type is projectile, launch the projectiles */
        if (ammoType is ProjectileAmmoType ammo)
        {
            NetworkObject pellet;
            for (int i = 0; i < spread.Length; i++)
            {
                pellet = NetworkObjectPool.Instance.GetObject(((ProjectileAmmoType)ammoType).pelletPrefab.gameObject, modelBarrelEnd.position, Quaternion.Euler(pelletRays[i].direction));
                pellet.GetComponent<IProjectile>().Launch(pelletRays[i].direction);
            }
        }
        /* If the ammo type is hitscan, perform raycasts */
        else if (ammoType is HitscanAmmoType)
        {
            for (int i = 0; i < pelletRays.Length; i++)
            {
                //Raycast each pellet
                if (Physics.Raycast(pelletRays[i], out RaycastHit hit, LayerMask.NameToLayer("Player")))
                {
                    SpawnPelletTrailClientRpc(modelBarrelEnd.position, hit.point);
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        /* Render pellet hit sprites */
                        RenderHitSpriteClientRpc(hit.point, hit.normal);
                    }
                    if (!hit.transform.gameObject.CompareTag("Player")) continue;
                    pc = hit.transform.gameObject.GetComponent<PlayerCharacter>();

                    //Hit!
                    if (pc.team.Value != client.team.Value || pc.team.Value == Team.NoTeam && client.team.Value == Team.NoTeam)
                        pc.Hit(serverRpcParams.Receive.SenderClientId, Damage);
                }
                //SpawnPelletTrailClientRpc(modelBarrelEnd.position, pelletRays[i].origin + pelletRays[i].direction * MaxHitDistance);
            }
        }
        /* Shotgun fire callback */
        OnFire?.Invoke();
    }

    [ClientRpc]
    private void RenderHitSpriteClientRpc(Vector3 hitPosition, Vector3 surfaceNormal, ClientRpcParams clientRpcParams = default)
    {
        var go = GameObjectPool.Instance.GetObject(ammoType.HitSpritePrefab
            , hitPosition + Vector3.ClampMagnitude(surfaceNormal, 0.02f)
            , Quaternion.FromToRotation(Vector3.forward, surfaceNormal));
        go.GetComponent<PelletHoleSprite>().SetReleaseTimeout(new TimeSpan(0, 0, 20), ammoType.HitSpritePrefab);
    }

    /// <summary>
    /// Spawns a TrailRenderer between two coordinates.
    /// </summary>
    /// <param name="origin">The position to render the trail from</param>
    /// <param name="endpoint">The position to render the trail to</param>
    [ClientRpc]
    private void SpawnPelletTrailClientRpc(Vector3 origin, Vector3 endpoint)
    {
        //TODO: Pool these for the love of god
        GameObject bulletTrailEffect = Instantiate(((HitscanAmmoType)ammoType).pelletTrail, origin, Quaternion.identity);
        LineRenderer lr = bulletTrailEffect.GetComponent<LineRenderer>();

        lr.SetPosition(0, origin);
        lr.SetPosition(1, endpoint);

        Destroy(bulletTrailEffect, 0.2f);
    }

    /// <summary>
    /// Swaps an attachment onto this shotgun, detaching the previous one.
    /// </summary>
    /// <param name="id">The attachment to swap to</param>
    [ServerRpc]
    public void SwapToServerRpc(AttachmentID id, ServerRpcParams serverRpcParams = default)
    {
        Type t;
        if (!availableAttachments.TryGetValue(id, out IAttachment attachment))
        {
            Debug.LogError("Attachment " + id + " not found! Requested by player " + serverRpcParams.Receive.SenderClientId);
            return;
        }
        t = AttachmentDepot.GetTypeOfAttachment(id);

        switch (t)
        {
            case Type _ when t == typeof(Barrel):
                this.SwapTo(barrel, attachment);
                barrel = attachment as Barrel;
                break;
            case Type _ when t == typeof(Underbarrel):
                this.SwapTo(underbarrel, attachment);
                underbarrel = attachment as Underbarrel;
                break;
            case Type _ when t == typeof(AmmoType):
                this.SwapTo(ammoType, attachment);
                ammoType = attachment as AmmoType;
                break;
            case Type _ when t == typeof(Accessory):
                this.SwapTo(accessory, attachment);
                accessory = attachment as Accessory;
                break;
            default:
                Debug.LogError("Unknown attachment type of " + t.GetType() + "!");
                break;
        }
    }
}
