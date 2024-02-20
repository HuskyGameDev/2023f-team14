using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseLook : PlayerControlsNetworkBehaviour
{
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    public Transform PlayerOrientation;

    private float xRotation;
    private float yRotation;

    public Vector3 Forward => new(transform.forward.x, 0f, transform.forward.z);
    public Vector3 Right => new(transform.right.x, 0f, transform.right.z);

    private Vector2 mouseMovement;
    private CameraRecoil recoil;
    private PlayerControls playerControls;

    private void Awake()
    {
        recoil = GetComponent<CameraRecoil>();
    }

    public override void Initialize(PlayerControls pc)
    {
        playerControls = pc;
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
        playerControls.std.Pause.started += ctx => LockCursor(Cursor.visible);

    }

    /// <summary>
    /// Move the camera and rotate the player given a mouse input
    /// </summary>
    /// <param name="rawMouseMovement"></param>
    private void Look(Vector2 rawMouseMovement)
    {
        if (Cursor.visible) return;
        mouseMovement = new Vector2(rawMouseMovement.x * xSensitivity * Time.deltaTime, rawMouseMovement.y * ySensitivity * Time.deltaTime);
        //xRotation += mouseMovement.x;
        // Clamp x rotation

        xRotation = transform.rotation.eulerAngles.x - mouseMovement.y;
        if (xRotation > 85f && xRotation < 160f) xRotation = 85f;
        if (xRotation > 160f && xRotation < 275f) xRotation = 275f;

        LookToward(mouseMovement);
    }

    /// <summary>
    /// Sets the player forward vector to the given vector
    /// </summary>
    /// <param name="forward">the vector to look at</param>
    public void LookToward(Vector3 forward)
    {
        PlayerOrientation.rotation = Quaternion.Euler(0f, PlayerOrientation.rotation.eulerAngles.y + forward.x, 0f);
        transform.rotation = Quaternion.Euler(xRotation, PlayerOrientation.rotation.eulerAngles.y + forward.x, 0f);
    }

    public Vector2 GetInput()
    {
        return mouseMovement;
    }

    public void LockCursor(bool isLocked = true)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
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
