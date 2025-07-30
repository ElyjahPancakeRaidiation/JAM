using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    PlayerManager playerManager;
    Physics physics;

    [Header("PHYSICS VARIABLES")]
    [SerializeField] public float coefficientOfFriction;
    [SerializeField] public float rainyFrictionUp;
    [SerializeField] public float rainyFrictionDown;

    private bool checkingImpact;
    public float velocitySoundThreshold;
    // private UnityEvent playerImpact;
    public event Action playerImpact;//switched it from unity event to an action event because of some weird errors

    private float lastVelocityX; 

    void Start()
    {

        // if(playerImpact==null){ new UnityEvent(); }
        playerManager = GetComponent<PlayerManager>();
        physics = new Physics(GetComponent<Rigidbody2D>());
    }

    void Update()
    {
        physics.setCoefficientOfFriction(coefficientOfFriction);
        physics.setRainyFrictionUp(rainyFrictionUp);
        physics.setRainyFrictionDown(rainyFrictionDown);

        if (!playerManager.GlobalIsGrounded() && !checkingImpact)
        {
            //Starts the coroutine to play a sound when falling at certain speed
            StartCoroutine(impactSound());
        }

        playerManager.GetFormFunctionality().UpdateMethodMovement();
    }

    private void FixedUpdate()
    {
        physics.Friction();
        if (!playerManager.IsPlayerFormsEmpty())
        {
            playerManager.GetFormFunctionality().FormMovement();
        }
    }


    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("RainShit"))
        {
            physics.slipperyShitFunction();
        }
    }

    public IEnumerator impactSound()
    {
        checkingImpact = true;
        yield return new WaitUntil(() => playerManager.GlobalIsGrounded());
        //Debug.Log(physics._rb.velocity.y);
        if (Mathf.Abs(physics._rb.velocity.y) > velocitySoundThreshold)
        {
            playerManager.PlayPlayerSfx("Landing");
            playerImpact?.Invoke();
        }
        checkingImpact = false;
    }

    public void setCoefficientOfFriction(float newCOF) => coefficientOfFriction = newCOF;
    public float getCoefficientOfFriction() { return coefficientOfFriction; }
    public float getRainyFrictionUp() { return rainyFrictionUp; }
    public float getRainyFrictionDown() { return rainyFrictionDown; }
    public void setRainyFrictionUp(float amount) => rainyFrictionUp = amount;
    public void setRainyFrictionDown(float amount) => rainyFrictionDown = amount;
    // public UnityEvent getPlayerImpact()
    // {
    //     if(playerImpact==null){ new UnityEvent(); }
    //     return playerImpact;
    // }
    public float GetAcceleration()
    {
        float aMultiplier; //acceleration multiplier
        if (lastVelocityX < 0 && physics._rb.velocity.x < 0)
        {
            aMultiplier = -1;
        }
        else
        {
            aMultiplier = 1;
        }
        float avgAcceleration = aMultiplier * (physics._rb.velocity.x - lastVelocityX) / Time.deltaTime;
        return avgAcceleration;
    }    
}
