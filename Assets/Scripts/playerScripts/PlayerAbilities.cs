using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private isGroundedScript isGroundedScript;
    private GameManager gm;
    private Rigidbody2D _rb;

    #region Dash variables
    private const float DASHPOWERX = 18, DASHPOWERY = 14;
    [SerializeField]private int maxDashes;
    private int dashAmount;
    private bool canUseAbility;
    #endregion

    #region Pogo variables
    private const float SUPERJUMP = 40;
    private IEnumerator stopSliding;
   
    public bool usedJumpAbility =false;
    [SerializeField]private bool canJumpNextFrame = false;
    private float jumpFrameTimer = 0;
    public float maxJumpFrameTimer;
    public bool recentlyJumped;
    private float rJumpedTimer = 0;
    
    #endregion

    [SerializeField]private float groundCheckerDistance;
    [SerializeField]private LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        isGroundedScript = GameObject.FindGameObjectWithTag("GroundRay").GetComponent<isGroundedScript>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        dashAmount = maxDashes;
        canUseAbility = true;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, -Vector2.up*groundCheckerDistance);

        if(canJumpNextFrame){
            jumpFrameTimer += Time.deltaTime;
            if(jumpFrameTimer >= maxJumpFrameTimer){
                canJumpNextFrame = false;
                jumpFrameTimer = 0;
            }
        }
    
        if(Input.GetKeyDown(gm.playerAbilityKey) && canUseAbility){
            recentlyJumped = true;
            useFormsAbility();
        }

        if(isGroundedScript.isGrounded() && canJumpNextFrame){
            useFormsAbility();
        }
        if (recentlyJumped){
            rJumpedTimer += Time.deltaTime;
            if(rJumpedTimer >= 0.8f){
                recentlyJumped = false;
                rJumpedTimer = 0;
            }
        }
    }

    public void useFormsAbility(){
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
                if(isGroundedScript.isGrounded()){
                    pogoAbility();
                    // if(usedJumpAbility==true){
                     
                    //     usedJumpAbility=false;
                    // }
                }
                break;
        }
    }



    #region Ball Ability
    private void dashAbility(){
        if(dashAmount > 0){
            _rb.velocity = Vector2.zero;
            //based of the horizontal input -1, 0, 1
            //0 will now only go up might be good for more movement combinations?
            _rb.AddForce(new Vector2(playerMovement.getInput() * DASHPOWERX, DASHPOWERY), ForceMode2D.Impulse);
            dashAmount--;
            if(dashAmount == 0){StartCoroutine(dashAmountBack());}
        }
    }

    private IEnumerator dashAmountBack(){
        //This function puts a short cool down when getting your dash back since if this isnt here it will
        //automatically give your dash allowing double dash
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isGroundedScript.isGrounded());
        dashAmount = maxDashes;
    }


    #endregion

    #region Pogo Ability
    private void pogoAbility(){
        canJumpNextFrame = false;
        jumpFrameTimer = 0;
        _rb.AddForce(new Vector2(0, SUPERJUMP), ForceMode2D.Impulse);
        // usedJumpAbility = true;
        stopSliding = preventSlide();
        StartCoroutine(stopSliding);
        
    }

    public IEnumerator preventSlide(){
        
        yield return new WaitForSeconds(.6f);
        yield return new WaitUntil(() => isGroundedScript.isGrounded());
          
        if (playerMovement.isPogo==true){

            _rb.velocity = Vector3.zero;
        }
    }
    #endregion

    //This is for when the player changes form it changes the distance of the ray cast.
    public void setGroundDistance(float distanceAmount){groundCheckerDistance = distanceAmount;}
    public void setUseAbility(bool canUseAbility)
    { 
        this.canUseAbility = canUseAbility;
    }

    public Boolean isGrounded()
    {
        //Shoots a ray cast down and decides whether or not it is true based on if it is hitting an object with the layer mask ground
        RaycastHit2D ray = Physics2D.Raycast(transform.position, -Vector2.up, groundCheckerDistance, groundMask); 
        return ray;
    }

    public bool getJumpNextFrame(){return canJumpNextFrame;}


}
