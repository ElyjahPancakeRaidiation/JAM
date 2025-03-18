using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    private PlayerMovement playerMovement;
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
    #endregion

    [SerializeField]private float groundCheckerDistance;
    [SerializeField]private LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        dashAmount = maxDashes;
        canUseAbility= true;
    }

    // Update is called once per frame
    void Update()
    {
    
        if(Input.GetKeyDown(gm.playerAbilityKey) /*&& canUseAbility*/){
            useFormsAbility(playerMovement.getCurForm().formName);
        }
    
    }

    private void useFormsAbility(string formName){
        switch (formName)
        {
            case "Ball":
                //Will have the dashing ability
                dashAbility();
                break;
            case "Pogo":
                //Will have the mega jump and arms ability
                if(isGrounded()){
                    pogoAbility();
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
        yield return new WaitUntil(() => isGrounded());
        dashAmount = maxDashes;
    }


    #endregion

    #region Pogo Ability
    private void pogoAbility(){
        _rb.AddForce(new Vector2(0, SUPERJUMP), ForceMode2D.Impulse);
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


}
