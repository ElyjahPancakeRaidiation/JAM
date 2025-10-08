using System;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using Unity.Mathematics;

public class ManageWind : MonoBehaviour
{
    private ParticleSystem windParticles;
    //private ParticleSystem windParticles;
    [Header("Wind Position and Size of the Wind Bounds")]
    [Space(10)]
    [SerializeField] private Vector3 windParticlePosition;

    //  private BoxCollider2D windCollider;
    [SerializeField] private float sizeX;
    [SerializeField] private float sizeY;

    [SerializeField] private float sizeMultiplier;
    [SerializeField] private Vector2 windRangeOffset;
    private bool particlesInstantiated;
    [Space(5)]
    [Header("Wind Force Amount")]
    [Space(10)]
    [SerializeField] private float windForceX;
    [SerializeField] private float windForceY;

    [SerializeField] private bool checkPlayerInVisibleZone;
    [SerializeField] private Vector2 visibleSize;

    [SerializeField] private Vector2 visibleWindOffset;

    private BoxCollider2D windCollider;
    private bool isForceHorizontal;
    [Space(5)]
    [Header("Wind Particles Amount/Speed/Rate")]
    [Space(10)]
    [SerializeField] private int currentMaxParticles;
    [SerializeField] private float particleSpeed;
    [SerializeField] private float particleRate;
    private bool playerWithinZone;

    private bool isForceIncreasing;

    private float stayTimer;


    private enum MultiplierStatus { On, Off }
    [Header("Vary WindForce With Graph")]
    [SerializeField] private MultiplierStatus multiplierStatus;

    [SerializeField] private float finalVelocity;

    private float maxMultiplier;
    private float incrementValue;

    private LayerMask layerMask;
    private LayerMask layerMaskVine;
    private GameObject player;
    private float multiplier;
    private Rigidbody2D playerRb;

    private float currentVelocity;

    private bool captureNextFrame;
    [Header("Player's Velocity Thresholds to Change WindForce")]
    [SerializeField] private Vector2[] vRange;

    private PlayerManager playerManager;

    private bool velocityChecked;

    private float playerVelocity;


    private float currentPosition;
    float currentDistance;
    private float increaseDistance;
    [Header("Adjust Wind Graph and Edit KeyFrames")]

    [SerializeField] private AnimationCurve windForceCurve;
    [SerializeField] private Vector2 firstFrame;
    [SerializeField] private Vector2 secondFrame;
    [SerializeField] private Vector2 thirdFrame;
    [SerializeField] private float fourthFrame;

    public static bool IsPlayerInAnyZone;
    public static int windCounter;


    private WindAudioPlayer windAudio;
    void Awake()
    {
        windParticles = GetComponent<ParticleSystem>();
        playerWithinZone = false;
        if (GetComponent<BoxCollider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
        }
        windCollider = GetComponent<BoxCollider2D>();
        windCollider.enabled = true;
        windCollider.isTrigger = true;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        layerMask = LayerMask.GetMask("Player");
        layerMaskVine = LayerMask.GetMask("Vine");
        playerManager = player.GetComponent<PlayerManager>();
        windAudio = GameObject.FindGameObjectWithTag("WindAudio").GetComponent<WindAudioPlayer>();

    }
    void Start()
    {
        currentDistance = increaseDistance;

    }

    void Update()
    {
        ParticleBounds();
        RotateWind(windParticles);
        GetParticlePosition(windParticles);
        IncreaseParticleSpeed(windForceX, windForceY);
        ColliderBounds();
        RunAudio();
        AdjustWindZoneAudio();
        IRunAudioWhenPlayerExitsZone();
        RunAudioTimer();

        // Debug.Log("AudioTImer: " + audioTimer);

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
        float distance = Mathf.Abs(currentPosition * 3 - windRangeOffset.y);

        // finalVelocity = Mathf.Abs((currentPosition * Velocitydifference) - windRangeOffset.y);
        //  Debug.Log("distance: " + distance);
        finalVelocity = Mathf.Sqrt(2 * 9.8f * distance);

        return finalVelocity;

    }
    void SetDashAmount(int d)
    {
        PlayerManager.playerManager.playerForms[0].functionality.setFloat(d);
    }

