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
    private HingeJoint2D arms;
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


    public bool usedJumpAbility = false;
    [SerializeField] private bool canJumpNextFrame = false;
    public float jumpFrameTimer = 0;
    public float maxJumpFrameTimer;
    public bool recentlyJumped;


    private bool jumpAgain;

    public RaycastHit2D groundThingyMajiggy { get; private set; }
    public bool jumpedClicked;

    [SerializeField] private float Jumpheight;

    #endregion

    #region Arm variables
    [Header("Arms Variables")]

    [SerializeField] private HingeJoint2D hinge;
    private Collider2D armCol;
    [SerializeField] private LayerMask vineLayer;
    [SerializeField] private float armColRadius;

    public bool isConnected;

    [SerializeField] private float boostX, boostY, grabBoostX, grabboostY;//For the boost of the swing
    [SerializeField] private Vector2 rightSide, leftSide;
    private Vector2 side;//The offical position of where the hinge anchor is going to be

    #endregion

    [SerializeField] private float groundCheckerDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool hasArms;
    public bool usedJump;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        isGroundedScript = GameObject.FindGameObjectWithTag("GroundRay").GetComponent<isGroundedScript>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        arms = GetComponent<HingeJoint2D>();
        arms.enabled = false;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
        audioManagerV2 = audioManager.GetComponent<AudioManagerV2>();
        dashAmount = maxDashes;
        canUseAbility = true;
        jumpAgain = true;
        jumpedClicked = false;
        hasArms = true;
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
        if (canUseAbility)
        {
            string formName = playerMovement.getCurForm().formName;
            if (!playerMovement.currentVine)
            {
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
                        else if (playerMovement.coyoteTimer > .56 && playerMovement.coyoteTimer < .65)
                        {
                            StartCoroutine(JumpCoyoteTimer());

                            StartCoroutine(JumpAbility());

                        }
                        else
                        {
                            //not grounded, so at this point the only thing ability key will do is potentially grab vines
                            Debug.Log("Getting ran?");
                            if (hasArms)
                            {
                                checkForVines();
                            }
                        }

                        break;
                }
            }
            else
            {
                detach();
            }
        }
    }

    private void checkForVines()
    {
        Debug.Log("inside check for vines");
        Collider2D collider = Physics2D.OverlapBox(gameObject.transform.position, GetComponent<BoxCollider2D>().bounds.size, 0f, LayerMask.GetMask("Vine"));
        if (collider)
        {
            arms.enabled = true;
            arms.connectedBody = collider.gameObject.GetComponent<Rigidbody2D>(); //connect arms hinge to the vine segment
            playerMovement.currentVine = collider.transform.parent;
        }
    }
    private void detach()
    {
        arms.connectedBody = null;
        arms.enabled = false;
        playerMovement.currentVine = null;
    }


    #region Ball Ability
    private void dashAbility()
    {
        if (dashAmount > 0)
        {
            StartCoroutine(audioManagerV2.playPlayerSFX("Dashing"));
            //based of the horizontal input -1, 0, 1
            //0 will now only go up might be good for more movement combinations?
            var horInput = playerMovement.getInput();

            if (horInput != 0)
            {
                _rb.velocity = Vector2.zero;
                _rb.AddForce(new Vector2(horInput * DASHPOWERX, DASHPOWERY), ForceMode2D.Impulse);
            }

            if (horInput == 0)
            {

                // _rb.AddForce(new Vector2(horInput * DASHPOWERX, UNCHANGEDDASHY), ForceMode2D.Impulse);
                _rb.AddForce(new Vector2(_rb.velocity.x / 100, UNCHANGEDDASHY), ForceMode2D.Impulse);
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
    public void setAbilityPower(float dashX, float dashY, float megaJump)
    {
        DASHPOWERX = dashX;
        DASHPOWERY = dashY;
        UNCHANGEDDASHY = dashY;
        Jumpheight = megaJump;
    }
    public Vector3 getAbilityPower()
    {
        return new Vector3(DASHPOWERX, DASHPOWERY, Jumpheight);
    }
    public bool isGrounded()
    {
        //im gonna fucking kill myslef
        // Shoots a ray cast down and decides whether or not it is true based on if it is hitting an object with the layer mask ground
        groundThingyMajiggy = Physics2D.Raycast(transform.position, -Vector2.up, groundCheckerDistance, groundMask);
        Debug.DrawRay(transform.position, -Vector2.up, Color.green);
        return groundThingyMajiggy;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, -Vector2.up * groundCheckerDistance);
    }
    public bool GetCanUseAbility()
    {
        return canUseAbility;
    }
    public int getDashAmount()
    {
        return dashAmount;
    }
    
    #region Arm Ability

    private void Grab(){

        if (armCol != null)
        {
            if (IsVineDirRight(armCol.transform.position))
            {
                side = rightSide;
            }
            else{
                side = leftSide;
            }

            if (!isConnected)
            {
                if (playerMovement.getFormInt() == 1)
                {
                    _rb.freezeRotation = true;
                }
            }
            else
            {
                _rb.freezeRotation = false;
            }
        }
    }

    IEnumerator Swinging(){
        if (!isConnected)
        {
            if (!isGrounded())
            {
                playerMovement.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
                hinge.enabled = true;
                hinge.autoConfigureConnectedAnchor = false;
                hinge.useLimits = true;
                Vector2 vec = armCol.GetComponent<vinetest>().transformTest.localPosition;
                hinge.connectedBody = armCol.GetComponent<Rigidbody2D>();
                hinge.anchor = side;
                hinge.connectedAnchor = vec;
                armCol.GetComponent<vinetest>().onVine = true;
                _rb.AddForce(new Vector2(playerMovement.getHorizontalInput() * grabBoostX, grabboostY), ForceMode2D.Impulse);
                isConnected = !isConnected;
                    
                yield return new WaitForEndOfFrame();
                StopCoroutine(Swinging());
            }
        }
        else
        {

            hinge.connectedBody = null;
            hinge.enabled = false;
            _rb.AddForce(new Vector2(playerMovement.getHorizontalInput() * boostX, boostY), ForceMode2D.Impulse);
            gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            armCol.GetComponent<vinetest>().onVine = false;
            isConnected = !isConnected;
            StopCoroutine(Swinging());
        }
    }

    bool IsVineDirRight(Vector2 vine){
        if (vine.x > transform.position.x)
        {
            return true;
        }

        return false;
    }

    void FixedUpdate() {
        armCol = Physics2D.OverlapCircle(transform.position + new Vector3(0, .5f, 0), armColRadius, vineLayer);
    }


    #endregion
}
