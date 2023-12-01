using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Washington, Christophe
//11.26.2023
//This script is in charge of the player movement.
public class PlayerMovement : MonoBehaviour
{
    //private PlayerInputActions playerActions;
    //private Vector2 moveVec = Vector2.zero;
    
    private Rigidbody rb;

    public float speed = 4f;
    public float moveDir;
    public bool playerMoving = false;

    public bool isGrounded = true;
    public float jumpForce = 10f;
    public float gravityScale = 2f;
    //public float jumpHeight = 10f;
    
    //public float startTime;
    //public float timeSpacePressed;

    private void Awake()
    {
        //playerActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDir = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        jump();
        run();
        increaseGravity();
    }

    private void run()
    {
        //causes player to accelerate when initially moving instead of immediately moving at max speed
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            if (speed < 6.5f)
            {
                speed += 15 * Time.deltaTime;
            }
        }
        
        else
        {
            if (speed > 4f)
            {
                speed -= 30 * Time.deltaTime;
            }
        }

        ////rb.velocity.y is stated as such because we are not messing with the y value, so it should stay the same
        rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);

    }

    private void jump()
    {

        //use raycasting to determine whether the player is grounded, and can jump
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity);

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1f))
        {
            isGrounded = true;
        }
        
        else
        {
            isGrounded = false;
        }


        /*if (Input.GetKeyDown("c"))
        {
            startTime = Time.deltaTime;
        }
        if(Input.GetKeyUp("c") && isGrounded)
        {
            timeSpacePressed = Time.deltaTime - startTime;
            jumpForce = 0;
            startTime = 0;
            timeSpacePressed = 0;
        }*/

        //rb.velocity.x is stated as such because we are not messing with the x value, so it should stay the same
        if (Input.GetKey("c") && isGrounded)
        {
            //jumpForce = jumpHeight * (gravityScale / timeSpacePressed);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            
        }

    }

    private void increaseGravity()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.AddForce(Physics.gravity * (gravityScale - 1) * (rb.mass * 2));
        }
        else
        {
            rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
        }

    }

    private void wallGrab()
    {
        if (Input.GetKey("z"))
        {
            rb.velocity = new Vector2(0,0);
            Debug.Log("Grabbed Wall");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            wallGrab();
        }
        


        /*if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("Touched Wall");
            if (Input.GetKey("z"))
            {
                rb.velocity = new Vector2(0, 0);
                Debug.Log("Pressed Grab");
            }
            else
            {
                rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
            }
        }*/
    }
    /*private void OnEnable()
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
        playerMoving = true;
    }

    //monitors the player movement when input is stopped
    private void OnMoveStop(InputAction.CallbackContext value)
    {
        moveVec = Vector3.zero;
        playerMoving = false;
    }

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
        //rb.velocity = moveVec * speed;

    }*/


    //This is the logic i need
    //if the player inputs left or right, they will move in that direction. Done
    //if the player inputs left or right, they will accelerate from immobile to a max speed of, say, 8. Done (max speed 6.5)
    //if the player stops inputting left or right, they will deccelerate from their current speed to 0 quickly, but not instantly. Done
    //if the player presses jump, they will be able to move vertically. At the peak of their jump their vertical speed will slow to 0 before accelerating back towards the ground. Done
    //if the player is not on the ground and has already jumped, they will not be able to jump again. Done (raycasting)
    //if the player presses dash, they will dash in the direction they are "facing". if they are inputting any of the 8 cardinal directions while pressing dash, then they will dash in that direction instead. Prioritize input over no input.
    //if the player is next to a wall, they can grab the wall by holding a button.
}
