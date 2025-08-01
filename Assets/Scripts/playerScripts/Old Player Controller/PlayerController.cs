using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using System.Threading;

public class PlayerController : MonoBehaviour
{

    #region General
    [Header("General")]
    Abilities abilityScript;
    public KeyCode formChangeKey;
    public KeyCode rightformChangeKey;
    public bool devControl;//Just used to override the locked forms(I got really lazy and I dont want to keep going back and fourth changing the bools)
    // public int neareastSpawner;
    //public Transform spherePoint;
    public TestManager gm;
    //Reference to the players sprite render component
    [SerializeField]private SpriteRenderer playerSpriteRender;
    private GameObject player;

    //The different sprites used for the player
    [SerializeField]private Sprite[] playerFormSprite;
    //[SerializeField]private Animator anim;
    //public float jumpTime;
    public AudioManagerScript AMS;

    //public Collider2D circleCol; // checks for all colliders //NOTE - do we need this?
    public Collider2D vineCol;
    

    public GameObject spawner;
    public GameObject grabOn;

    //I believe we should thy fix this and put it into its own class for playing sounds
    IEnumerator playingSound;
    private bool soundIsPlaying;
    [SerializeField]public CamControllerV2 cam;//NOTE - Change script name and get rid of old cam controller
    #endregion

    #region movements
    [Header("Movement")]
    public bool canMove = true;
    [SerializeField]private bool isMoving;
    public Rigidbody2D rb;
    public float horizontal;
    //Mainly used for dashing and taking the last input when the player isnt pressing anything 
    //DOESNT FUCKING WORK
    public int horiLatestInput = 1;
    public float speed,jumpSpeedX,jumpSpeedY;
    [SerializeField]private float bonusRotationSpeed;
    [SerializeField]private float rotChangePointMax;//The max amount rb rotation can get to before giving a boost when changing dir
    [SerializeField]private float rotChangePointMin;//The minimal amount rb rotation can get to before stoping the boost when changing dir
    private bool canBoostRotSpeed;

    [SerializeField] public GameObject dustParticles;
    [SerializeField] DustMovement dustMovement;

    [Header("Interaction")]
    private Collider2D interactCol;
    [SerializeField]public float interactRadius;
    [SerializeField]public LayerMask interactMask, groundMask;//interact mask is for objects you can interact with by pressing E. Ground is for ground
    [Header("Respawn variables(Level 3)")]
    [SerializeField]public float respawnRadius;
    [SerializeField]public LayerMask respawnMask;//interact mask is for objects you can interact with by pressing E. Ground is for ground
    
    
    [Header("Player Forms")]
    public int maxForm;
    public int curForm;
    public enum playerForms{Ball, Pogo}
    public static playerForms playerForm;
    // Change to false false false for game and initialize in gm
    public static bool[] playerPieces = {true, false};//bools for the player pieces {0: ball, 1: pogo, 2: arm}

    [Header("Physics")]
    public bool ignoreResistences = false;
    [SerializeField] float coefficientOfAirResistence, coefficientOfFriction;
    public isGroundedScript groundedScript;

    #region Ball movement variables

    [Header("Head")]
    [SerializeField]private Collider2D ballCol;

    #endregion

    #region Pogo movement variables

    [Header("Body")]
    [SerializeField]private Collider2D pogoCol;
    public IEnumerator jumping;
    public bool canJump = true;
    
	#endregion

	#region Arm movement variables

    [Header("Arm")]
    [SerializeField]private GameObject leftArm;
    [SerializeField]private GameObject rightArm;
    public bool hasArms;

	#endregion


	#endregion

    private bool formChanged;

