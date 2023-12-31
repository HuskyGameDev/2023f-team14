using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseLook : Unity.Netcode.NetworkBehaviour
{
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    public Vector3 Forward => new(transform.forward.x, 0f, transform.forward.z);
    public Vector3 Right => new(transform.right.x, 0f, transform.right.z);

    private Vector2 mouseMovement;
    private PlayerControls playerControls;

    public void Initialize(PlayerControls controlsInstance)
    {
        playerControls = controlsInstance;
    }

    public override void OnNetworkSpawn()
    {
        //Debug.Log("Camera spawned. " + (IsOwner ? "It's " : "Not ") + "mine!");
        gameObject.SetActive(IsOwner);
        if (!IsOwner) return;

        if (playerControls == null)
        {
            Debug.LogError("Controls instance is null in MouseLook script.");
            return;
        }
        // Hides cursor
        LockCursor(true);

        // Subscribe to actions
        playerControls.std.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        playerControls.std.Look.canceled += ctx => mouseMovement = Vector2.zero;
        playerControls.std.Escape.started += ctx => LockCursor(Cursor.visible);

    }

    private void Look(Vector2 rawMouseMovement)
    {
        if (Cursor.visible) return;
        mouseMovement = new Vector2(rawMouseMovement.x * xSensitivity * Time.deltaTime, rawMouseMovement.y * ySensitivity * Time.deltaTime);
        //xRotation += mouseMovement.x;
        // Clamp x rotation

        xRotation = transform.rotation.eulerAngles.x - mouseMovement.y;
        if (xRotation > 85f && xRotation < 160f) xRotation = 85f;
        if (xRotation > 160f && xRotation < 275f) xRotation = 275f;


        orientation.rotation = Quaternion.Euler(0f, orientation.rotation.eulerAngles.y + mouseMovement.x, 0f);
        transform.rotation = Quaternion.Euler(xRotation, orientation.rotation.eulerAngles.y + mouseMovement.x, 0f);
    }

    public Vector2 GetInput()
    {
        return mouseMovement;
    }

    private void LockCursor(bool lok = true)
    {
        Cursor.lockState = lok ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lok;
    }

    private void OnDrawGizmos()
    {
        if (!IsServer) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 100);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 100);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 100);
    }
}
