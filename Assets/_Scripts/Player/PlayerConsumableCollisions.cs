using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerConsumableCollisions : Unity.Netcode.NetworkBehaviour
{
    public TMP_Text ammo;
    int ammoAsInteger = 0;

    void OnTriggerEnter(Collider consumable)
    {
        if (!IsOwner) return;
        if (consumable.gameObject.tag == "AmmoBox")
        {
            Destroy(consumable.gameObject);
            AmmoConsumptionServerRpc(OwnerClientId);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Find the Canvas that contains the "ammo" Text UI element
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            // Find the "ammo" Text UI element within the Canvas
            ammo = canvas.transform.Find("AmmoNumericValue").GetComponent<TMP_Text>();

            try
            {
                //try to convert ammo (as a string) to an integer
                ammoAsInteger = int.Parse(ammo.text);
            }
            catch (FormatException e)
            {
                Debug.LogError("Error: " + e.Message);
            }

        }

    }

    [ServerRpc]
    private void AmmoConsumptionServerRpc(ulong playerID)
    {

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { playerID }
            }
        };
        AmmoConsumptionClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void AmmoConsumptionClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        if (ammo != null)
        {
            //acquire ammo
            ammoAsInteger += 10;
            ammo.text = ammoAsInteger.ToString();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
