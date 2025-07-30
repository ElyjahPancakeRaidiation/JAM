using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class JumpController : MonoBehaviour
{

    private bool canJump;
    private Physics physics;

    private float horizontalInput;

    private isGroundedScript isGroundedScript;

    [SerializeField] private float height;

    [SerializeField] private float speed;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector2 groundCheckerdistance;
    // Start is called before the first frame update
    void Start()
    {
        physics = new Physics(GetComponent<Rigidbody2D>());
        isGroundedScript = GameObject.FindGameObjectWithTag("GroundRay").GetComponent<isGroundedScript>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
                
        callJump();
    }
    void FixedUpdate()
    {
       
    }

    void playerMovement()
    {
        // physics.Friction();
        horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 direction = new Vector2(speed, physics._rb.velocity.y);

        physics._rb.velocity = horizontalInput * direction;
    }

    void Jump()
    {
        float jump = Mathf.Sqrt(height * Physics2D.gravity.y * physics._rb.gravityScale * -2) * physics._rb.mass;
        Vector2 direction = new Vector2(physics._rb.velocity.x, jump);
        physics._rb.velocity = direction;
    }

    void callJump()
    {
        
        try
        {
            // if (isGroundedScript.isGrounded())
            // {
            //     canJump = true;
            // }
            // else canJump = false;

            if (canJump && Input.GetKeyDown(KeyCode.F))
            {
                Jump();
                Debug.Log("Space");
            }
        }
        catch (Exception)
        {
            Debug.Log("not found");
        }
    }

  
}
