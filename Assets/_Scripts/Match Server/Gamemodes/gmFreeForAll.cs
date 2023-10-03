using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace PumpingAction.GameModes.FreeForAll
{
    public class gmFreeForAll : GameMode
    {
        public gmFreeForAll()
        {
            maxTeamSize = 1;
            minNumTeams = 1;
        }
    }
}
