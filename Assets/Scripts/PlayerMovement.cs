using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : Physics
{
    private PlayerAbilities playerAbility;
    private float horizontalInput;
    //Movement will be set through the forms different scriptables
    private float movementSpeed;

    [Header("                                                             PLAYER                                                             ")]
    [SerializeField]private List<AbilitySettingScriptable> forms;
    private int maxForm, curForm;
    private SpriteRenderer _spriteRender;

    #region Ball settings
    [Header("----Ball Settings----")]
    //Used for when the players a ball and makes the turning from fast movement more sudden.
    //The lower the number the faster it stops
    [SerializeField]private float smoothStopSpeed;
    [SerializeField]private float maxSpeedPoint;
    private float oppositeInput;
    //Only used for a ref for the smoothDamp variable
    private float curFloat;
    //WithEasing is only used in the code to control whether the player should be easing or not
    private bool withEasing;
    //IsEasing is a bool that can turn on or off the mechanic easing - this is mainly used for when the player is more slippery
    [SerializeField]private bool isEasingOn;
    [SerializeField]private float angularVelHalf;
    private Coroutine turnEasingBackOn;

    #endregion

    #region PogoMovement
    [Header("----Pogo Settings----")]
    public bool canJumpAgain = true;

    public bool isPogo = false;
    public IEnumerator jumping;
    public bool canJump = true;
    public float jumpSpeedX, jumpSpeedY;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRender = GetComponent<SpriteRenderer>();
        forms[curForm].formSetting(_rb, _spriteRender, GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
        playerAbility = GetComponent<PlayerAbilities>();
        isEasingOn = true;
    }

    // Update is called once per frame
    void Update()
    {  
      
        maxForm = forms.Count-1;
        horizontalInput = Input.GetAxisRaw("Horizontal");

        //This prevents the easing from going above what its supposed to be

        if(isEasingOn){

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

        if(forms[curForm].formName== "Pogo"){
            isPogo = true;
        }
        else isPogo = false;
    }

    void FixedUpdate()
    {
        Friction();
        if(GetComponent<CircleCollider2D>().enabled){
            
            ballMovement();
        }else if(GetComponent<BoxCollider2D>().enabled){
            
            torsoMovement();
        }
        
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
        
        
        //wait .5 seconds before anything
        yield return new WaitForSeconds(.5f);
        
        //keep checking until the player touches the ground
		yield return new WaitUntil (() => playerAbility.isGrounded());
        stopSliding();
        
        //and then allow the player to jump again
		canJump = true;
    }

  
    public void stopSliding(){
        //it now detects when its pogo. If switched to ball ability should be cancelledd
        if (isPogo==true){
             
            if (playerAbility.isGrounded() /*&& playerAbility.usedJumpAbility ==false*/){
             
            _rb.velocity = new Vector2(0, 0);
            }

        }
        
    }

    public float getInput(){return horizontalInput;}
    public void setSpeed(float speed){movementSpeed = speed;}
    public int getFormInt(){return curForm;}
    public void setNewForm(AbilitySettingScriptable newForm){forms.Add(newForm);}
    public AbilitySettingScriptable getCurForm(){return forms[curForm];}

    //When particles collide with the player it turns on the function slippery shit making it harder for the player to go up
    //but easier to go down.
    void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("RainShit")){
            isEasingOn = false;
            slipperyShitFunction();
            if(turnEasingBackOn != null) {//This if statement ensures that easing is not turned back on while rain is hiting the player.
                StopCoroutine(turnEasingBackOn);
                turnEasingBackOn = null;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(!collision.gameObject.CompareTag("RainShit")){
            if(turnEasingBackOn == null){
                turnEasingBackOn = StartCoroutine(EasingBackOn());
            }
        }
    }

    private IEnumerator EasingBackOn(){
        yield return new WaitForSeconds(2f);
        isEasingOn = true;
    }

}
