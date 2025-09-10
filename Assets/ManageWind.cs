using System;
using Unity.VisualScripting;
using UnityEngine;

public class ManageWind : MonoBehaviour
{
    private ParticleSystem windParticles;
    //private ParticleSystem windParticles;

    [SerializeField] private Vector3 windParticlePosition;
    //  private BoxCollider2D windCollider;
    [SerializeField] private float sizeX;
    [SerializeField] private float sizeY;
    [SerializeField] private Vector2 offset;
    private bool particlesInstantiated;
    [SerializeField] private float windForceX;
    [SerializeField] private float windForceY;

    private BoxCollider2D windCollider;
    private bool isForceHorizontal;
    [SerializeField] private int currentMaxParticles;
    [SerializeField] private bool playerWithinZone;

    [SerializeField] private float incrementValue;

    private bool isForceIncreasing;
    private float stayTimer;

    [SerializeField] private float maxMultiplier;

    //  [SerializeField] private isGroundedScript isGrounded;

    private LayerMask layerMask;

    [SerializeField] private GameObject player;
    private float multiplier;
    private Rigidbody2D playerRb;

    private float currentVelocity;

    private bool captureNextFrame;

    public AnimationCurve windForceCurve;

    // Start is called before the first frame update

    void OnEnable()
    {

    }

    void Awake()
    {
        bool abe = false;


        // windCollider = GetComponent<BoxCollider2D>();
        windParticles = GetComponent<ParticleSystem>();
        playerWithinZone = false;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        //isGrounded =  GameObject.FindGameObjectWithTag("WindDetector").GetComponent<isGroundedScript>();
        layerMask = LayerMask.GetMask("Player");
      
       // windCollider = GetComponent<BoxCollider2D>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ColliderBounds(new Vector2(sizeX, sizeY), offset);
        ParticleBounds();
        RotateWind(windParticles);
        GetParticlePosition(windParticles);

        // ColliderBounds();
     

        //  Debug.Log(IsPlayerWithinZone());
        //RunWind();

    }

    void FixedUpdate()
    {
        //Debug.Log("velocity: " + playerRb.velocity.y);
        RunWind();

        // if (captureNextFrame == true)
        // {
        //     currentVelocity = playerRb.velocity.y;
        //    // captureNextFrame = false;
        // }
        Debug.Log("current velocity: " + currentVelocity);
    }

    void OnDrawGizmos()
    {
        //    Gizmos.color = Color.cyan;

        //    Gizmos.DrawWireCube(transform.position + (Vector3)offset, new Vector3(sizeX, sizeY, 0));

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(transform.position + (Vector3)offset, new Vector3(sizeX, sizeY, 0));

    }

    void ColliderBounds()
    {
        windCollider.size = new Vector2(sizeX, sizeY);
        windCollider.offset = offset;
    }

    void RunWind()
    {
        if (IsPlayerWithinZone())
        {

            playerWithinZone = true;
            ApplyForce();
            //ForceMultiplierY(incrementValue, maxMultiplier);
            stayTimer += Time.deltaTime;
            IncreaseParticleSpeed(windForceX, windForceY);
            SpawnWindParticlesV2();
            // IncreaseMultiplier();

        }
        else
        {
            StopWindParticles();
            stayTimer = 0;
            multiplier = 1;
            incrementValue = .5f;
            playerWithinZone = false;
            maxMultiplier = 1;

        }
    }


    bool IsPlayerWithinZone()
    {   
        return Physics2D.OverlapBox(transform.position + (Vector3)offset, new Vector3(sizeX, sizeY), 0, layerMask);
    }

    bool ForceDirection(float forceX, float forceY)
    {
        Vector2 force = GetForce(forceX, forceY);
        Vector2 originForce = new Vector2(0, 0);

        float forceDifference = Vector2.Distance(originForce, force);

        // Debug.Log("Difference " + forceDifference);

        if (forceX > 0 || forceX < 0)
        {
            isForceHorizontal = true;
        }
        else if (forceY > 0 || forceY < 0)
        {
            isForceHorizontal = true;
        }

        return isForceHorizontal;
    }

    bool getIsForceIncreasing()
    {
        if (maxMultiplier > 1)
        {
            isForceIncreasing = true;
        }
        else isForceIncreasing = false;

        return isForceIncreasing;
    }


    Vector2 GetForce(float forceX, float forceY)
    {
        //collidedRb.AddForce(new Vector2(windForceX, windForceY * OldForceMultiplierY(multiplierIncrement)),

        Vector2 forcePower = new Vector2(forceX, forceY * multiplier);

        return forcePower;
    }

    void GetParticlePosition(ParticleSystem currentParticles)
    {
        var windPosition = currentParticles.shape;
        windPosition.position = windParticlePosition;

    }

