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

    public bool isGrounded = true;
    public float jumpForce = 10f;
    public float gravityScale = 2f;

    private bool wallSliding = false;
    private float wallSlideSpeed = 2f;

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 30f;
    private float dashTime = 0.2f;
    private Vector2 dashDir;
    private float dashCooldown = 1f;

    public bool touchingWall;

    //Serialize fields here to be able to reference the wallcheck
    [SerializeField] private Transform wallCheck;
    //[SerializeField] private LayerMask wallLayer;
    public LayerMask wallLayer;

    private void Awake()
    {
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
        //wallSlide();
        wallTouch();
        playerDash();
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

        if (touchingWall == false)
        {
            rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        }
        else
        {
            if (isGrounded)
            {
                touchingWall = false;
                rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
            }
        }
        ////rb.velocity.y is stated as such because we are not messing with the y value, so it should stay the same
        

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

    /*private bool touchingWall()
    {
        //this tells us whether the empty game object "WallCheck" is making contact with a wall
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
        Debug.Log("Touched Wall");
    }*/

    private void wallTouch()
    {
        RaycastHit wallHit;
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out wallHit, Mathf.Infinity);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * wallHit.distance, Color.red);
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out wallHit, 1f))
        {
            if (isGrounded == false && moveDir != 0f)
            {
                wallSliding = true;
                // Mathf.Clamp keeps the values of the velocity between the given range
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
            }
        }
    }

    /*private void wallSlide()
    {
        //if the player is touching the wall and is not grounded and there is a directional input, the player will wall slide
        if (touchingWall() && isGrounded == false && moveDir != 0f)
        {
            wallSliding = true;
            // Mathf.Clamp keeps the values of the velocity between the given range
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            wallSliding = false;
        }
    }*/

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
        float originalGravity = gravityScale;
        gravityScale = 0f;

        dashDir = new Vector2(moveDir, Input.GetAxisRaw("Vertical"));
        if (dashDir == Vector2.zero)
        {
            dashDir = new Vector2(transform.localScale.x, 0);
        }
        rb.velocity = dashDir.normalized * dashingPower;
        yield return new WaitForSeconds(dashTime);
        gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
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
    //if the player is not on the ground and has already jumped, they will not be able to jump again. Done (raycasting)
    //if the player presses dash, they will dash in the direction they are "facing". if they are inputting any of the 8 cardinal directions while pressing dash, then they will dash in that direction instead. Prioritize input over no input.
    //if the player is next to a wall, they can grab the wall by holding a button.
}
