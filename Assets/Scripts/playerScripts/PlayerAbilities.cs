using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public isGroundedScript isGroundedScript { get; private set; }
    private GameManager gm;
    private Rigidbody2D _rb;
    private GameObject audioManager;
    private AudioManagerV2 audioManagerV2;

    #region Dash variables
    [SerializeField] private float DASHPOWERX = 18, DASHPOWERY = 14;
    [SerializeField] private float UNCHANGEDDASHY = 14;
    [SerializeField] private int maxDashes;
    private int dashAmount;
    private bool canUseAbility;
    #endregion

    #region Pogo variables
    private const float SUPERJUMP = 28;
    private IEnumerator stopSliding;

    public bool usedJumpAbility = false;
    [SerializeField] private bool canJumpNextFrame = false;
    public float jumpFrameTimer = 0;
    public float maxJumpFrameTimer;
    public bool recentlyJumped;
    private float rJumpedTimer = 0;

    private bool jumpAgain;

    

    [SerializeField] private float height;

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
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
        audioManagerV2 = audioManager.GetComponent<AudioManagerV2>();
        dashAmount = maxDashes;
        canUseAbility = true;
        jumpAgain = true;
        
    }

    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(transform.position, -Vector2.up * groundCheckerDistance);


        if (canJumpNextFrame)
        {
            jumpFrameTimer += Time.deltaTime;
            if (jumpFrameTimer >= maxJumpFrameTimer)
            {
                canJumpNextFrame = false;
                jumpFrameTimer = 0;
            }
        }

        //These two if statements do the same thing figure out in the future how to combine them
        if (Input.GetKeyDown(gm.playerAbilityKey))
        {
            recentlyJumped = true;
            useFormsAbility();
        }

        if (isGroundedScript.isGrounded() && canJumpNextFrame)
        {
            useFormsAbility();
        }

        if (recentlyJumped)
        {
            rJumpedTimer += Time.deltaTime;
            if (rJumpedTimer >= 0.8f)
            {
                recentlyJumped = false;
                rJumpedTimer = 0;
            }
        }
    }


    public void useFormsAbility()
    {
        if (canUseAbility)
        {
            string formName = playerMovement.getCurForm().formName;
            switch (formName)
            {
                case "Ball":

                    //Will have the dashing ability

                    dashAbility();
                    break;
                case "Pogo":
                    //Will have the mega jump and arms ability
                    canJumpNextFrame = true;
                    // StartCoroutine(newPogoAbliity());

                    if (isGroundedScript.isGrounded())
                    {

                        newJumpAbliity();
                        usedJump = true;
                        //   pogoAbility();
                        Debug.Log(isGroundedScript.isGrounded());
                    }
                    else if (playerMovement.coyoteTimer > 0) {
                        StartCoroutine(JumpCoyoteTimer());
                        newJumpAbliity();
                        Debug.Log("playing");
                    }

                    break;
            }
        }
    }



    #region Ball Ability
    private void dashAbility()
    {
        if (dashAmount > 0)
        {
            StartCoroutine(audioManagerV2.playPlayerSFX("Dashing"));
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
    private IEnumerator JumpCoyoteTimer()
    {
        yield return new WaitForSeconds(.03f);
        playerMovement.coyoteTimer = 0;

    }
    private IEnumerator newPogoAbliity()

    {
        if (isGrounded())
        {
            canJumpNextFrame = false;
            jumpFrameTimer = 0;
            float jumpImpulse = Mathf.Sqrt(height * Physics2D.gravity.y * _rb.gravityScale * -2) * _rb.mass;
            StartCoroutine(audioManagerV2.playPlayerSFX("Jumping"));
            Vector2 Verticaldirection = new Vector2(_rb.velocity.x, jumpImpulse);
            _rb.velocity = Verticaldirection;
            // _rb.AddForce(Verticaldirection);

        }

        yield return new WaitForSeconds(0f);


    }

    private void newJumpAbliity()

    {

        canJumpNextFrame = false;
        jumpFrameTimer = 0;
        float jumpImpulse = Mathf.Sqrt(height * Physics2D.gravity.y * _rb.gravityScale * -2) * _rb.mass;
        Vector2 Verticaldirection = new Vector2(_rb.velocity.x, jumpImpulse);
        _rb.velocity = Verticaldirection;
       



    }
    // public IEnumerator preventSlide()
    // {

    //     yield return new WaitForSeconds(.6f);
    //     yield return new WaitUntil(() => isGroundedScript.isGrounded());

    //     if (playerMovement.isPogo == true)
    //     {

    //         _rb.velocity = Vector3.zero;
    //     }
    // }
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

    public bool getJumpNextFrame() { return canJumpNextFrame; }
    public int getDashAmount() { return dashAmount; }
    public bool GetCanUseAbility()
    {
        return canUseAbility;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, -Vector2.up * groundCheckerDistance);
    }
}
