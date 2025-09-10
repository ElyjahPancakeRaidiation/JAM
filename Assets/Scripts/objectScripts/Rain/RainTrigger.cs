using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RainTrigger : MonoBehaviour
{
    private RainController rain;

    [Header("----Increase and Decrease Settings----")]
    [SerializeField, Tooltip("Increases or decreases the amount you set")]
    private float amount;

    [SerializeField, Tooltip("The amount of rain wanted in the end")]
    private float goal;

    private Coroutine rainEnumerator;

    [Header("----Rain Friction Settings----")]
    [SerializeField, Tooltip("If true it will change the players current rainyfriction to rainyFrictionUp and rainyFrictionDown")]
    private bool changesRainFriction;

    [SerializeField]private float rainyFrictionUp, rainyFrictionDown;


    [SerializeField, Tooltip("Turns on or off whether the rain is following the player when they passes by.")]
    private bool isRainFollowingPlayer;

    [SerializeField, Tooltip("When the player passes through this it will turn off the rain when players cam isn't looking at it. To turn the rain back on it must pass through another trigger with the bool off.")]
    private bool stopRainWhenOutOfCamera;
    [SerializeField] private bool followPlayerYAxis;




    // Start is called before the first frame update
    void Start()
    {
        rain = GameObject.FindGameObjectWithTag("Rain Controller").GetComponent<RainController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){

            //if statement decides whether it should decrease or increase
            if(goal > rain.getAmountOfRain() && rainEnumerator == null){
                rainEnumerator = StartCoroutine(rain.increaseAmountOfParticles(amount, goal));
            }else if(goal < rain.getAmountOfRain() && rainEnumerator == null){
                rainEnumerator = StartCoroutine(rain.decreaseAmountOfParticles(amount, goal));
            }

            if(changesRainFriction){
                collision.GetComponent<PlayerManager>().PlayerMovement().setRainyFrictionUp(rainyFrictionUp);
                collision.GetComponent<PlayerManager>().PlayerMovement().setRainyFrictionDown(rainyFrictionDown);
            }

            if(!rain.getIsOutOfSight()){//Makes it so it doesnt stop when the player goes through the trigger again
                rain.setIsFollowingPlayer(isRainFollowingPlayer);
            }
            rain.setOutOfSight(stopRainWhenOutOfCamera);
            rain.setFollowPlayerYAxis(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        rainEnumerator = null;
    }
}
