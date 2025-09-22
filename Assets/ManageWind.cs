using System;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Collections;
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
    private bool playerWithinZone;

    private bool isForceIncreasing;
    private float stayTimer;
    private enum MultiplierStatus { On, Off }

    [SerializeField] private MultiplierStatus multiplierStatus;
    [SerializeField] private float maxMultiplier;
    [SerializeField] private float incrementValue;

    private LayerMask layerMask;
    private LayerMask layerMaskVine;
    [SerializeField] private GameObject player;
    private float multiplier;
    private Rigidbody2D playerRb;

    private float currentVelocity;

    private bool captureNextFrame;

    public AnimationCurve windForceCurve;

    private PlayerManager playerManager;

    [SerializeField] private bool velocityChecked;

    public float playerVelocity;

    private float finalVelocity;
    public float currentPosition;

    [SerializeField] private float increaseDistance;
    [SerializeField] private WindForceAdjustment[] windForceAdjustments;

    void Awake()
    {
        // windCollider = GetComponent<BoxCollider2D>();
        windParticles = GetComponent<ParticleSystem>();
        playerWithinZone = false;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        //isGrounded =  GameObject.FindGameObjectWithTag("WindDetector").GetComponent<isGroundedScript>();
        layerMask = LayerMask.GetMask("Player");
        layerMaskVine = LayerMask.GetMask("Vine");
        // windCollider = GetComponent<BoxCollider2D>();
        windForceAdjustments = GetComponents<WindForceAdjustment>();
        playerManager = player.GetComponent<PlayerManager>();

    }


    void Update()
    {
        //ColliderBounds(new Vector2(sizeX, sizeY), offset);
        ParticleBounds();
        RotateWind(windParticles);
        GetParticlePosition(windParticles);
        // Debug.Log("Player's velocity: " + GetVelocity());

    }

    void FixedUpdate()
    {
        RunWind();
        GetVelocity();
        ChangeMultiplier();
    }

    float GetVelocity()
    {

        if (!playerManager.GlobalIsGrounded() && velocityChecked)
        {
            currentPosition = player.transform.position.y;
            velocityChecked = false;
        }

        if (playerManager.GlobalIsGrounded())
        {
            velocityChecked = true;
            playerVelocity = 0;
            finalVelocity = playerVelocity;

        }
        //Mathf.Pow(increaseDistance, 2)
        float distance = (currentPosition * increaseDistance) - offset.y;
       // finalVelocity = Mathf.Abs((currentPosition * Velocitydifference) - offset.y);
        // Debug.Log("distance: " + distance);
        finalVelocity = Mathf.Sqrt(2 * 9.8f * distance);

        return finalVelocity;

    }
    [SerializeField] private Vector2 firstFrame;
    [SerializeField]private Vector2 secondFrame;
    [SerializeField] private Vector2 thirdFrame;
    [SerializeField]private float fourthFrame;
   
    void ChangeMultiplier()
    {
        //float velocity = Mathf.Abs(GetVelocity());

        if (multiplierStatus == MultiplierStatus.On)
        {

            Debug.Log("final velocity: " + finalVelocity);
            // windForceCurve.AddKey(3.5f, finalVelocity);
     
            windForceCurve = new AnimationCurve(new Keyframe(0, firstFrame.y), new Keyframe(secondFrame.x, secondFrame.y),
            new Keyframe(thirdFrame.x, thirdFrame.y), new Keyframe(fourthFrame, finalVelocity));

            windForceY = windForceCurve.Evaluate(stayTimer);
            // incrementValue += velocity / 1000;
            //Debug.Log("increment: " + incrementValue);

            Debug.Log("multiplier: " + multiplier);
            Debug.Log("Result: " + multiplier * windForceY);

        }

    }

    void OnDrawGizmos()
    {
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

            // Debug.Log("maxMultiplier: " + maxMultiplier);
            playerWithinZone = true;
            ApplyForce();
            // ForceMultiplierY(incrementValue, maxMultiplier);
            stayTimer += Time.deltaTime;
            IncreaseParticleSpeed(windForceX, windForceY);
            SpawnWindParticlesV2();
            // IncreaseMultiplier();
            ApplyForceToVines();
        }

        else
        {
            StopWindParticles();
            stayTimer = 0;
            multiplier = 1;
            // incrementValue = .5f;
            playerWithinZone = false;

        }
    }

    bool IsPlayerWithinZone()
    {
        return Physics2D.OverlapBox(transform.position + (Vector3)offset, new Vector3(sizeX, sizeY), 0, layerMask);
    }
    Collider2D[] IsVineWithinZone()
    {
        return Physics2D.OverlapBoxAll(transform.position + (Vector3)offset, new Vector3(sizeX, sizeY), 0, layerMaskVine);
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
        if (multiplierStatus == MultiplierStatus.On)
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
        if (multiplierStatus == MultiplierStatus.On)
        {
            if (increment < .05)
            {
                increment = .05f;
            }
            multiplier += increment * stayTimer;

            if (multiplier > maxMultiplier)
            {
                multiplier = maxMultiplier;
            }
            // Debug.Log("multiplier: " + multiplier);
            // Debug.Log("ResultForce: " + multiplier * windForceY);
            // Debug.Log("WindY: " + windForceY);
            return multiplier;
        }
        return 1;
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

    }

    void ApplyForceToVines()
    {

        if (!IsPlayerWithinZone()) { return; }
        foreach (Collider2D coll in IsVineWithinZone())
        {

            Rigidbody2D vineRb = coll.attachedRigidbody;

            vineRb.AddForce(GetForce(windForceX * .5f, windForceY * .5f), ForceMode2D.Force);

        }
    }

    void ParticleBounds()
    {
        // ParticleSystem.ShapeModule windPartShape = windParticles.shape;
        var windParticle = windParticles.shape;
        windParticle.radius = ForceDirection(windForceX, windForceY) ? sizeX / 1.2f : sizeY / 1.2f;

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

}

