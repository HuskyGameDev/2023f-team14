using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    Vector3 moveDir = new(0, 0, 0);
    // Update is called once per frame
    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDir.z = +1f;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            moveDir.z = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir.z = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            moveDir.z = 0;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir.x = +1f;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            moveDir.x = 0;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir.x = -1f;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            moveDir.x = 0;
        }

        float moveSpeed = 3f;
        transform.position += moveSpeed * Time.deltaTime * moveDir;
    }
}
