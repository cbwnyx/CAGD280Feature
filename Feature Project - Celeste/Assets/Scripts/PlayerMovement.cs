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

    public float speed = 4f;
    public float moveDir;
    public bool playerMoving = false;
    public bool isMovingRight;

    public bool isGrounded = true;
    public float jumpForce = 10f;
    public float gravityScale = 2f;
    public float maxVerticalForce;

    private bool wallSliding = false;
    private float wallSlideSpeed = 2f;
    public bool touchingWall;
    public bool climbingWall;

    private bool wallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    private float wallJumpDuration = 1f;
    private Vector2 wallJumpPower = new Vector2(12f, 8f);

    private bool canDash = true;
    public bool isDashing;
    private float dashingPower = 20f;
    private float dashTime = 0.2f;
    private Vector2 dashDir;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveDir = Input.GetAxisRaw("Horizontal");
        wallJumpDirection = -moveDir;

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
        //canRun();
        if (wallJumping == false)
        {
            run();
        }
        increaseGravity();
        wallSlide();
        playerDash();
        wallJump();
    }

    private void run()
    {
        //causes player to accelerate when initially moving instead of immediately moving at max speed
        
        if (isDashing == false)
        {
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

            if (Input.GetKey(KeyCode.RightArrow))
            {
                isMovingRight = true;
            }
            // HERE IS WHERE I LEFT OFF
            //Logic--if the player is moving in a direction, create a way to observe that the direction is changed.
            //if it is observed that the direction is changed, the player slows down before accelerating in the other direction.


            if (touchingWall == false)
            {
                rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
            }
            else
            {
                if (isGrounded)
                {
                    wallJumping = false;
                    rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
                }
            }

        }

        
        //rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        ////rb.velocity.y is stated as such because we are not messing with the y value, so it should stay the same


    }

    private void jump()
    {

        //rb.velocity.x is stated as such because we are not messing with the x value, so it should stay the same
        if (Input.GetKey("c") && isGrounded)
        {
            //jumpForce = jumpHeight * (gravityScale / timeSpacePressed);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            
        }

    }

    private void increaseGravity()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gravityScale = 3f;
            rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
        }
        else
        {
            gravityScale = 2f;
            rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
        }

    }

    private void wallSlide()
    {
        if (touchingWall)
        {
            if (isGrounded == false && moveDir != 0f)
            {
                wallSliding = true;
                // Mathf.Clamp keeps the values of the velocity between the given range
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            }
        }

        if (Input.GetKey("z") && touchingWall)
        {
            climbingWall = true;
        }
    }

    private void wallJump()
    {
        if (wallSliding)
        {
            wallJumping = false;
            //wallJumpDirection = -moveDir;
            wallJumpCounter = wallJumpTime;
        }
        else
        {
            //allows player to turn away from the wall, but still have a moment where they can wall jump
            touchingWall = false;
            wallJumpCounter -= Time.deltaTime;
        }

        if (Input.GetKey("c") && wallJumpCounter > 0f && touchingWall)
        {
            /*wallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpCounter = 0f;
            touchingWall = false;*/
            StartCoroutine(WallJumper());
        }

    }

    private IEnumerator WallJumper()
    {
        //cant run
        // perform jump
        //can run again
        wallJumping = true;
        touchingWall = false;

        rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
        yield return new WaitForSeconds(wallJumpDuration);
        wallJumping = false;
        wallJumpCounter = 0f;
        
    }

    private void playerDash()
    {
        if (Input.GetKey("x") && canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        //float originalGravity = gravityScale;
        gravityScale = 0f;

        dashDir = new Vector2(moveDir, Input.GetAxisRaw("Vertical"));
        if (dashDir == Vector2.zero)
        {
            dashDir = new Vector2(transform.localScale.x, 0);
        }
        rb.velocity = dashDir.normalized * dashingPower;
        yield return new WaitForSeconds(dashTime);
        if (rb.velocity.y > 6.5f)
        {
            maxVerticalForce = 6.5f;
            rb.velocity = new Vector2(rb.velocity.x, maxVerticalForce);
        }
        gravityScale = 2f;
        isDashing = false;


    }

    
    private void OnCollisionEnter(Collision collision)
    {
        /*if (collision.gameObject.tag == "Wall")
        {
            touchingWall = true;
        }*/
        if (collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
            canDash = true;
            touchingWall = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            touchingWall = true;
        }
    }
    /*private void canRun()
    {
        if (wallJumping == false || climbingWall == false)
        {
            run();
        }
    }*/
    // if wall jumping is true, running is false
    // if wall climbing is true, running is false


    //This is the logic i need
    //if the player inputs left or right, they will move in that direction. Done
    //if the player inputs left or right, they will accelerate from immobile to a max speed of, say, 8. Done (max speed 6.5)
    //if the player stops inputting left or right, they will deccelerate from their current speed to 0 quickly, but not instantly. Done
    //if the player presses jump, they will be able to move vertically. At the peak of their jump their vertical speed will slow to 0 before accelerating back towards the ground. Done
    //if the player is not on the ground and has already jumped, they will not be able to jump again. Done (was raycasting, now just using collision)
    //if the player presses dash, they will dash in the direction they are "facing". if they are inputting any of the 8 cardinal directions while pressing dash, then they will dash in that direction instead. Prioritize input over no input. Done
    //if the player is next to a wall, they can grab the wall by holding a button. Wallslide Done
    //Dash Issue: The player has a movement speed limit. This is to handle acceleration, but if the player dashes with any horizontal input, it makes the dash very short and not very fast. On the contrary, if the player dashes vertically, they will go very far and fast due to the lack of speed limit. Fixed
}
