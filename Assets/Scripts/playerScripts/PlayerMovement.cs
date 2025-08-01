using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{   

    
    private Physics physics;
    [Header("----Physics----")]
    [SerializeField]private float coefficientOfFriction;
    [SerializeField]private float rainyFrictionUp, rainyFrictionDown;

    private PlayerAbilities playerAbility;
    private float horizontalInput;
    //Movement will be set through the forms different scriptables
    private float movementSpeed;
    [SerializeField]private bool isGrounded;

    #region Player Settings
    [Header("----Player----")]
    //temp
    private GameObject audioManager;
    private AudioManagerV2 audioManagerV2;
    //temp
    [SerializeField] private bool canControl;
    [SerializeField]private List<AbilitySettingScriptable> forms;
    private int maxForm, curForm;
    private SpriteRenderer _spriteRender;
    private PlayerAbilities playerAbilities;
    #endregion
    [SerializeField] private GameObject prefabDustSpawner;
    private GameObject _dustSpawner;

    private UnityEvent playerImpact; //player lands on the ground w a certain amount of velocity/momentum

    #region Ball settings
    [Header("Ball Settings")]
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
    private float lastVelocityX;
    private Coroutine turnEasingBackOn;

    #endregion

    #region PogoMovement
    [Header("Pogo Settings")]
    public bool canJumpAgain = true;

    public bool isPogo = false;
    public IEnumerator jumping;

    private IEnumerator hop;
    public bool canJump = true;
    public float jumpSpeedX, jumpSpeedY;

    public float coyoteTimer { get; set; }

    [SerializeField] private float floatTime;
    #endregion
    private float lastVelocityY;
    public float velocitySoundThreshold;
    public bool checkingImpact;
    
    public class MainTouch
    {
        public float fingerID;
        public Vector2 origin;
        public Vector2 touchPos;
        public void setOrigin(Vector2 origin)
        {
            this.origin = origin;
        }
        public void setFingerID(float fingerID)
        {
            this.fingerID = fingerID;
        }
        public float getXDistance()
        {
            return touchPos.x - origin.x;
        }
    }
    public MainTouch mainTouch;
    #region Mobile Settings
    [Header("Mobile Settings")]
    public Vector2 screenSize;
    [SerializeField]public float inputRange; //i think this is in pixels idk bru
    [SerializeField]public float inputDetectionPercentX;
    [SerializeField]private bool visualizeTouchArea;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerImpact = new UnityEvent();
        playerAbilities = GetComponent<PlayerAbilities>();
        _dustSpawner = Instantiate(prefabDustSpawner);
        physics = new Physics(GetComponent<Rigidbody2D>());
        playerImpact.AddListener(_dustSpawner.GetComponent<DustScriptV2>().playLandingParticles);
        _spriteRender = GetComponent<SpriteRenderer>();

        audioManager = GameObject.FindGameObjectWithTag("AudioManager");
        audioManagerV2 = audioManager.GetComponent<AudioManagerV2>();

        forms[curForm].formSetting(physics._rb, _spriteRender, GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
        playerAbility = GetComponent<PlayerAbilities>();
        isEasingOn = true;

        screenSize = new Vector2(Screen.width, Screen.height);
        canControl = true;

        // playerAbilities.isGroundedScript.setStartPosition((Vector2)transform.position + forms[curForm].startPositionOffset);
        // playerAbilities.isGroundedScript.setColSize(forms[curForm].groundChecker);
    }

    // Update is called once per frame
    void Update()
    {
        coyoteTimer -= Time.deltaTime;
        physics.setCoefficientOfFriction(coefficientOfFriction);
        physics.setRainyFrictionUp(rainyFrictionUp);
        physics.setRainyFrictionDown(rainyFrictionDown);

        maxForm = forms.Count - 1;
#if UNITY_ANDROID
        if(canControl){mobileInput();}
#else
        if (canControl) { horizontalInput = Input.GetAxisRaw("Horizontal"); }
#endif
        //This prevents the easing from going above what its supposed to be


        if (isEasingOn)
        {

            if (withEasing)
            {
                if (physics._rb.velocity.x >= -0.1f && physics._rb.velocity.x <= 0.1f)
                {
                    withEasing = false;
                }

                //This piece of code ensures that easing is never on when it doesn't have to be
                //Since the angularvelocity is directyl related to the direction the player is rolling to.
                if (oppositeInput == 1 && physics._rb.angularVelocity < 0)
                {
                    withEasing = false;
                }
                else if (oppositeInput == -1 && physics._rb.angularVelocity > 0)
                {
                    withEasing = false;
                }
            }
            if (withEasing && horizontalInput == oppositeInput)
            {
                Vector2 velocity = physics._rb.velocity;
                //Smoothly brings down the velocity's x to a 0 making it a smooth stop when the player turns.
                velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref curFloat, smoothStopSpeed);
                //This doesn't really do much although it is similar to what Tarin did with the player controller
                angularVelHalf = -(physics._rb.angularVelocity / 2) * 10;
                /*
                This is used for the players rotation in the rigidbody mainly when its a ball. It's supposed to make sure the balls rotation is going 
                the same as the players input however with further inspection this was done with the gravity
                */
                physics._rb.angularVelocity += angularVelHalf * Time.fixedDeltaTime;
                physics._rb.velocity = velocity;
            }
        }

        //Change keybind so the player gets it from the game manager
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            changeForm();
            withEasing = false;
        }

        //Skips the form if it is a add on
        if (forms[curForm].formAddOn)
        {
            changeForm();
        }

        if (forms[curForm].formName == "Pogo")
        {
            isPogo = true;
        }
        else isPogo = false;
        isGrounded = playerAbilities.isGrounded(); //LMAO
        physics.setCoefficientOfFriction(coefficientOfFriction);
        lastVelocityX = physics._rb.velocity.x;
        lastVelocityY = physics._rb.velocity.y;
        if (!playerAbilities.isGrounded() && !checkingImpact)
        {
            StartCoroutine(impactSound());
        }
    }

    
    private void mobileInput()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {    //if maintouch not initialized yet
                if (touch.phase == TouchPhase.Began && mainTouch == null && touch.position.x < screenSize.x * inputDetectionPercentX)
                {
                    mainTouch = new MainTouch();
                    mainTouch.setOrigin(touch.position);
                    mainTouch.setFingerID(touch.fingerId);
                    Debug.Log("Touch started: " + touch.fingerId);
                }
            }   //if maintouch is initialized, update its position
            if (mainTouch != null)
            {
                updateMainTouch();
            }else{
                horizontalInput = 0;
            }
        }
        else
        {
            horizontalInput = 0;
        }
    }
    private void updateMainTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if(touch.fingerId == mainTouch.fingerID)
            {
                if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    mainTouch = null;
                }
                else
                {
                    mainTouch.touchPos = touch.position;
                    horizontalInput = Mathf.Clamp(mainTouch.getXDistance()/inputRange, -1, 1); //screenSize.x * inputRange is the max distance the player can move their finger to get the max input of 1
                }
            }
        }
    }
    private void OnGUI()
    {
        if(visualizeTouchArea){
            GUI.color = new Color(0, 0, 0, 0.1f);
            GUI.DrawTexture(new Rect(0, 0, screenSize.x * inputDetectionPercentX, screenSize.y), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
    void FixedUpdate()
    {
        physics.Friction();
        if (GetComponent<CircleCollider2D>().enabled)
        {

            ballMovement();

        }
        else if (GetComponent<BoxCollider2D>().enabled)
        {
            torsoMovement();
        }
    }
    public float getAcceleration(){
        float aMultiplier; //acceleration multiplier
        if (lastVelocityX < 0 && physics._rb.velocity.x < 0){
            aMultiplier = -1;
        }else{
            aMultiplier = 1;
        }
        float avgAcceleration = aMultiplier * (physics._rb.velocity.x - lastVelocityX)/Time.deltaTime;
        return avgAcceleration;
    }
    public void changeForm()
    {
        if (curForm == maxForm)
        {
            curForm = 0;
        }
        else
        {
            curForm++;
        }

        forms[curForm].formSetting(physics._rb, _spriteRender, GetComponent<CircleCollider2D>(), GetComponent<BoxCollider2D>());
        // playerAbilities.isGroundedScript.setStartPosition((Vector2)transform.position + forms[curForm].startPositionOffset);
        // playerAbilities.isGroundedScript.setColSize(forms[curForm].groundChecker);
    }

    private void ballMovement(){
        physics._rb.AddForce(new Vector2(horizontalInput * movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);

        if(physics._rb.velocity.x <= -maxSpeedPoint || physics._rb.velocity.x >= maxSpeedPoint){
            oppositeInput = -1 * (physics._rb.velocity.x/Mathf.Abs(physics._rb.velocity.x));
            if(horizontalInput != oppositeInput){
                withEasing = true;
            }
        }
        
    }

    private void torsoMovement(){
        //电子游戏 - 人形摇杆 <-death threat
       // OR
        //Or also just use add force and do some corotines(Will probably try this first)
        if (horizontalInput != 0)
        {
            if (playerAbility.isGrounded() && !playerAbility.getJumpNextFrame())
            {
                if (canJump)
                {
                    // jumping = Jump();
                    // StartCoroutine(jumping);
                    hop = hopping();
                    StartCoroutine(hop);

                    canJump = false;
                }
            }
            else
            {
                physics._rb.AddForce(new Vector2(horizontalInput * movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);
            }
        }
    }
    public IEnumerator Jump()
    {
        // Debug.Log("Jumping");
        Vector2 jumpForce = new Vector2(horizontalInput * jumpSpeedX, jumpSpeedY);

        // impulse makes it so it's a strong force happening at once
        physics._rb.AddForce(jumpForce, ForceMode2D.Impulse);

        // physics._rb.MovePosition(new Vector2(2,3));
        //wait .5 seconds before anything
        yield return new WaitForSeconds(.5f);

        //keep checking until the player touches the ground
        yield return new WaitUntil(() => playerAbility.isGrounded());
         canJump = true;
        // yield return new WaitForSeconds(.1f);
        // if(!playerAbility.getJumpNextFrame()){
        //     stopSliding();
        // }

        //and then allow the player to jump again

    }

    public float getHorizontalInput()
    {
        return horizontalInput;
    }
    public IEnumerator hopping()
    {
        coyoteTimer = floatTime;
        Vector2 jumpForce = new Vector2(horizontalInput * jumpSpeedX, jumpSpeedY);
        physics._rb.velocity = jumpForce;
        yield return new WaitForSeconds(.5f);

        //keep checking until the player touches the ground
        yield return new WaitUntil(() => playerAbility.isGrounded());
        canJump = true;
    }
    public IEnumerator impactSound()
    {
        checkingImpact = true;
        yield return new WaitUntil(() => playerAbility.isGrounded());
        if (Mathf.Abs(lastVelocityY) > velocitySoundThreshold)
        {
            //make volume based on velocity
            StartCoroutine(audioManagerV2.playPlayerSFX("Landing"));
            //GetComponent<AudioSource>().Play();
        }
        checkingImpact = false;
    }

    public void setCanControl(bool value) { canControl = value; }
    public float getInput(){return horizontalInput;}
    public void setSpeed(float speed){movementSpeed = speed;}
    public float getMaxSpeedPoint(){return maxSpeedPoint;}
    public Vector2 getCurVelocity(){ return physics._rb.velocity; }
    public void setCurVelocity(Vector2 val){ physics._rb.velocity = val; }
    public int getFormInt() { return curForm; }
    public void setNewForm(AbilitySettingScriptable newForm){forms.Add(newForm);}
    public AbilitySettingScriptable getCurForm(){return forms[curForm];}
    public List<AbilitySettingScriptable> getAllForms(){ return forms; }
    public float getRainyFrictionUp() { return rainyFrictionUp; }
    public float getRainyFrictionDown(){return rainyFrictionDown;}
    public void setRainyFrictionUp(float amount){rainyFrictionUp = amount;}
    public void setRainyFrictionDown(float amount){rainyFrictionDown = amount;}

    //When particles collide with the player it turns on the function slippery shit making it harder for the player to go up
    //but easier to go down.
    void OnParticleCollision(GameObject other)
    {
        //Debug.Log("i think woring");
        if(other.CompareTag("RainShit")){
            isEasingOn = false;
            physics.slipperyShitFunction();
            if(turnEasingBackOn != null) {//This if statement ensures that easing is not turned back on while rain is hiting the player.
                StopCoroutine(turnEasingBackOn);
                turnEasingBackOn = null;
            }
        }
    }
   
    public void setCoefficientOfFriction(float newCOF){ 
        coefficientOfFriction = newCOF;
    }
    public float getCoefficientOfFriction(){ 
        return coefficientOfFriction;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("RainShit")){
            if(turnEasingBackOn == null){
                turnEasingBackOn = StartCoroutine(EasingBackOn());
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground")){
            if (collision.relativeVelocity.y > velocitySoundThreshold)
            {
                
            }
        }
    }
    private IEnumerator EasingBackOn(){
        yield return new WaitForSeconds(2f);
        isEasingOn = true;
    }

}