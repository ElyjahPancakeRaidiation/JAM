using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public isGroundedScript isGroundedScript { get; private set; }
    private GameManager gm;
    private Rigidbody2D _rb;

    #region Dash variables
    [SerializeField] private float DASHPOWERX = 18, DASHPOWERY = 14;
    [SerializeField] private float UNCHANGEDDASHY = 14;
    [SerializeField] private int maxDashes;
    private int dashAmount;
    private bool canUseAbility;
    #endregion

    #region Pogo variables
    private const float SUPERJUMP = 28;
  

    public bool usedJumpAbility = false;
    [SerializeField] private bool canJumpNextFrame = false;
    public float jumpFrameTimer = 0;
    public float maxJumpFrameTimer;
    public bool recentlyJumped;
    

    private bool jumpAgain;

    public bool jumpedClicked;

    [SerializeField] private float Jumpheight;

    #endregion

    [SerializeField] private float groundCheckerDistance;
    [SerializeField] private LayerMask groundMask;

    public bool usedJump;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        isGroundedScript = GameObject.FindGameObjectWithTag("GroundRay").GetComponent<isGroundedScript>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        dashAmount = maxDashes;
        canUseAbility = true;
        jumpAgain = true;
        jumpedClicked = false;
        
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(gm.playerAbilityKey) && canUseAbility)
        {
        
            useFormsAbility();
        }

    }


    public void useFormsAbility()
    {
        string formName = playerMovement.getCurForm().formName;
        switch (formName)
        {
            case "Ball":

                //Will have the dashing ability

                dashAbility();
                break;
            case "Pogo":
                //Will have the mega jump 


                if (isGroundedScript.isGrounded())
                {
                    StartCoroutine(JumpAbility());
                    
            
                }
                else if (playerMovement.coyoteTimer > .56 && playerMovement.coyoteTimer < .65) {
                    StartCoroutine(JumpCoyoteTimer());
                
                    StartCoroutine(JumpAbility());

                }

                break;
        }
    }



    #region Ball Ability
    private void dashAbility()
    {
        if (dashAmount > 0)
        {
            _rb.velocity = Vector2.zero;
            //based of the horizontal input -1, 0, 1
            //0 will now only go up might be good for more movement combinations?
            var horInput = playerMovement.getInput();
       
            if (horInput != 0)
            {
                _rb.AddForce(new Vector2(horInput * DASHPOWERX, DASHPOWERY), ForceMode2D.Impulse);
               
                
            }

            if (horInput == 0)
            {
             
                _rb.AddForce(new Vector2(horInput * DASHPOWERX, UNCHANGEDDASHY), ForceMode2D.Impulse);
            }
            
            dashAmount--;
            if (dashAmount == 0) { StartCoroutine(dashAmountBack()); }
        }
    }

    private IEnumerator dashAmountBack()
    {
        //This function puts a short cool down when getting your dash back since if this isnt here it will
        //automatically give your dash allowing double dash
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGroundedScript.isGrounded());
        dashAmount = maxDashes;
    }


    #endregion

    #region Pogo Ability
    //this delay is so play can't infinitely jump while coyote timer is on. Otherwise you are able to double or even triple jump
    //if you spam the jump key
    private IEnumerator JumpCoyoteTimer()
    {
        yield return new WaitForSeconds(.03f);
        playerMovement.coyoteTimer = 0;

    }
   

 
    private IEnumerator JumpAbility()
    {
        jumpedClicked = true;
        float jumpForce = Mathf.Sqrt(Jumpheight * Physics2D.gravity.y * _rb.gravityScale * -2) * _rb.mass;
        Vector2 Verticaldirection = new Vector2(_rb.velocity.x, jumpForce);
        _rb.velocity = Verticaldirection;
        yield return new WaitForSeconds(.1f);
        jumpedClicked = false;
    }

    #endregion

    //This is for when the player changes form it changes the distance of the ray cast.
    public void setGroundDistance(float distanceAmount) { groundCheckerDistance = distanceAmount; }
    public void setUseAbility(bool canUseAbility)
    {
        this.canUseAbility = canUseAbility;
    }

    public bool isGrounded()
    {
        // Shoots a ray cast down and decides whether or not it is true based on if it is hitting an object with the layer mask ground
        RaycastHit2D ray = Physics2D.Raycast(transform.position, -Vector2.up, groundCheckerDistance, groundMask);
        Debug.DrawRay(transform.position, -Vector2.up, Color.green);
        return ray;

    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, -Vector2.up * groundCheckerDistance);
    }
}
