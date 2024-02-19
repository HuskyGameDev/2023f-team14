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

    /// <summary>
    /// Tilts the gun according to player movement.
    /// </summary>
    /// <param name="state">The movement state</param>
    public void MoveTilt(PlayerMovementState state)
    {
        if (state.movement.Equals(Vector3.zero))
        {
            noMove = true;
            return;
        }
        noMove = false;

        //Move
        float dot = Vector3.Dot(state.movement, playerOrientation.right);
        if (Mathf.Abs(dot) < 0.05f)
        {
            dot = 0;
            StepToward(Dimension.Z, 0);
        }
        if (dot > 0) StepToward(Dimension.Z, 1);
        else if (dot < 0) StepToward(Dimension.Z, -1);


        //JUMP
        //* This code works, it just looks dumb in game
        /**
        dot = Vector3.Dot(state.gravity, Vector3.up);
        Debug.Log("up dot: " + dot);
        if (Mathf.Abs(dot) < 0.05f)
        {
            dot = 0;
            StepToward(Dimension.X, 0);
        }
        if (dot > 0) StepToward(Dimension.X, -1);
        else if (dot < 0) StepToward(Dimension.X, 1);
        */
    }

    private void CameraTilt()
    {
        //TODO: Implement
    }

    /// <summary>
    /// Increases or decreases the specified dimension of steps by slerpStep, towards value
    /// </summary>
    /// <param name="dim">The dimension to step for</param>
    /// <param name="value">The value to step toward</param>
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

        // Calculate the angle between the two vectors
        float angle = Vector3.Angle(initialRotation, pullVector);
        Vector3 dest = initialRotation;

        // If the angle is greater than 180 degrees, negate one of the vectors
        if (angle > 180f)
        {
            dest = -dest;
            angle = 360f - angle;
        }

        // Slerp between the adjusted vectors
        dest.z = pullVector.z;
        res = Vector3.SlerpUnclamped(initialRotation, dest, steps.z);
        dest.y = pullVector.y;
        res = Vector3.SlerpUnclamped(res, dest, steps.y);
        dest.x = pullVector.x;
        res = Vector3.SlerpUnclamped(res, dest, steps.x);
        transform.SetLocalPositionAndRotation(transform.localPosition, Quaternion.Euler(res));
    }

    void FixedUpdate()
    {
        if (noMove)
        {
            StepToward(Dimension.Z, 0);
            StepToward(Dimension.X, 0);
            StepToward(Dimension.Y, 0);
        }
    }

    private enum Dimension
    {
        X = 0,
        Y = 1,
        Z = 2
    }
}