    void IncreaseParticleSpeed(float forceX, float forceY)
    {

        Vector2 originForce = new Vector2(0, 0);
        Vector2 curentForce = GetForce(forceX, forceY);

        float forceDifference = Vector2.Distance(curentForce, originForce);

        ParticleSystem.VelocityOverLifetimeModule newParticleSpeed = windParticles.velocityOverLifetime;

        ParticleSystem.EmissionModule particleAmount = windParticles.emission;

        ParticleSystem.MainModule totalParticles = windParticles.main;

        totalParticles.maxParticles = currentMaxParticles;
        if (!getIsForceIncreasing())
        {
            newParticleSpeed.speedModifier = new ParticleSystem.MinMaxCurve(.7f + forceDifference / 40, 6 + forceDifference / 10);
        }
        else
        {
            newParticleSpeed.speedModifier = new ParticleSystem.MinMaxCurve(Mathf.Lerp(forceDifference / 20, forceDifference / 15, stayTimer / 5), Mathf.Lerp(forceDifference / 12, forceDifference / 7, stayTimer / 5));
            particleAmount.rateOverTime = new ParticleSystem.MinMaxCurve(100 + Mathf.Lerp(forceDifference * 5, forceDifference * 7, stayTimer / 3), 160 + Mathf.Lerp(forceDifference * 8, forceDifference * 12, stayTimer / 3));

        }




        // Debug.Log("New speed: " + particleStartSpeed.startSpeed);
        // Debug.Log("Multiplier: " + particleStartSpeed.startSpeed);
    }

    public float ForceMultiplierY(float increment, float maxMultiplier)
    {
        if (increment < .05)
        {
            increment = .05f;
        }
        multiplier += increment * stayTimer / 10;

        if (multiplier > maxMultiplier)
        {
            multiplier = maxMultiplier;
        }

        return multiplier;
    }

    void RotateWind(ParticleSystem currentParticles)
    {
        Vector2 forcePower = GetForce(windForceX, windForceY);

        //get the wind rotation

        float ninety = 90 * Mathf.Deg2Rad;

        float forceRotation = ninety - Mathf.Atan2(forcePower.y, forcePower.x);
        float forceRotationInDegrees = 90 - (Mathf.Rad2Deg * Mathf.Atan2(forcePower.y, forcePower.x));

        var currentRotation = currentParticles.main;
        currentRotation.startRotation = forceRotation * -1;

        var transformRotation = currentParticles.shape;
        transformRotation.rotation = new Vector3(0, 0, forceRotationInDegrees * -1);

    }

    void ApplyForce()
    {
        playerRb.AddForce(GetForce(windForceX, windForceY), ForceMode2D.Force);
        ApplyForceToVines();
    }

    void ApplyForceToVines()
    {

        if (GameObject.FindGameObjectsWithTag("Vine") != null)
        {
            GameObject[] vines = GameObject.FindGameObjectsWithTag("Vine");

            foreach (GameObject vine in vines)
            {
                Rigidbody2D vineRb = vine.GetComponent<Rigidbody2D>();
                vineRb.AddForce(GetForce(windForceX, windForceY), ForceMode2D.Force);
            }
        }
            
        
      
    }

    void ParticleBounds(/*Vector2 force, float radius*/)
    {
        // ParticleSystem.ShapeModule windPartShape = windParticles.shape;
        var windParticle = windParticles.shape;
        windParticle.radius = ForceDirection(windForceX, windForceY) ? sizeX / 1.5f : sizeY / 1.5f;

    }

    void SpawnWindParticlesV2()
    {

        if (windForceX != 0 || windForceY != 0)
        {
            windParticles.Play();
        }
        else StopWindParticles();

    }


    void StopWindParticles()
    {
        windParticles.Stop();
    }
    
    void IncreaseMultiplier()
    {

        float playerVerticalVelocity = Mathf.Abs(playerRb.velocity.y);
        maxMultiplier += windForceCurve.Evaluate(stayTimer);
        incrementValue += .1f;
        float verticalVelocity = playerRb.velocity.y;

        // maxMultiplier += 



    }
    void OnTriggerStay2D(Collider2D other)
    {

        if (IsPlayerWithinZone())
        {
            Rigidbody2D collidedRb = other.attachedRigidbody;

            if (collidedRb != playerRb)
            {
                collidedRb.AddForce(GetForce(windForceY, windForceY), ForceMode2D.Force);
            }

            // IncreaseParticleSpeed(windForceX, windForceY);
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if (playerRb = other.attachedRigidbody)
        {
            currentVelocity = playerRb.velocity.y;
        }


    }

    void OnTriggerExit2D(Collider2D other)
    {
        currentVelocity = 0;
    }
}

