using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rb;

    private PlayerAbilities playerAbility;
   private float horizontalInput;
    //Movement will be set through the forms different scriptables
    private float movementSpeed;
    [SerializeField]private bool isGrounded;

   

    [SerializeField]private List<AbilitySettingScriptable> forms;
    private int maxForm, curForm;
    private SpriteRenderer _spriteRender;
    private PlayerAbilities playerAbilities;

    private GameObject _dustSpawner;
    public DustScript dustScript;

    #region Ball settings
    [Header("----Ball Settings----")]
    //Used for when the players a ball and makes the turning from fast movement more sudden.
    //The lower the number the faster it stops
    [SerializeField]private float smoothStopSpeed;
    [SerializeField]private float maxSpeedPoint;
    [SerializeField]private float coefficientOfFriction;
    private float oppositeInput;
    //Only used for a ref for the smoothDamp variable
    private float curFloat;
    [SerializeField]private bool withEasing;
    [SerializeField]private float angularVelHalf;
    private float lastVelocityX;
    #endregion

    #region PogoMovement
    public bool canJumpAgain = true;
    public IEnumerator jumping;
    public bool canJump = true;
    public float jumpSpeedX,jumpSpeedY;
    private 
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerAbilities = GetComponent<PlayerAbilities>();
        _dustSpawner = GameObject.FindGameObjectWithTag("Dust");
        dustScript = _dustSpawner.GetComponent<DustScript>();
        _spriteRender = GetComponent<SpriteRenderer>();
        forms[curForm].formSetting(_rb, _spriteRender, GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
        playerAbility = GetComponent<PlayerAbilities>();
    }

    // Update is called once per frame
    void Update()
    {  
      
           maxForm = forms.Count-1;
        horizontalInput = Input.GetAxisRaw("Horizontal");

        //This prevents the easing from going above what its supposed to be
        if(withEasing){
            if(_rb.velocity.x >= -0.1f && _rb.velocity.x <= 0.1f){
                withEasing = false;
            }

            //This piece of code ensures that easing is never on when it doesn't have to be
            //Since the angularvelocity is directyl related to the direction the player is rolling to.
            if(oppositeInput == 1 && _rb.angularVelocity < 0){
                withEasing = false;
            } else if(oppositeInput == -1 && _rb.angularVelocity > 0){
                withEasing = false;
            }
        }

        if(withEasing && horizontalInput == oppositeInput){
            Vector2 velocity = _rb.velocity;
            //Smoothly brings down the velocity's x to a 0 making it a smooth stop when the player turns.
            velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref curFloat, smoothStopSpeed);
            //This doesn't really do much although it is similar to what Tarin did with the player controller
            angularVelHalf = -(_rb.angularVelocity/2) * 10;
            /*
            This is used for the players rotation in the rigidbody mainly when its a ball. It's supposed to make sure the balls rotation is going 
            the same as the players input however with further inspection this was done with the gravity
            */
            _rb.angularVelocity += angularVelHalf * Time.fixedDeltaTime;
            _rb.velocity = velocity;
        }

        //Change keybind so the player gets it from the game manager
        if(Input.GetKeyDown(KeyCode.LeftShift)){
            changeForm();
            withEasing = false;
        }

        //Skips the form if it is a add on
        if(forms[curForm].formAddOn){
            changeForm();
        }
    }

    void FixedUpdate()
    {
        isGrounded = playerAbilities.isGrounded();
        Friction();
        if(GetComponent<CircleCollider2D>().enabled){
            ballMovement();
            dustScript.checkForDust();
            
        }else if(GetComponent<BoxCollider2D>().enabled){
            torsoMovement();
        }
        lastVelocityX = _rb.velocity.x;
    }
    public float getAcceleration(){
        float aMultiplier; //acceleration multiplier
        if (lastVelocityX < 0 && _rb.velocity.x < 0){
            aMultiplier = -1;
        }else{
            aMultiplier = 1;
        }
        float avgAcceleration = aMultiplier * (_rb.velocity.x - lastVelocityX)/Time.deltaTime;
        return avgAcceleration;
    }
    private void changeForm(){
        if(curForm == maxForm){
            curForm = 0;
        }else{
            curForm++;
        }

        forms[curForm].formSetting(_rb, _spriteRender, GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
    }

    private void ballMovement(){
        _rb.AddForce(new Vector2(horizontalInput * movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);

        if(_rb.velocity.x <= -maxSpeedPoint || _rb.velocity.x >= maxSpeedPoint){
            oppositeInput = -1 * (_rb.velocity.x/Mathf.Abs(_rb.velocity.x));
            if(horizontalInput != oppositeInput){
                withEasing = true;
            }
        }
        
    }

    private void torsoMovement(){
    

        //Or also just use add force and do some corotines(Will probably try this first)
        if (horizontalInput != 0)
                {
                    if (playerAbility.isGrounded())
                    {
                        if (canJump)
                        {   
                            jumping = Jump();
                            StartCoroutine(jumping);
                           
                            canJump = false;
                        }
                    }
                    else{
                        _rb.AddForce(new Vector2(horizontalInput * movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);
                    }
                }

    }
public IEnumerator Jump() 
    {   
        Vector2 jumpForce = new Vector2(horizontalInput * jumpSpeedX, jumpSpeedY);
        //impulse makes it so it's a strong force happening at once
        _rb.AddForce(jumpForce, ForceMode2D.Impulse);

        jumpAgainNo();
        //wait .5 seconds before anything
        yield return new WaitForSeconds(.5f);
        
        //keep checking until the player touches the ground
		yield return new WaitUntil (() => playerAbility.isGrounded());
        jumpAgainYes();
        stopSliding();
        //and then allow the player to jump again
		canJump = true;
    }

    public Boolean jumpAgainNo(){
        return canJumpAgain = false;
    }
    public Boolean jumpAgainYes(){
        return canJumpAgain = true;
    }
    public void stopSliding(){
        if (canJumpAgain==true){
            if (playerAbility.isGrounded()){
            _rb.velocity = Vector3.zero;
            }

        }
        
    }

    public float getInput(){return horizontalInput;}
    public void setSpeed(float speed){movementSpeed = speed;}
    public int getFormInt(){return curForm;}
    public void setNewForm(AbilitySettingScriptable newForm){forms.Add(newForm);}
    public AbilitySettingScriptable getCurForm(){return forms[curForm];}

    void Friction()
    {
        // Air resistance opposes motion but in ball motion is reversed because rotation
        // Grabs the sign of velocity and multiplies it by -1 to get opposite
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(_rb.velocity.x / Mathf.Abs(_rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(_rb.velocity.y / Mathf.Abs(_rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        _rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfFriction * Mathf.Abs(_rb.velocity.x * _rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfFriction * Mathf.Abs(_rb.velocity.y * _rb.velocity.y)));
    }
   
    
}
