using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController controller;
    Vector3 moveInput;
    public float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    [ServerRpc]
    public void MoveOnServerRPC(Vector3 input)
    {
        moveInput = input;       
    }    

    // Update is called once per frame
    void Update()
    {
        controller.Move(moveInput * speed * Time.deltaTime);
    }
}
