using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;

    public Transform orientation;

    private float xRotation;
    private float yRotation;

    private Vector2 mouseMovement;
    private PlayerControls playerControls;

    public void Initialize(PlayerControls controlsInstance)
    {
        playerControls = controlsInstance;
    }

    private void Start()
    {
        if (playerControls == null)
        {
            Debug.LogError("Controls instance is null in MouseLook script.");
            return;
        }

        // Hides cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Subscribe to actions
        playerControls.std.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        playerControls.std.Look.canceled += ctx => mouseMovement = Vector2.zero;
    }

    private void Look(Vector2 rawMouseMovement)
    {
        mouseMovement = new Vector2(rawMouseMovement.x * xSensitivity * Time.deltaTime, rawMouseMovement.y * ySensitivity * Time.deltaTime);

        // Retrieve rotation
        xRotation -= mouseMovement.y;
        yRotation += mouseMovement.x;

        // Clamp x rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate player's model to match camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
