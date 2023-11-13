using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Washington, Christophe
//11.09.2023
//This script creates a scriptable object named PlayerData, which handles all of the physics and momentum which is applied to the player.
public class PlayerData : ScriptableObject
{
    public float maxRunSpeed;

    public float runAcceleration;
    public float runAccelForce;

    public float runNegAcceleration;
    public float runNegAccelForce;

    public float accelAirborne;
    public float negAccelAirborne;

    public bool conserveMomentum = true;


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


}
