using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Cryptography;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class ManageWind : MonoBehaviour
{
    [SerializeField] private ParticleSystem Windparticles;
    private ParticleSystem currentWindParticle;

    [SerializeField] private Vector3 windParticlePosition;
    private BoxCollider2D windCollider;
    [SerializeField] private float sizeX;
    [SerializeField] private float sizeY;
    [SerializeField] private Vector2 offset;
    private bool particlesInstantiated;
    [SerializeField] private float windForceX;
    [SerializeField] private float windForceY;
    private bool isForceHorizontal;
    [SerializeField] private int currentMaxParticles;
    private bool playerWithinZone;

    [SerializeField] private float incrementValue;

    private bool isForceIncreasing;
    private float stayTimer;

    [SerializeField] private float maxMultiplier;

    private isGroundedScript isGrounded;

    private LayerMask mask;
    private Vector2 point, pointOffset, endPosition;

    private GameObject player;
    private float multiplier;
    private Rigidbody2D playerRb;


    // Start is called before the first frame update


    /*
        Task: 
        1. Play the particles and apply the force only when it's the player in there.
        2. Orientiate wind particle in direction to the force
        3. Figure out what to do when the bounds of the collider increases


    */


    void Awake()
    {

        windCollider = GetComponent<BoxCollider2D>();
        playerWithinZone = false;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRb = player.GetComponent<Rigidbody2D>();
        isGrounded =  GameObject.FindGameObjectWithTag("WindDetector").GetComponent<isGroundedScript>();
        
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ColliderBounds(new Vector2(sizeX, sizeY), offset);
        ParticleBounds();
        RotateWind(currentWindParticle);
        GetParticlePosition(currentWindParticle);

     //  Debug.Log(IsPlayerWithinZone());
        //RunWind();
         
    }

    void FixedUpdate()
    {   
    
        RunWind();

    }

    void LateUpdate()
    {
       

    }
    void ColliderBounds(Vector2 size, Vector2 offset)
    {
        windCollider.size = size;
        windCollider.offset = offset;

    }
    
    void OnDrawGizmos()
    {
       Gizmos.color = Color.cyan;

       Gizmos.DrawWireCube(transform.position + (Vector3)offset, new Vector3(sizeX, sizeY, 0));

    //    Gizmos.color = Color.red;

    //    Gizmos.DrawWireCube(player.transform.position + (Vector3)point + (Vector3)pointOffset, endPosition);

    }

    void RunWind()
    {
        if (IsPlayerWithinZone())
        {
            playerWithinZone = true;
            ApplyForce();
            ForceMultiplierY(incrementValue, maxMultiplier);
            stayTimer += Time.deltaTime;
            IncreaseParticleSpeed(windForceX, windForceY);
            SpawnWindParticles();
            stayTimer += Time.deltaTime;
        }
        else
        {   
            StopWindParticles();
            stayTimer = 0;
            multiplier = 1;
            playerWithinZone = false;

        }
    }
  

    bool IsPlayerWithinZone()
    {

        return isGrounded.isGroundedRay(player.transform.position + (Vector3)point, pointOffset, endPosition, mask);
    }

    bool ForceDirection(float forceX, float forceY)
    {
  
        if (forceX > 0 || forceX < 0)
        {
            isForceHorizontal = false;
        }
        else if (forceY > 0 || forceY < 0)
        {
            isForceHorizontal = true;
        }

        return isForceHorizontal;
    }

    bool getIsForceIncreasing()
    {
        if (incrementValue > 0)
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
        if (currentWindParticle != null)
        {
            var windPosition = currentParticles.shape;

            windPosition.position = windParticlePosition;
        }

    }

    void IncreaseParticleSpeed(float forceX, float forceY)
    {
        if (currentWindParticle != null)
        {

            Vector2 originForce = new Vector2(0, 0);
            Vector2 curentForce = GetForce(forceX, forceY);

            float forceDifference = Vector2.Distance(curentForce, originForce);
            
            ParticleSystem.VelocityOverLifetimeModule newParticleSpeed = currentWindParticle.velocityOverLifetime;

            newParticleSpeed.speedModifier = new ParticleSystem.MinMaxCurve(Mathf.Lerp(forceDifference/15, forceDifference / 8, stayTimer/5), Mathf.Lerp(forceDifference / 5, forceDifference/3, stayTimer/5));
     
            ParticleSystem.EmissionModule particleAmount = currentWindParticle.emission;

            ParticleSystem.MainModule totalParticles = currentWindParticle.main;
           
            totalParticles.maxParticles = currentMaxParticles;
            if (getIsForceIncreasing() || forceDifference > 2)
            {
                particleAmount.rateOverTime = new ParticleSystem.MinMaxCurve(100 + forceDifference, 160 + forceDifference * 1.5f);
            }

        }


        // Debug.Log("New speed: " + particleStartSpeed.startSpeed);
        // Debug.Log("Multiplier: " + particleStartSpeed.startSpeed);
    }

    public float ForceMultiplierY(float increment, float maxMultiplier)
    {
        
        multiplier += increment * stayTimer/10;

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
        if (currentParticles != null)
        {
            float ninety = 90 * Mathf.Deg2Rad;

            float forceRotation = ninety - Mathf.Atan2(forcePower.y, forcePower.x);
            float forceRotationInDegrees = 90 - (Mathf.Rad2Deg * Mathf.Atan2(forcePower.y, forcePower.x));

            var currentRotation = currentParticles.main;
            currentRotation.startRotation = forceRotation * -1;

            var transformRotation = currentParticles.shape;
            transformRotation.rotation = new Vector3(0, 0, forceRotationInDegrees * -1);

        }

    }

    void ApplyForce()
    {
         
        playerRb.AddForce(GetForce(windForceX, windForceY), ForceMode2D.Force);
   
        
    }
    void ParticleBounds(/*Vector2 force, float radius*/)
    {
        if (currentWindParticle != null)
        {
            var windParticle = currentWindParticle.shape;
            windParticle.radius = ForceDirection(windForceX, windForceY) ? sizeX / 2 : sizeY / 2;
        }
    }

    void SpawnWindParticles()
    {

        if (!particlesInstantiated)
        {
            currentWindParticle = Instantiate(Windparticles, transform.position + new Vector3(0, -50, 0), Quaternion.identity);

            particlesInstantiated = true;
        }
        if (windForceX != 0 || windForceY != 0)
        {
            currentWindParticle.Play();
        }
        else StopWindParticles();

    }


    void StopWindParticles()
    {
        if (currentWindParticle != null) currentWindParticle.Stop();
    }

    void OnTriggerStay2D(Collider2D other)
    {   
   
        if (playerWithinZone)
        {
            Rigidbody2D collidedRb = other.attachedRigidbody;

            if (collidedRb != playerRb)
            {
                collidedRb.AddForce(GetForce(windForceY, windForceY), ForceMode2D.Force);
            }
            
           // IncreaseParticleSpeed(windForceX, windForceY);
            
        }

    }

}

