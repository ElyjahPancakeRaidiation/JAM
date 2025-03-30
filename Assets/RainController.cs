using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class RainController : MonoBehaviour
{
    [SerializeField, Tooltip("Rain shit is the actual particle itself. Will Always initially be set in the prefab(DONT TOUCH)")]
    private ParticleSystem rainShit;//contains rain particles componenent


    [SerializeField, Tooltip("As the name says this controls the amount of particlest that will show on screen")]
    private float amountOfParticles;


    [SerializeField, Tooltip("This controls the speed of the rain particle")]
    private float speed;


    [SerializeField, Tooltip("This is an offset to keep a certain amount of distance between the rain controller object and the player (mainly change the Y axis)")]
    private Vector2 offset;

    private Transform playerPos;

    [SerializeField, Tooltip("If this is true this object will follow the player, if not it will be stationary")]
    private bool isFollowingPlayer;

    [SerializeField, Tooltip("this controls the speed the rain follows the player. It will only follow in the X direction so where ever you put it in the y it will stay there.")]
    private float followSpeed;

    private float refFloat;

    //Tells the rain controller whether it should turn off the rain when out of sight of the camera
    private bool outOfSight, isOutOfSight;
    private float minCameraView, maxCameraView;

    // Start is called before the first frame update
    void Start()
    {
        if(rainShit == null){Debug.LogError("Rain shit particle system in null");}//saftey incase its null
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        var emissions = rainShit.emission;
        var main = rainShit.main;
        emissions.rateOverTime = amountOfParticles;
        main.simulationSpeed = speed;

        //Both are the maximum and minimum of when an object is out of the camera view for the x position
        minCameraView = -1.40f;
        maxCameraView = 2.40f;
    }

    // Update is called once per frame
    void Update()
    {

        if(outOfSight){

            if(Camera.main.WorldToViewportPoint(transform.position).x < minCameraView){
                rainShit.gameObject.SetActive(false);
                isFollowingPlayer = true;
                isOutOfSight = true;
            }else if(Camera.main.WorldToViewportPoint(transform.position).x > maxCameraView){
                rainShit.gameObject.SetActive(false);
                isFollowingPlayer = true;
                isOutOfSight = true;
            }

        }else{
            rainShit.gameObject.SetActive(true);
            isOutOfSight = false;
        }

        if(isFollowingPlayer){
            float changeX = Mathf.SmoothDamp(transform.position.x, playerPos.position.x + offset.x, ref refFloat, followSpeed);
            transform.position = new Vector3(changeX, transform.position.y);
        }
        
        var emissions = rainShit.emission;
        var main = rainShit.main;
        emissions.rateOverTime = amountOfParticles;
        main.simulationSpeed = speed;
    }

    public IEnumerator decreaseAmountOfParticles(float decreaseAmount, float goal){
        
        // var emissions = rainShit.emission;
        // float rateOverTimeAmount = emissions.rateOverTime.constant;

        if(amountOfParticles == 0){yield break;}//Make sure it doesnt go to the negatives

        while(amountOfParticles > goal){
            amountOfParticles -= decreaseAmount * Time.deltaTime;
            yield return null;
        }
        if(amountOfParticles < 0){amountOfParticles = 0;}
    }

    public IEnumerator increaseAmountOfParticles(float increaseAmount, float goal){
        // var emissions = rainShit.emission;
        // float rateOverTimeAmount = emissions.rateOverTime.constant;
        while(amountOfParticles < goal){
            amountOfParticles += increaseAmount * Time.deltaTime;
            yield return null;
        }
    }

    public void setIsFollowingPlayer(bool state){isFollowingPlayer = state;}
    public void setOutOfSight(bool state){outOfSight = state;}
    public float getAmountOfRain(){return amountOfParticles;}
    public bool getIsOutOfSight(){return isOutOfSight;}
}
