using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPredictionStateConsistencyChecker : MonoBehaviour, PRN.IStateConsistencyChecker<PlayerMovementState>
{
    public bool IsConsistent(PlayerMovementState serverState, PlayerMovementState ownerState) =>
        Vector3.Distance(serverState.position, ownerState.position) <= 0.01f
            && Vector3.Distance(serverState.movement, ownerState.movement) <= 0.01f
            && Vector3.Distance(serverState.gravity, ownerState.gravity) <= 0.01f;
}
