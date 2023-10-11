using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    // Sensitivity settings for mouse look
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;
    public Transform playerBody; // Player's body to rotate

    private PlayerControls controls;
    private Vector2 mouseMovement; // Current frame's mouse movement
    private float playerRotation; // Accumulated vertical rotation

    public void Initialize(PlayerControls controlsInstance)
    {
        // Setup the player controls for this component
        controls = controlsInstance;
    }

    private void Start()
    {
        // Lock and hide the cursor for an FPS look
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Subscribe to inputs
        controls.std.Look.performed += ctx => {
            // Reads in mouse movement and adjusts for sensitivity
            Vector2 rawMouseMovement = ctx.ReadValue<Vector2>();
            mouseMovement = new Vector2(rawMouseMovement.x * xSensitivity * Time.deltaTime,
                                        rawMouseMovement.y * ySensitivity * Time.deltaTime);
            Look(mouseMovement);
        };
        controls.std.Look.canceled += ctx => mouseMovement = Vector2.zero;
    }

    private void Look(Vector2 mouseDelta)
    {
        // Adjust and clamp the player's vertical rotation based on mouse movement
        playerRotation -= mouseMovement.y;
        playerRotation = Mathf.Clamp(playerRotation, -90f, 90f);

        // Apply rotations: vertical rotation for camera, and horizontal rotation on the player body
        transform.localRotation = Quaternion.Euler(playerRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseMovement.x);
    }
}
