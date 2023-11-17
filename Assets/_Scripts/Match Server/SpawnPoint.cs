using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private int numEnemiesPresent;
    /// <summary>
    /// A mask for every Team this point can spawn. An "enemy" is any player not covered by this mask.
    /// </summary>
    public ushort teamTagMask = 0;

    void Awake()
    {
        numEnemiesPresent = 0;
    }

    public bool IsEnemyPresent => numEnemiesPresent > 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player")) return;

        var pc = other.GetComponent<PlayerCharacter>();
        pc.OnDeath += Leave;
        numEnemiesPresent += (teamTagMask & (ushort)pc.team.Value) > 0 ? 0 : 1;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.tag.Equals("Player")) return;

        var pc = other.GetComponent<PlayerCharacter>();
        Leave(pc);
    }

    private void Leave(PlayerCharacter pc)
    {
        numEnemiesPresent -= (teamTagMask & (ushort)pc.team.Value) > 0 ? 0 : 1;
        pc.OnDeath -= Leave;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = TeamUtils.GetTeamColor((Team)teamTagMask);
        Gizmos.DrawSphere(transform.position, 6);
    }
}