	private void Start(){
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<TestManager>();
        if (gm == null)
        {
            return;
        }
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControllerV2>();
        groundedScript = GameObject.FindGameObjectWithTag("GroundRay").GetComponent<isGroundedScript>();
        playerSpriteRender = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        dustParticles = GameObject.FindGameObjectWithTag("Dust");
        dustMovement = dustParticles.GetComponent<DustMovement>();
        abilityScript = GetComponent<Abilities>();
        //anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerForm = playerForms.Ball;
        FormSettings();
        AMS = GameObject.Find("AudioManager").GetComponent<AudioManagerScript>();

        if (!devControl)
        {
            playerPieces[0] = true;
            playerPieces[1] = false;
        }
    }

        
    // Update is called once per frame
    void Update()
    {

        if (canMove) 
        {
            if(!gm.isMobileControls){
                horizontal = Input.GetAxisRaw("Horizontal");
            }else{
                horizontal = MobileJoystick.instance.mobileInput();
            }

            Movements();
        }

        if (Input.GetKeyDown(formChangeKey) || Input.GetKeyDown(rightformChangeKey) || formChanged)
        {//NOTE - Looks weirdly complicated try to recode this
            if (!devControl)
            {
               
                if (curForm >= maxForm)
                {
                    curForm = 0;
                }
                else
                {
                    curForm++;
                }
                ChangeForm(curForm);//Controlls the changing of the players form
            }
            else
            {
                curForm++;
                if (curForm >= 2)//Had to change for current build
                {
                    curForm = 0;
                }
                ChangeForm(curForm);//Controlls the changing of the players form
            }
        }

        LatestInput((int)horizontal);
        
    }

    public void formChangeButton(){
        if(!formChanged){
            formChanged = true;
        }
    }

    private void FixedUpdate() {

        if (canMove){

            if (!ignoreResistences)
            {
                if (groundedScript.isGrounded())
                {
                    Friction();
                }
                else
                {
                    AirResistance();
                }
            }
        }

        if (rb.velocity.x > 1.5f || rb.velocity.x < -1.5)//the game will register if it is moving when its past a certain speed
        {
            isMoving = true;//only used for the sfx of moving in the grass.
        }
        else
        {
            isMoving = false;
        }

        
    }

    private void LatestInput(int horizontalInput){//Finds the latest input for horizontal
        if (horizontalInput != 0)
        {
            int i = horizontalInput;
            horiLatestInput = i;
        }
        else
        {
            horiLatestInput = 0;
        }
    }

    public void ChangeForm(int playerFormNum)
    {
        formChanged = false;
        playerForm = (playerForms)playerFormNum;
        FormSettings();

    }
    // NOTE - Could this be turned into struct? Research more about structs
    void FormSettings(){//defualt settings for each form(mainly for the sprites of each form)
            switch (playerForm)
            {
                case playerForms.Ball:
                    //Sets the balls sprite, unfreezes rotation, and changes the animation
                    curForm = 0;
                    rb.mass = 1;
                    ballCol.enabled = true;
                    pogoCol.enabled = false;
                    playerSpriteRender.sprite = playerFormSprite[0];
                    //anim.enabled = false;
                    rb.freezeRotation = false;
                    try
                    {
                        leftArm.SetActive(false);
                        rightArm.SetActive(false);
                    }
                    catch (System.Exception)
                    {
                        
                        Debug.LogError("Left Arm and Right arm are not assigned. If not using them do not mind this message");
                        return;
                    }
                    break;

                case playerForms.Pogo:
                    curForm = 1;
                    rb.mass = 1f;
                    rb.freezeRotation = true;
                    transform.rotation = quaternion.RotateZ(0);//Puts the character up straight
                    ballCol.enabled = false;//changes the collider from ball to pogo
                    pogoCol.enabled = true;
                    //anim.enabled = true;
                    playerSpriteRender.sprite = playerFormSprite[1];//changes the sprites from ball to pogo man
                    //anim.SetInteger("Horizontal", (int)horizontal);//this is for walking animation 
                    canJump = true;
                    if (hasArms)
                    {
                        try
                        {
                            leftArm.SetActive(true);
                            rightArm.SetActive(true);
                        }
                        catch (System.Exception)
                        {
                            Debug.LogError("It seems as though you are trying to use arms however YOU DO NOT HAVE THE ARMS IN THE VARIABLE GAMEOBECT CALLED LEFT ARM AND RIGHT ARM - Elyjah Justice Logan");
                            return;
                        }
                    }
                    break;
            }
        }
        

    private void Movements()
    {//different movements for each form
		switch (playerForm)
		{
			case playerForms.Ball:
                rb.AddForce(new Vector2(horizontal * speed * Time.deltaTime, 0), ForceMode2D.Impulse);//moves the player in the direction the player is pressing
                RotationSpeed();
                break;
			case playerForms.Pogo:
                if (horizontal != 0)
                {
                    if (groundedScript.isGrounded())
                    {
                        if (canJump)
                        {
                            jumping = Jump();
                            StartCoroutine(jumping);
                            canJump = false;
                        }
                    }
                    else{
                        rb.AddForce(new Vector2(horizontal * speed * Time.deltaTime, 0), ForceMode2D.Impulse);
                    }
                }
                break;
			default:
				break;
		}

	}

