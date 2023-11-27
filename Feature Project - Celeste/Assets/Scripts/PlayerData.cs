using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Washington, Christophe
//11.09.2023
//This script creates a scriptable object named PlayerData, which handles all of the physics and momentum which is applied to the player.
public class PlayerData : ScriptableObject
{
    //Momentum

    public float maxRunSpeed;

    public float runAcceleration;
    public float runAccelForce;

    public float runNegAcceleration;
    public float runNegAccelForce;

    public float accelAirborne;
    public float negAccelAirborne;

    public bool conserveMomentum = true;


    //Gravity

    //How much downwards force is applied to the character
    public float gravityStrength;
    //How much that downwards force is multiplied by
    public float gravityScale;

    //multiplies gravityScale when the player is falling
    public float fallMultiplier;
    //terminal velocity
    public float maxFallSpeed;

    //same principle as FallMultiplier and maxFallSpeed, but specifically when down is being pressed/held
    public float fastfallMultiplier;
    public float maxFastFallSpeed;

    //Jump

    //height of jump
    public float jumpHeight;
    //float of the time between jump input and reaching peak of jump
    //influences player gravity and jump force
    public float jumpAirTime;
    //upwards force of player when jumping
    public float jumpForce;

    //Jump Control

    //gravity increase if player releases the jump button while still jumping
    public float jumpGravityMultiplier;
    //reduces gravity when the player is near peak of jump
    public float jumpHangGravity;
    //affects y velocity when near peak of jump
    public float jumpHangTime;
    //explain later
    public float jumpHangAccel;
    public float jumpHangMaxSpeed;

    //Wall Jump
    //force of jump when player wall jumps
    public Vector3 wallJumpForce;
    //reduces how effective player movement is when the player is wall jumping
    public float wallJumpLerp;
    //how long wallJumpLerp affects player for
    public float wallJumpTime;
    //player will turn after jumping off wall
    public bool wallJumpTurn;

    //Wall Slide

    public float slideSpeed;
    public float slideAccel;

    //Coyote Time and Delayed Jump

    //after player falls off of a platform, there is a period of time where they can still jump and the game will accept the input
    public float coyoteTime; 
    //if the player inputs jump early, the game will perform the input once it reaches the ground again
    public float jumpDelayedInput;

    //when inspector updates, this function happens https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html
    private void OnValidate()
    {
        //calculate the strength of the gravity applied to the player based on how much time is going to be spent in the air
        gravityStrength = -(2 * jumpHeight) / (jumpAirTime * jumpAirTime);

        gravityScale = gravityStrength / Vector3.up.y;

        runAccelForce = (50 * runAcceleration) / maxRunSpeed;
        runNegAccelForce = (50 * runNegAcceleration) / maxRunSpeed;

        jumpForce = Mathf.Abs(gravityStrength) * jumpAirTime;

        //ranges for acceleration and negative acceleration (slowing down)
        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, maxRunSpeed);
        runNegAcceleration = Mathf.Clamp(runNegAcceleration, 0.01f, maxRunSpeed);

    }
}
