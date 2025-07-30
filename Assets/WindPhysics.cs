using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindPhysics : MonoBehaviour
{

    private Rigidbody2D rb;

    public ParticleSystem[] WindParticles;

    private float stayTimer;
    
    [Header("Timer and Multiplier for extra Boost")]
    [SerializeField] private float maxStayTimer; //the time player stays in the zone

    [SerializeField] private float maxMultiplier;
   
    [SerializeField] private float windForceX, windForceY;


    

    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        
     
       
    }

    // Update is called once per frame
    void Update()
    {

    }


    public float ForceMultiplierX(float resetMultiplier = 1)
    {
        float multiplier = resetMultiplier;

        multiplier += stayTimer;

        if (stayTimer >= maxStayTimer)
        {
            multiplier = maxMultiplier;
        }
        Debug.Log(multiplier);
        return multiplier;

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
                collidedRb.AddForce(new Vector2(windForceX , windForceY * ForceMultiplierX()), ForceMode2D.Force);
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
        ForceMultiplierX(1); 
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