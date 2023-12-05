using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Washington, Christophe
//12.5.2023
//This script adjusts the gravity so that the jump feels more comfortable and less floaty.
public class PlayerJumpAdjustments : MonoBehaviour
{
    public float gravityMultiplier = 3f;
    public float hopMultiplier = 2.5f;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //If the player is descending, their fall speed will be increased.
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.deltaTime;
        }
        //If the player does not press the jump input for long, the jump will be shorter.
        else if (rb.velocity.y > 0 && !Input.GetKey("c"))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (hopMultiplier - 1) * Time.deltaTime;
        }
    }
}
