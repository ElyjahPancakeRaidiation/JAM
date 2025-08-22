using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindManager : MonoBehaviour
{

    private Rigidbody2D rb;

    public ParticleSystem[] WindParticles;

    public float stayTimer;

    [Header("Timer and Multiplier for extra Boost")]
    [SerializeField] public float maxStayTimer; //the time player stays in the zone

    [SerializeField] private float maxMultiplier;

    [SerializeField] private float windForceX, windForceY;


    private GameObject player;

    private PlayerManager playerManager;
    [SerializeField] private float multiplierIncrement;
    
   // [SerializeField] private AnimationCurve forceCurve;
    [SerializeField] private bool playerWithinZone;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

    }

    // Update is called once per frame
    void Update()
    {
        //InWindZone();
        runTimer();
    }

    void FixedUpdate()
    {
        // stayTimer += Time.deltaTime;
        // ForceMultiplierY()
    
        
    }
    public float MultiplyForceY(float increment, float resetMultiplier = 1)
    {

        float multiplier = resetMultiplier;

        //i want to return multiplier each time it is incremented
        while (multiplier <= maxMultiplier)
        {
            multiplier += increment;
        }
        Debug.Log(multiplier);

        return multiplier;
    }

    public float OldForceMultiplierY(float increment, float resetMultiplier = 1)
    {
        float multiplier = resetMultiplier;
        
        multiplier += increment * (stayTimer * stayTimer);
        //    multiplier += increment * stayTimer;
        float result = multiplier * windForceY;
        Debug.Log(result);
        return multiplier;
    }


    void runTimer()
    {
        if (playerWithinZone)
        {
             stayTimer += Time.deltaTime;
        }
       
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        //whatever the object that is in the windzone, get that object's rigidbody
        Rigidbody2D collidedRb = other.attachedRigidbody;
        collidedRb.AddForce(new Vector2(windForceX, windForceY /** OldForceMultiplierY(multiplierIncrement)*/), ForceMode2D.Force);
    
        if (playerWithinZone)
        {
            
        }
        if (other.CompareTag("Player"))
            {
                try
                {
                    WindParticles[0].Play();
                    playerWithinZone = true;
                 

                }
                catch (Exception e)
                {
                    Debug.Log("Hello assign the particle system to the game object. Irene || 1800018192393121239238193812912812 - E find my pages DISCOVER MY TRUTH");
                }

            }

    }     //and then apply the force on it




    void OnTriggerExit2D(Collider2D collision)
    {
        try
        {
           WindParticles[0].Stop();
        }
        catch (Exception e)
        {
            Debug.Log("Hello assign the particle system to the game object. Irene - GET THE OTHER ERROR FIND MY PAGES!");
        }
    }

}