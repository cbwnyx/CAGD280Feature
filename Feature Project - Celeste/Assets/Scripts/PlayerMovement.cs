using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Washington, Christophe
//11.26.2023
//This script is in charge of the player movement.
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Run Variables")]
    public float speed = 4f;
    public float moveDir;
    public float wallMoveDir;
    public bool playerMoving = false;
    public bool isMovingRight;

    [Header("Jump Variables")]
    public bool isGrounded = true;
    private float jumpForce = 10f;
    public float gravityScale = 2f;
    private float maxVerticalForce;

    [Header("Wall Climb and Slide Variables")]
    public bool touchingWall;
    public bool climbingWall;
    private bool wallSliding = false;
    private float wallSlideSpeed = 2f;

    private bool wallJumping;
    private Vector2 wallJumpPower = new Vector2(12f, 8f);
    private float wallJumpDirection;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    private float wallJumpDuration = 1f;

    [Header("Dash Variables")]
    private bool canDash = true;
    public bool isDashing;
    private float dashingPower = 20f;
    private float dashTime = 0.2f;
    private Vector2 dashDir;
    
    //Initialize Rigidbody
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Monitors directional input
        moveDir = Input.GetAxisRaw("Horizontal");
        //Makes the wall jump direction opposite of the input
        wallJumpDirection = -moveDir;

        //Changes player color to indicate whether dash is usable or not
        if (canDash == false)
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0.4f, 0.9f, 0.9f, 1f);
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0.4f, 0f, 0f, 1f);
        }
    }

    private void FixedUpdate()
    {
        jump();
        if (climbingWall == false)
        {
            rb.useGravity = true;
            increaseGravity();
            wallSlide();
            if (wallJumping == false)
            {
                run();
            }
        }
        else
        {
            wallClimb();
        }
        playerDash();
        wallJump();
    }
    private void increaseGravity()
    {
        //If the player inputs down, the gravity will increase. Otherwise, the gravity stays the same at 2.5f
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gravityScale = 4f;
            rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
        }
        else
        {
            gravityScale = 2f;
            rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
        }

    }

    private void run()
    {
        if (isDashing == false)
        {
            //causes player to accelerate when initially moving instead of immediately moving at max speed
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                if (speed < 6.5f)
                {
                    speed += 15 * Time.deltaTime;
                }
            }
            //if the player stops movement input, the player object slows to a stop instead of immediately stopping
            else
            {
                if (speed > 4f)
                {
                    speed -= 30 * Time.deltaTime;
                }

            }

            //if it is observed that the player movement direction is changed, the player slows down before accelerating in the other direction.
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (isMovingRight == false)
                {
                    speed = 4f;
                    isMovingRight = true;
                }

            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (isMovingRight)
                {
                    speed = 4f;
                    isMovingRight = false;
                }
            }

            //If the player is touching or moving into a wall, they will be unable to move
            if (touchingWall == false)
            {
                rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
            }
            else
            {
                
                if (moveDir == -wallMoveDir)
                {
                    touchingWall = false;
                    wallJumping = false;
                    rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
                }
            }
        }
    }

    private void playerDash()
    {
        //If the player presses the dash button and they are able to dash, they will dash
        if (Input.GetKey("x") && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        gravityScale = 0f;

        //Gets directional input in 8 directions
        dashDir = new Vector2(moveDir, Input.GetAxisRaw("Vertical"));

        //If there is no directional input from the player, they will dash straight right
        if (dashDir == Vector2.zero)
        {
            dashDir = new Vector2(transform.localScale.x, 0);
        }

        //normalizes directional input to one of 8 directions, and moves player in that direction
        rb.velocity = dashDir.normalized * dashingPower;
        yield return new WaitForSeconds(dashTime);

        //Sets a vertical velocity cap on player so that their momentum is consistent across y and x axis
        if (rb.velocity.y > 6.5f)
        {
            maxVerticalForce = 6.5f;
            rb.velocity = new Vector2(rb.velocity.x, maxVerticalForce);
        }

        gravityScale = 2f;
        isDashing = false;
    }

    private void jump()
    {
        //rb.velocity.x is stated as such because we are not messing with the x value, so it should stay the same
        if (Input.GetKey("c") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    private void wallSlide()
    {
        //If the player is touching the wall and is not grounded, they will slowly slide towards the ground
        if (touchingWall)
        {
            // Mathf.Clamp keeps the values of the velocity between the given range
            if (isGrounded == false && moveDir != 0f)
            {
                wallSliding = true;
                climbingWall = false;
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            }
        }
        if (Input.GetKey("z") && touchingWall)
        {
            climbingWall = true;
        }

    }
    private void wallClimb()
    {
        //If the player is touching the wall and they input "z", they will stop moving and will hang onto the wall
        if (climbingWall)
        {
            wallSliding = false;
            rb.velocity = Vector2.zero;
            rb.useGravity = false;
            climbingWall = false;

            //If the player inputs up or down, they will climb up or down
            if (Input.GetKey(KeyCode.UpArrow))
            {
                rb.velocity = new Vector2(rb.velocity.x, 4f);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                rb.velocity = new Vector2(rb.velocity.x, -4f);
            }
        }
    }
    private void wallJump()
    {
        //If the player is sliding down the wall, they are not wall jumping
        if (wallSliding || climbingWall)
        {
            wallJumping = false;
            wallJumpCounter = wallJumpTime;
        }
        //allows player to turn away from the wall, but still have a moment where they can wall jump
        else
        {
            touchingWall = false;
            wallJumpCounter -= Time.deltaTime;
        }
        //If the player presses jump input and is touching the wall, the wall jump coroutine begins
        if (Input.GetKey("c") && wallJumpCounter > 0f && touchingWall)
        {
            StartCoroutine(WallJumper());
        }
    }
    private IEnumerator WallJumper()
    {
        //If the player wall jumps, they are detached from the wall and can jump outwards or upwards depending on directional input
        wallJumping = true;
        climbingWall = false;
        touchingWall = false;

        rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
        yield return new WaitForSeconds(wallJumpDuration);

        speed = 2f;
        wallJumping = false;
        wallJumpCounter = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If the player collides with the wall, save the direction they are moving for later (for detaching from the wall)
        if (collision.gameObject.tag == "Wall")
        {
            wallMoveDir = moveDir;
        }
        //If the player collides with the floor, refreshes dash
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
            canDash = true;
            touchingWall = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //As long as the player is colliding with the wall, touchingWall is true
        if (collision.gameObject.tag == "Wall")
        {
            touchingWall = true;
        }
    }

    //This is the logic i need
    //if the player inputs left or right, they will move in that direction. Done
    //if the player inputs left or right, they will accelerate from immobile to a max speed of, say, 8. Done (max speed 6.5)
    //if the player stops inputting left or right, they will deccelerate from their current speed to 0 quickly, but not instantly. Done
    //if the player presses jump, they will be able to move vertically. At the peak of their jump their vertical speed will slow to 0 before accelerating back towards the ground. Done
    //if the player is not on the ground and has already jumped, they will not be able to jump again. Done (was raycasting, now just using collision)
    //if the player presses dash, they will dash in the direction they are "facing". if they are inputting any of the 8 cardinal directions while pressing dash, then they will dash in that direction instead. Prioritize input over no input. Done
    //if the player is next to a wall, they can grab the wall by holding a button. Done
    //Dash Issue: The player has a movement speed limit. This is to handle acceleration, but if the player dashes with any horizontal input, it makes the dash very short and not very fast. On the contrary, if the player dashes vertically, they will go very far and fast due to the lack of speed limit. Fixed
}
