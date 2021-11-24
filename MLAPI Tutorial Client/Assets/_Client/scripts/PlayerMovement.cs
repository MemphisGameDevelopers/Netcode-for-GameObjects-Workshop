using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
// using MLAPI;
// using MLAPI.Messaging;

public class PlayerMovement : NetworkBehaviour
{
    CharacterController controller;
    Vector2 moveInput;
    Vector3 movement;
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void SetMovement(InputAction.CallbackContext input)
    {
        if(IsOwner)
        {
            moveInput = input.ReadValue<Vector2>();
            movement = new Vector3(moveInput.x, 0, moveInput.y);
            MoveOnServerRPC(movement);    
        } 
    }

    [ServerRpc]
    public void MoveOnServerRPC(Vector3 input){}
   


    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        controller.Move(movement * speed * Time.deltaTime);
    }
        
}
