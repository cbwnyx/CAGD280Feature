using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Washington, Christophe
//11.26.2023
//This script is in charge of the player movement.
public class PlayerMovement : MonoBehaviour
{
    private PlayerInputActions playerActions;
    private Vector2 moveVec = Vector2.zero;
    private Rigidbody rb;
    public float speed = 8f;

    private void Awake()
    {
        playerActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // the issue with this acceleration is that it keeps accelerating. I want to make it so that it accelerates up to a max speed of 8.
        speed = 4;
        speed += 5 * Time.deltaTime;
        rb.velocity = moveVec * speed;
        
    }

    private void OnEnable()
    {
        playerActions.Enable();
        playerActions.PlayerActions.Move.performed += OnMove;
        playerActions.PlayerActions.Move.canceled += OnMoveStop;
    }
    private void OnDisable()
    {
        playerActions.Disable();
        playerActions.PlayerActions.Move.performed -= OnMove;
        playerActions.PlayerActions.Move.canceled -= OnMoveStop;
    }

    private void OnMove(InputAction.CallbackContext value)
    {
        moveVec = value.ReadValue<Vector2>();
    }

    //monitors the player movement when input is stopped
    private void OnMoveStop(InputAction.CallbackContext value)
    {
        moveVec = Vector2.zero;
    }

    //This is the logic i need
    //if the player inputs left or right, they will move in that direction.
    //if the player inputs left or right, they will accelerate from immobile to a max speed of, say, 8.
    //if the player stops inputting left or right, they will deccelerate from their current speed to 0 quickly, but not instantly.
    //if the player presses jump, they will be able to move vertically. At the peak of their jump their vertical speed will slow to 0 before accelerating back towards the ground.
    //if the player is not on the ground and has already jumped, they will not be able to jump again.
    //if the player presses dash, they will dash in the direction they are "facing". if they are inputting any of the 8 cardinal directions while pressing dash, then they will dash in that direction instead. Prioritize input over no input.
    //if the player is next to a wall, they can grab the wall by holding a button.
}
