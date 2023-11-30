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

    public float speed = 4f;
    public bool playerMoving = false;

    //private Vector2 moveJump = Vector2.zero;
    public bool isGrounded = true;
    public float jumpForce = 100f;

    private void Awake()
    {
        playerActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        jump();
        runAcceleration();
    }

    private void OnEnable()
    {
        playerActions.Enable();
        playerActions.PlayerActions.Move.performed += OnMove;
        playerActions.PlayerActions.Move.canceled += OnMoveStop;

        //playerActions.PlayerActions.Jump.performed += OnJump;
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
        playerMoving = true;
    }

    //monitors the player movement when input is stopped
    private void OnMoveStop(InputAction.CallbackContext value)
    {
        moveVec = Vector2.zero;
        playerMoving = false;
    }

    /*private void OnJump(InputAction.CallbackContext value)
    {
        moveJump = value.ReadValue<Vector2>();
    }*/

    //if speed is less than 6.5, accelerate
    private void runAcceleration()
    {
        if (playerMoving == true)
        {
            if (speed < 6.5f)
            {
                speed += 15 * Time.deltaTime;
            }
        }
        if (playerMoving == false)
        {
            if (speed > 4f)
            {
                speed -= 30 * Time.deltaTime;
            }
        }
        rb.velocity = moveVec * speed;
    }

    private void jump()
    {
        //use raycasting to determine whether the player is grounded, and can jump
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector2.down), out hit, Mathf.Infinity);
        
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector2.down), out hit, 1.5f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (Input.GetKey("space") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
        }
    }
    //This is the logic i need
    //if the player inputs left or right, they will move in that direction. Done
    //if the player inputs left or right, they will accelerate from immobile to a max speed of, say, 8. Done (max speed 6.5)
    //if the player stops inputting left or right, they will deccelerate from their current speed to 0 quickly, but not instantly. Done
    //if the player presses jump, they will be able to move vertically. At the peak of their jump their vertical speed will slow to 0 before accelerating back towards the ground. (Partially done--pressing space does move the player vertically, but very slowly in a kind of ascent rather than a jump.)
    //if the player is not on the ground and has already jumped, they will not be able to jump again.
    //if the player presses dash, they will dash in the direction they are "facing". if they are inputting any of the 8 cardinal directions while pressing dash, then they will dash in that direction instead. Prioritize input over no input.
    //if the player is next to a wall, they can grab the wall by holding a button.
}
