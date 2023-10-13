using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace PumpingAction.GameModes.CTF
{
    public class CTF_Flag : NetworkBehaviour
    {
        private bool initialized;
        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        private void Awake()
        {
            initialized = false;
        }

        private void Initialize()
        {
            if (!initialized)
            {
                this.transform.position = Vector3.zero;
                initialized = true;
            }
        }

        public override void OnNetworkSpawn()
        {
            Initialize();
            base.OnNetworkSpawn();
        }
    }
}
