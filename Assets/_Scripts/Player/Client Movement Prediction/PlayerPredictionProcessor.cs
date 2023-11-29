using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerMovementInput : PRN.IInput, Unity.Netcode.INetworkSerializable
{
    public int tick;
    public Vector2 lateralMovement;
    public bool jump;
    public Vector2 mouseLook;

    public void SetTick(int tick) => this.tick = tick;
    public readonly int GetTick() => tick;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref lateralMovement);
        serializer.SerializeValue(ref jump);
        serializer.SerializeValue(ref mouseLook);
    }
}

public struct PlayerMovementState : PRN.IState, Unity.Netcode.INetworkSerializable
{
    public int tick;
    public Vector3 position;
    public Vector3 movement;
    public Vector3 gravity;
    public Quaternion orientation;

    public void SetTick(int tick) => this.tick = tick;
    public readonly int GetTick() => tick;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref movement);
        serializer.SerializeValue(ref gravity);
        serializer.SerializeValue(ref orientation);
    }
}

public class PlayerPredictionProcessor : MonoBehaviour, PRN.IProcessor<PlayerMovementInput, PlayerMovementState>
{
    private CharacterController controller;
    private Transform playerOrientation;

    [Header("Movement Settings")]
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float jumpHeight;
    [SerializeField]
    private float gravityForce = -9.81f;
    [SerializeField]
    [Tooltip("The multiplier for the player's downward acceleration if they are already falling")]
    private float gravityDownwardAcceleration = 1.3f;

    private Vector3 movement = Vector3.zero;
    private Vector3 gravity = Vector3.zero;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerOrientation = transform.Find("PlayerModel");
    }

    public PlayerMovementState Process(PlayerMovementInput input, TimeSpan deltaTime)
    {
        movement = (float)deltaTime.TotalSeconds * movementSpeed * (playerOrientation.forward * input.lateralMovement.y + playerOrientation.right * input.lateralMovement.x).normalized;
        if (controller.isGrounded)
        {
            gravity = Vector3.zero;
            if (input.jump)
                gravity = (float)deltaTime.TotalSeconds * Mathf.Sqrt(jumpHeight * 2 * -gravityForce) * Vector3.up;
        }
        if (gravity.y > 0)
            gravity += gravityForce * Mathf.Pow((float)deltaTime.TotalSeconds, 2) * Vector3.up;
        else
            gravity += gravityDownwardAcceleration * gravityForce * Mathf.Pow((float)deltaTime.TotalSeconds, 2) * Vector3.up;

        controller.Move(movement + gravity);

        Look(input.mouseLook);

        return new()
        {
            position = transform.position,
            movement = movement,
            gravity = gravity,
            orientation = transform.rotation
        };
    }

    private void Look(Vector2 mouseInput)
    {
        // Retrieve rotation
        var xRotation = transform.rotation.eulerAngles.x;
        xRotation -= mouseInput.y;
        var yRotation = transform.rotation.eulerAngles.y;
        yRotation += mouseInput.x;

        // Clamp x rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate player's model to match camera
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    /// <summary>
    /// Called when inconsistency occurs
    /// </summary>
    /// <param name="state"></param>
    public void Rewind(PlayerMovementState state)
    {
        controller.enabled = false;
        transform.position = state.position;
        movement = state.movement;
        gravity = state.gravity;
        controller.enabled = true;
    }

}


