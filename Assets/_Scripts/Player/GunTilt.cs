using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunTilt : MonoBehaviour
{
    [SerializeField]
    private Transform playerOrientation;
    [SerializeField]
    private TimeSpan slerpTime;
    [SerializeField]
    private Vector3 maxRotationAngles;
    [SerializeField]
    [Range(0f, 1f)]
    private float slerpStep;
    /// <summary>
    /// X is vertical rotation (gun barrel moves up for positive values, down for negative values).
    /// y is horizontal rotation (gun barrel moves right for positive values, left for negative values).
    /// z is Tilt (top of gun rolls right for positive, left for negative).
    /// </summary>
    private Vector3 initialRotation, pullVector, res, steps = Vector3.zero;
    private bool noMove = true;

    void Start()
    {
        initialRotation = transform.localEulerAngles;
        Debug.Log("Initial: " + initialRotation);
        pullVector = initialRotation + maxRotationAngles;
    }
    public void MoveTilt(PlayerMovementState state)
    {
        if (state.movement.Equals(Vector3.zero))
        {
            noMove = true;
            return;
        }
        noMove = false;

        float dot = Vector3.Dot(state.movement, playerOrientation.right);
        if (Mathf.Abs(dot) < 0.01f)
        {
            dot = 0;
            StepToward(Dimension.Z, 0);
        }
        if (dot > 0) StepToward(Dimension.Z, 1);
        else if (dot < 0) StepToward(Dimension.Z, -1);
        //Debug.Log("Dot product of " + state.movement + " and " + playerOrientation.right + ": " + Vector3.Dot(state.movement, playerOrientation.right));
    }

    private void CameraTilt()
    {

    }

    private void StepToward(Dimension dim, float value)
    {
        if (Mathf.Abs(value - steps[(int)dim]) < slerpStep) { steps[(int)dim] = value; return; }
        if (steps[(int)dim] < value)
        {
            steps[(int)dim] += slerpStep;
            if (steps[(int)dim] > 1) steps[(int)dim] = 1;
            return;
        }
        if (steps[(int)dim] > value)
        {
            steps[(int)dim] -= slerpStep;
            if (steps[(int)dim] < -1) steps[(int)dim] = -1;
            return;
        }
        steps[(int)dim] = value;

    }

    public void Update()
    {
        CameraTilt();

        if (noMove) StepToward(Dimension.Z, 0);

        // Calculate the angle between the two vectors
        float angle = Vector3.Angle(initialRotation, pullVector);
        Vector3 dest = pullVector;

        // If the angle is greater than 180 degrees, negate one of the vectors
        if (angle > 180f)
        {
            dest = -dest;
            angle = 360f - angle;
        }

        // Slerp between the adjusted vectors
        res = Vector3.SlerpUnclamped(initialRotation, dest, steps.z);
        transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.Euler(res));
    }

    private enum Dimension
    {
        X = 0,
        Y = 1,
        Z = 2
    }
}