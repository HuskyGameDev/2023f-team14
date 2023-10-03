using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class GameMode
{
    public int maxTeamSize { get; protected set; }
    public int minNumTeams { get; protected set; }

}
