using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustScript : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public PlayerMovement playerMovement;
    private PlayerAbilities playerAbilities;
    public float yOffset;
    public float xOffset;
    private float horizontalInput;
    private GameManager gm;
    private ParticleSystem dustParticles;
    
    private ParticleSystem.VelocityOverLifetimeModule baseVelocity;
    private float xSpeed;
    private float ySpeed;
    [SerializeField] private bool shouldSkid;
    [SerializeField] private float skidSpeed;
    [SerializeField] private float emissionMultiplier;
    [SerializeField] private float timeDelay;
    [SerializeField] private bool jumpedWhileSkidding;
     public float rotationSpeed = 10f;
    public LayerMask groundMask;
    private bool generatingDust;

    [SerializeField]

    void Start()
    {
        dustParticles = GetComponent<ParticleSystem>();
        baseVelocity = dustParticles.velocityOverLifetime;
        xSpeed = baseVelocity.xMultiplier;
        ySpeed = baseVelocity.yMultiplier; 

        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAbilities = player.GetComponent<PlayerAbilities>();
        rb = player.GetComponent<Rigidbody2D>();

        yOffset = player.GetComponent<CircleCollider2D>().radius;
        xOffset = player.GetComponent<CircleCollider2D>().radius*2.1f;
    }
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        xOffset = Math.Abs(xOffset) * horizontalInput * -1;
        if(playerAbilities.isGrounded()){
            if(!jumpedWhileSkidding && playerMovement.getCurForm().formName == "Ball"){
                transform.position = new Vector2(player.transform.position.x - xOffset, player.transform.position.y - yOffset);
            }
        }
        surfaceAlignment();
    }

    private void surfaceAlignment()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, 2.0f, groundMask);
        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSpeed);
        }
    }

    public void checkForDust(){
        float speed = Math.Abs(rb.velocity.x);
        if(speed >= skidSpeed && playerAbilities.isGrounded() && playerAbilities.recentlyJumped == false){
            shouldSkid = true;
            if (playerMovement.getAcceleration() < 0 && ((horizontalInput == -1 && rb.velocity.x > 0) || (horizontalInput == 1 && rb.velocity.x < 0)) && !generatingDust && playerMovement.getCurForm().formName == "Ball"){
                StartCoroutine(createDust(horizontalInput));
                dustParticles.Play();
            }
        }
        if ((-1.5f < speed && speed < 1.5f) || playerMovement.getAcceleration() > 0 || rb.velocity.x/Math.Abs(rb.velocity.x) == horizontalInput || playerMovement.getCurForm().formName != "Ball"){
            // Debug.Log("Stopping dust particles");
            shouldSkid = false;
            dustParticles.Stop();
        }
    }
    private IEnumerator createDust(float horizontalInput)
    {
        generatingDust = true;
        ParticleSystem.MainModule mainModule = dustParticles.main;
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = dustParticles.velocityOverLifetime;
        velocityOverLifetime.xMultiplier = Math.Abs(velocityOverLifetime.xMultiplier) * (horizontalInput * -1);
        ParticleSystem.EmissionModule emission = dustParticles.emission;
        while(shouldSkid && playerMovement.getCurForm().formName == "Ball"){
            emission.rateOverTime = Math.Abs(rb.velocity.x) * emissionMultiplier;
            if(Input.GetKeyDown(KeyCode.Space)){
                jumpedWhileSkidding = true;
            }
            yield return null;
        }
        yield return new WaitForSeconds(timeDelay);
        generatingDust = false;
        jumpedWhileSkidding = false;
    }
    public void setParticleColor(Color color)
    {
        ParticleSystem.MainModule mainModule = dustParticles.main;
        mainModule.startColor = color;
    }
    public bool getShouldSkid(){
        return shouldSkid;
    }
}
