using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float xSensitivity = 100f;
    public float ySensitivity = 100f;
    public Transform playerBody;

    private PlayerControls controls;
    private Vector2 mouseMovement;
    private float playerRotation;

    public void Initialize(PlayerControls controlsInstance)
    {
        controls = controlsInstance;
    }

    private void Start()
    {
        // Hides cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Subscribe to events
        controls.std.Look.performed += ctx =>
        {
            Vector2 rawMouseMovement = ctx.ReadValue<Vector2>();
            mouseMovement = new Vector2(rawMouseMovement.x * xSensitivity * Time.deltaTime,
                                        rawMouseMovement.y * ySensitivity * Time.deltaTime);
            Look(mouseMovement);
        };
        controls.std.Look.canceled += ctx => mouseMovement = Vector2.zero;
    }

    private void Look(Vector2 mouseDelta)
    {
        // Clamp player's up/down rotation
        playerRotation -= mouseMovement.y;
        playerRotation = Mathf.Clamp(playerRotation, -90f, 90f);

        // Rotate player's model to match camera
        transform.localRotation = Quaternion.Euler(playerRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseMovement.x);
    }
}