    public IEnumerator Jump() 
    {
        Vector2 jumpForce = new Vector2(horizontal * jumpSpeedX, jumpSpeedY);
        rb.AddForce(jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(.5f);
		yield return new WaitUntil (() => groundedScript.isGrounded());
		canJump = true;
    }

    void AirResistance()
    {
        // Air resistance opposes motion
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(rb.velocity.x / Mathf.Abs(rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(rb.velocity.y / Mathf.Abs(rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        //CoeAR = 0.075 
        rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfAirResistence * (rb.velocity.x * rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfAirResistence * (rb.velocity.y * rb.velocity.y)));
    }
    void Friction()
    {
        // Air resistance opposes motion but in ball motion is reversed because rotation
        // Grabs the sign of velocity and multiplies it by -1 to get opposite
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(rb.velocity.x / Mathf.Abs(rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(rb.velocity.y / Mathf.Abs(rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfFriction * Mathf.Abs(rb.velocity.x * rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfFriction * Mathf.Abs(rb.velocity.y * rb.velocity.y)));
    }

    public void setRespawn(GameObject spawner){
        this.spawner = spawner;
    }

    private IEnumerator PlaySound(float waitAmount){//Plays the sound and waits until it is finished + however amount you want to add
        AMS.sfx.clip = AMS.currentSfx;
        AMS.sfx.Play();
        yield return new WaitForSeconds(AMS.sfx.clip.length + waitAmount);
        soundIsPlaying = false;
    }

    private void RotationSpeed(){

        bonusRotationSpeed = -(rb.angularVelocity/2);
        
        if (!groundedScript.isGrounded())
        {
            if(horizontal == 1){
                if (rb.angularVelocity > 0.02f)
                {
                    rb.angularVelocity -= -bonusRotationSpeed * Time.fixedDeltaTime * 10f;
                }
            }else if(horizontal == -1){
                if (rb.angularVelocity < -0.02f)
                {
                    rb.angularVelocity += bonusRotationSpeed * Time.fixedDeltaTime * 10f;
                }
            }
        }
        
        
        switch (horizontal)
        {
            case 1:

                if(canBoostRotSpeed){
                    if (rb.angularVelocity < rotChangePointMin){
                        rb.angularVelocity -= -bonusRotationSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        canBoostRotSpeed = false;
                    }
                }

                if (rb.angularVelocity < -rotChangePointMax)
                {
                    canBoostRotSpeed = true;
                }
                break;

            case -1:

                if (canBoostRotSpeed)
                {
                    if(rb.angularVelocity > rotChangePointMin){
                        rb.angularVelocity += bonusRotationSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        canBoostRotSpeed = false;
                    }
                }

                if (rb.angularVelocity > rotChangePointMax)
                {
                    canBoostRotSpeed = true;
                }
                break;
            
        }
    }

    // private void OnDrawGizmos()  
    // {
    //     Gizmos.DrawWireSphere(spherePoint.transform.position, respawnRadius);
    // }

    private void OnCollisionEnter2D(Collision2D collision)
	{
        if(collision.gameObject.tag == "Spike"){
            Debug.Log("dead");
            StartCoroutine(PlayDead());
            rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;
        }
	}

    private void OnTriggerEnter2D(Collider2D other) {//For level 3 death valley
        if(other.gameObject.tag == "Spike"){
            Debug.Log("dead");
            StartCoroutine(PlayDead());
            //rb.velocity = Vector3.zero;
            rb.angularVelocity = 0;
        }
    }

    //fix this add it to its own class for sfx
	private void OnTriggerStay2D(Collider2D collision)
	{
        
        switch (collision.tag)
        {
            case "sfx":
                if (!soundIsPlaying && isMoving)
                {
                    playingSound = PlaySound(0.6f);
                    StartCoroutine(playingSound);
                    soundIsPlaying = true;
                }
                break;
            
        }

    }

	private void OnTriggerExit2D(Collider2D collision)
	{
        if (collision.tag == "sfx")
        {
            StopCoroutine(playingSound);
            soundIsPlaying = false;
        }

	}

    public IEnumerator PlayDead() 
    {
        canMove = false;
        StartCoroutine(gm.RespawnLevel3());
        yield return new WaitUntil(() => groundedScript.isGrounded());
        canMove = true;

    }


}