    void ChangeMultiplier()
    {
        //float velocity = Mathf.Abs(GetVelocity());

        if (multiplierStatus == MultiplierStatus.On)
        {

            // Debug.Log("final velocity: " + finalVelocity);

            TrackVelocity();
            windForceCurve = new AnimationCurve(new Keyframe(0, firstFrame.y), new Keyframe(secondFrame.x, secondFrame.y),
            new Keyframe(thirdFrame.x, thirdFrame.y), new Keyframe(fourthFrame, finalVelocity * increaseDistance));

            windForceY = windForceCurve.Evaluate(stayTimer);
            multiplierOn = true;
            // Debug.Log("Increase Distance: " + increaseDistance);
        }
        else multiplierOn = false;

    }
    void TrackVelocity()
    {

        for (int i = 0; i < vRange.Length; i++)
        {   //2D array or list or dictionary, final velocity between two numbers, [x,y] y needs to initialize increaseDistance
            if (vRange[i].x <= finalVelocity && finalVelocity <= vRange[i + 1].x)
            {
                increaseDistance = vRange[i].y;

            }

        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + (Vector3)windRangeOffset, new Vector3(sizeX, sizeY, 0));


        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)visibleWindOffset, (Vector3)visibleSize);
    }

    void ColliderBounds()
    {
        windCollider.size = new Vector2(sizeX, sizeY);
        windCollider.offset = windRangeOffset;
    }

    bool isDashRecharged;
    void RunWind()
    {
        if (IsPlayerWithinZone())
        {

            playerWithinZone = true;
            ApplyForce();
            stayTimer += Time.deltaTime;
            if (multiplierOn && !isDashRecharged)
            {
                SetDashAmount(1);
                isDashRecharged = true;
            }
            windCounter = 300;
            ApplyForceToVines();
            if (!checkPlayerInVisibleZone)
            {
                SpawnWindParticlesV2();

            }
            // if (!windAudio.GetAudioPlaying())
            // {
            //     windAudio.PlayAudio();
            //     windAudio.GetAudioPlaying(true);

            // 



        }
        else
        {
            StopWindParticles();
            stayTimer = 0;
            multiplier = 1;
            windCounter = 0;
            isDashRecharged = false;
            // incrementValue = .5f;


            // if (windAudio.GetAudioPlaying())
            // {
            //     windAudio.StopAudio();
            //     windAudio.GetAudioPlaying(false);

            // }
        }

        if (checkPlayerInVisibleZone)
        {

            if (IsPlayerWithinVisibleWindZone())
            {
                SpawnWindParticlesV2();
            }
            else
            {
                StopWindParticles();
                IsPlayerInAnyZone = false;
            }
        }

    }

    public bool IsPlayerWithinZone()
    {
        return Physics2D.OverlapBox(transform.position + (Vector3)windRangeOffset, new Vector3(sizeX, sizeY), 0, layerMask);
    }

    Collider2D[] IsVineWithinZone()
    {
        return Physics2D.OverlapBoxAll(transform.position + (Vector3)windRangeOffset, new Vector3(sizeX, sizeY), 0, layerMaskVine);
    }
    bool IsPlayerWithinVisibleWindZone()
    {
        return Physics2D.OverlapBox(transform.position + (Vector3)visibleWindOffset, (Vector3)visibleSize, 0, layerMask);
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

        float forceDifference = Mathf.Abs(Vector2.Distance(curentForce, originForce));

        ParticleSystem.VelocityOverLifetimeModule newParticleSpeed = windParticles.velocityOverLifetime;

        ParticleSystem.EmissionModule particleAmount = windParticles.emission;

        ParticleSystem.MainModule totalParticles = windParticles.main;

        totalParticles.maxParticles = currentMaxParticles;
        if (!getIsForceIncreasing())
        {
            newParticleSpeed.speedModifier = new ParticleSystem.MinMaxCurve(.7f + forceDifference / 25 * (1 + particleSpeed), 6 + forceDifference / 10 * (1 + particleSpeed));
            particleAmount.rateOverTime = new ParticleSystem.MinMaxCurve(100 + forceDifference * (1 + particleRate), 160 + forceDifference * (1 + particleRate));
        }
        else
        {
            // newParticleSpeed.speedModifier = new ParticleSystem.MinMaxCurve(Mathf.Lerp(forceDifference / 60, forceDifference / 40, stayTimer / 5), Mathf.Lerp(forceDifference / 40, forceDifference / 30, stayTimer / 5));
            // particleAmount.rateOverTime = new ParticleSystem.MinMaxCurve(100 + Mathf.Lerp(forceDifference * 5, forceDifference * 7, stayTimer / 3), 160 + Mathf.Lerp(forceDifference * 8, forceDifference * 12, stayTimer / 3));
            newParticleSpeed.speedModifier = new ParticleSystem.MinMaxCurve(.7f + forceDifference / 40 * (1 + particleSpeed), 6 + forceDifference / 10 * (1 + particleSpeed));
            particleAmount.rateOverTime = new ParticleSystem.MinMaxCurve(100 + forceDifference * (1 + particleRate), 160 + forceDifference * (1 + particleRate));

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
        windParticle.radius = ForceDirection(windForceX, windForceY) ? (1 + sizeMultiplier) * sizeX / 1.2f : (1 + sizeMultiplier * sizeY) / 1.2f;

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

    IEnumerator IStopWindAudio()
    {
        yield return new WaitForSeconds(0.5f);
        audioPlayedOnce = false;
        audioTimer = 0;
        if (audioPlayedOnce == false && windAudio.GetAudioPlaying())
        {


            // windAudio.StopAudio();
            windAudio.GetAudioPlaying(false);
        }

    }
    float outsideZoneTimer;

    void IRunAudioWhenPlayerExitsZone()
    {
        if (IsPlayerWithinZone()) return;
        if (!audioPlayedOnce)
        {
            windAudio.AdjustVolume(audioTimer, audioPlayedOnce, multiplierOn);
            windAudio.AdjustPitch(audioTimer);
        }
        outsideZoneTimer += Time.deltaTime;
    }
    bool KeepCurrentAudioVolume;
    bool GetKeepCurrentAudioVolume()
    {
        if (outsideZoneTimer > 1f)
        {
            return KeepCurrentAudioVolume = false;
        }
        else return KeepCurrentAudioVolume = true;
    }
    void RunAudioTimer()
    {
        // if (audioPlayedOnce == true)
        // {
        //     audioTimer += Time.deltaTime;
        // }
        // else
        // {   
        //     audioTimer += Time.deltaTime;
        // }

        audioTimer += Time.deltaTime;
    }
    float audioTimer;
    void RunAudio()
    {
        if (!IsPlayerWithinZone()) return;


        if (audioPlayedOnce && !windAudio.GetAudioPlaying())
        {
            audioTimer = 0;
            windAudio.PlayAudio();
            //  StartCoroutine(windAudio.EditAudio());
            windAudio.GetAudioPlaying(true);
        }


    }

    void AdjustWindZoneAudio()
    {
        if (audioPlayedOnce)
        {
            windAudio.AdjustVolume(audioTimer, audioPlayedOnce, multiplierOn);
            windAudio.AdjustPitch(audioTimer);
        }

    }
    private bool audioPlayedOnce;

    bool multiplierOn;
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            audioPlayedOnce = true;

            StopAllCoroutines();
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            StartCoroutine(IStopWindAudio());
        }
    }


}

