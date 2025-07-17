using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class DustScriptV2 : MonoBehaviour
{
    private GameObject player;
    private ParticleSystem dust;
    private ParticleSystem turningMode;
    private Rigidbody2D rb;
    private PlayerMovement movement;
    private PlayerAbilities abilities;
    private GameManager gm;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rotationSpeed;
    [SerializeField] public float yOffset;

    [SerializeField] public float velocityThreshold;
    [SerializeField] public float minimumVelocity;
    [SerializeField] public float abilityDelayInSeconds;
    public bool playerSkidding;
    public bool generatingDust = false;
    public bool recentlyJumped = false;
    private bool inMud;
    private bool playingLanding;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody2D>();
        movement = player.GetComponent<PlayerMovement>();
        abilities = player.GetComponent<PlayerAbilities>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        dust = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!recentlyJumped)
        {
            moveToPlayer();
            //updateColor();
        }
        if (Input.GetKeyDown(gm.playerAbilityKey) && abilities.GetCanUseAbility()) //dont have particles follow player midair after jumping
        {
            StartCoroutine(onJump());
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            debugLandingParticles();
        }
        if (abilities.isGrounded() && !generatingDust)
        {
            StartCoroutine(checkForSkidding());
        }
    }
    private void moveToPlayer()
    {
        transform.position = new Vector2(player.transform.position.x, player.transform.position.y + yOffset);
    }
    private IEnumerator checkForSkidding()
    {
        generatingDust = true;
        float movingDirection;
        float inputDirection;
        while (abilities.isGrounded() && !playingLanding)
        {
            if (!dust.isPlaying)
            {
                surfaceAlignment();
            }


            movingDirection = rb.velocity.x == 0 ? 0 : Mathf.Sign(rb.velocity.x); //direction you're moving
            inputDirection = Input.GetAxisRaw("Horizontal"); //this probably needs to be replaced

            //align emission to moving direction:
            var shape = dust.shape;
            shape.scale = new Vector3(movingDirection, 1, 1);

            if (Mathf.Abs(rb.velocity.x) > velocityThreshold && movingDirection == -1 * inputDirection && !dust.isPlaying && !recentlyJumped) //reached velocity threshold? holding opposite direction? has particle system started already? did the player try jumping?
            //figure out how to do this with mobile devices accurately
            {
                dust.Play();
            }
            if (Mathf.Abs(rb.velocity.x) < minimumVelocity || Math.Sign(rb.velocity.x) == Math.Sign(inputDirection))
            {
                dust.Stop();
            }
            yield return null;
        }
        dust.Stop();
        generatingDust = false;
    }

    private void surfaceAlignment()
    {
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, 2.0f, groundMask);
        if (hit.collider != null)
        {
            Vector2 normal = hit.normal;
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;

            var shape = dust.shape;
            //shape.rotation = new Vector3(shape.rotation.x, angle, shape.rotation.z);
            shape.rotation = Vector3.Lerp(shape.rotation, new Vector3(shape.rotation.x, shape.rotation.y, angle), Time.deltaTime * rotationSpeed);

        }
    }

    private IEnumerator onJump()
    {
        recentlyJumped = true;
        yield return new WaitForSeconds(abilityDelayInSeconds);
        yield return new WaitUntil(() => abilities.isGrounded());
        recentlyJumped = false;
    }
    private void loadSkidParticles()
    {
        var mainModule = dust.main;
        mainModule.loop = true;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.3f);
        mainModule.simulationSpace = ParticleSystemSimulationSpace.Local;

        var emissionModule = dust.emission;
        emissionModule.rateOverTimeMultiplier = 60;
        emissionModule.burstCount = 0;
        var shapeModule = dust.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Circle;
        shapeModule.radius = 0.9f;
        shapeModule.radiusThickness = 0.04f;
        shapeModule.arc = 40f;
        shapeModule.arcMode = ParticleSystemShapeMultiModeValue.Random;
        shapeModule.arcSpread = 0;
        shapeModule.rotation = new Vector3(90, 0, 0);
        shapeModule.scale = new Vector3(1f, 1f, 1f);
    }
    private void loadLandingParticles() //look at the reference in the scene for setting this up
    {
        var mainModule = dust.main;
        mainModule.loop = false;
        mainModule.startSpeed = 5;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        var emissionModule = dust.emission;
        emissionModule.rateOverTimeMultiplier = 0;
        ParticleSystem.Burst burst = new()
        {
            count = 30,
            cycleCount = 1
        };
        emissionModule.burstCount = 1;
        emissionModule.SetBurst(0, burst);

        var shapeModule = dust.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Cone;
        shapeModule.angle = 16f;
        shapeModule.radius = 0.6f;
        shapeModule.radiusThickness = 0.2f;
        shapeModule.arcMode = ParticleSystemShapeMultiModeValue.Random;
        shapeModule.arc = 360f;
        shapeModule.length = 1.2f;
        shapeModule.rotation = new Vector3(50, 0, 0);
        shapeModule.scale = new Vector3(1f, 0.26f, 1f);

    }
    public void playLandingParticles()
    {
        //Debug.Log("player smacked the ground");
        playingLanding = true;
        loadLandingParticles();
        moveToPlayer();
        //updateColor();
        dust.Play();
        StartCoroutine(ResetParticle());
    }

    private IEnumerator ResetParticle()
    {
        yield return new WaitForSecondsRealtime(1f);
        loadSkidParticles();
        playingLanding = false;
        //Debug.Log("ended ts");
    }
    private void debugLandingParticles()
    {
        playLandingParticles();
    }
}
