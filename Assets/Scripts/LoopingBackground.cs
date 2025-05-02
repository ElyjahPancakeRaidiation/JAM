using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    //[SerializeField]private float speed;
    [SerializeField]private GameObject imagePrefab;
    [SerializeField]private GameObject curImage, previousImage;
    [SerializeField]private GameObject endPoint;
    //private Vector3 objectSpawnPosition;
    
    public static bool isLooping;
    [SerializeField]private Transform spawnPosition;

    //This is used to find out whether it should duplicate when its below zero or above this depends on the distance 
    //from the lower bound point and the end point
    [Tooltip("This sets the calculations to ")]
    [SerializeField]private bool isDependantOnXAxis;
    [SerializeField]private Vector2 offset;
    [SerializeField]private Vector3 rotationOffset;

    public bool stopOnNext;
    public static bool pushPlayer;

    public static Vector2 freezePosition;
    public float attractionPower;
    [SerializeField]private Transform freezeobject;

    [SerializeField]private float upperBoundC;
    [SerializeField]private float lowerBoundC;
    private GameObject player;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        freezePosition = freezeobject.transform.position;
        freezePosition.x += .3f;
    }

    // Update is called once per frame
    void Update()
    {
        if((passedXYCord(player.transform.position.y, freezeobject.position.y-1, "greater") && passedXYCord(player.transform.position.y, freezeobject.position.y+1, "lesser")) && !stopOnNext){
            //flingPlayerScript.player.transform.position = Vector2.Lerp(flingPlayerScript.player.transform.position, freezeobject.position, 3);
            isLooping = true;
        }
        
        if(isLooping){
            if(curImage != null){curImage.GetComponent<MovingGround>().canMove = true;}
            previousImage.GetComponent<MovingGround>().canMove = true;
            loopHill();
        }else{
            if(curImage != null){curImage.GetComponent<MovingGround>().canMove = false;}
            previousImage.GetComponent<MovingGround>().canMove = false;
        }
    }

    private void loopHill(){
        // float x = Mathf.LerpAngle(flingPlayerScript.player.transform.position.x, freezePosition.x, attractionPower * Time.deltaTime);
            //float x = Mathf.Clamp(flingPlayerScript.player.transform.position.x, flingPlayerScript.point.x-1f, 20);
            //flingPlayerScript.player.transform.position = new Vector3(flingPlayerScript.player.transform.position.x, flingPlayerScript.player.transform.position.y);
            endPoint = previousImage.GetComponent<EndandStartPoint>().getEndPoint();
            spawnPosition = previousImage.GetComponent<EndandStartPoint>().getSpawnPoint();
            float lowerAxisDistance;
            float upperAxisDistance;
            if(isDependantOnXAxis){
                // lowerAxisDistance = lowerBoundCamPoint.transform.position.x - endPoint.transform.position.x;
                // upperAxisDistance = upperBoundCamPoint.transform.position.x - endPoint.transform.position.x;
                float upper = Camera.main.ViewportToWorldPoint(new Vector3(0, upperBoundC)).x;
                float lower = Camera.main.ViewportToWorldPoint(new Vector3(0, lowerBoundC)).x;
                lowerAxisDistance = lower - endPoint.transform.position.x;
                upperAxisDistance = upper - endPoint.transform.position.x;
            }else{
                float upper = Camera.main.ViewportToWorldPoint(new Vector3(0, upperBoundC)).y;
                float lower = Camera.main.ViewportToWorldPoint(new Vector3(0, lowerBoundC)).y;
                lowerAxisDistance = lower;
                upperAxisDistance = upper;
            }


            if(endPoint.transform.position.y > lowerAxisDistance && curImage == null){
                // spawn = true;
                
                curImage = Instantiate(imagePrefab, spawnPosition.position, Quaternion.identity);
                // Debug.Log("jaljadf;djs");
                if(stopOnNext){
                    isLooping = false;
                    curImage.GetComponent<MovingGround>().canMove = false;
                    previousImage.GetComponent<MovingGround>().canMove = false;
                    pushPlayer = true;
                }
            }

            if(endPoint.transform.position.y > upperAxisDistance && curImage != null){
                //Debug.Log(Camera.main.WorldToScreenPoint(endPoint.transform.position));
                Destroy(previousImage);
                previousImage = curImage;
                curImage = null;

            }
    }

    private bool passedXYCord(float objX, float objToPassX, string valueToCheck){
        if(valueToCheck.Equals("greater")){
            if(objX >= objToPassX){
                return true;
            }
        }else if(valueToCheck.Equals("lesser")){
            if(objX <= objToPassX){
                return true;
            }
        }

        return false;
    }
}
