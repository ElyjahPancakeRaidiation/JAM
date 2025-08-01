using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPhysics : MonoBehaviour
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
    private isGroundedScript isGroundedScript;

    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Vector2 point;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        isGroundedScript = GameObject.Find("Player Isgrounded").GetComponent<isGroundedScript>();

    }

    // Update is called once per frame
    void Update()
    {
        //InWindZone();
    }

    void FixedUpdate()
    {
        // stayTimer += Time.deltaTime;
        // ForceMultiplierY();
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
        multiplier += increment * (stayTimer * stayTimer * stayTimer);
    //    multiplier += increment * stayTimer;
        float result = multiplier * windForceY;
        Debug.Log(result);
        return multiplier;
    }
    void InWindZone()
    {
        if (isGroundedScript.isGroundedBox(point, offset, size, layerMask))
        {
            Debug.Log(isGroundedScript.isGroundedBox(point, offset, size, layerMask));
            Debug.Log("hello");
            try
            {
                WindParticles[0].Play();
                rb.AddForce(new Vector2(windForceX, windForceY /* ForceMultiplierY()*/), ForceMode2D.Force);

            }
            catch (Exception e)
            {
                Debug.Log("Hello assign the particle system to the game object. Irene || 1800018192393121239238193812912812 - E find my pages DISCOVER MY TRUTH");
            }
        }
    }


     void OnTriggerStay2D(Collider2D other)
    {

        //whatever the object that is in the windzone, get that object's rigidbody
        Rigidbody2D collidedRb = other.attachedRigidbody;

        if (collidedRb == rb)
        {
            try
            {
                WindParticles[0].Play();
                collidedRb.AddForce(new Vector2(windForceX, windForceY *  OldForceMultiplierY(multiplierIncrement)), ForceMode2D.Force);
                stayTimer += Time.deltaTime;

            }
            catch (Exception e)
            {
                Debug.Log("Hello assign the particle system to the game object. Irene || 1800018192393121239238193812912812 - E find my pages DISCOVER MY TRUTH");
            }

        }

        //and then apply the force on it


    }

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


        stayTimer = 0;
        OldForceMultiplierY(multiplierIncrement);
    }
    





    // public float ForceMultiplierY(float resetMultiplier = 1)
    // {
    //     throw new NotImplementedException();
    // }
}

// public interface Multiplier
// {

//     public float ForceMultiplierX(float resetMultiplier = 1);
    
//     public float ForceMultiplierY(float resetMultiplier = 1);
    
        
    



// }