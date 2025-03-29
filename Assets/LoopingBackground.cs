using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    //[SerializeField]private float speed;
    [SerializeField]private GameObject imagePrefab, upperBoundCamPoint, lowerBoundCamPoint;
    [SerializeField]private GameObject curImage, previousImage;
    [SerializeField]private GameObject endPoint, topPoint;
    //private Vector3 objectSpawnPosition;
    private Camera mainCamera;
    [SerializeField]private bool isLooping;
    [SerializeField]private Transform spawnPosition;
    private bool spawn;

    //This is used to find out whether it should duplicate when its below zero or above this depends on the distance 
    //from the lower bound point and the end point
    [SerializeField]private int direction;
    [Tooltip("This sets the calculations to ")]
    [SerializeField]private bool isDependantOnXAxis;
    [SerializeField]private Vector2 offset;
    [SerializeField]private Vector3 rotationOffset;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        if(isLooping){
            endPoint = previousImage.GetComponent<EndandStartPoint>().getEndPoint();
            spawnPosition = previousImage.GetComponent<EndandStartPoint>().getSpawnPoint();
            float lowerAxisDistance;
            float upperAxisDistance;
            if(isDependantOnXAxis){
                lowerAxisDistance = lowerBoundCamPoint.transform.position.x - endPoint.transform.position.x;
                upperAxisDistance = upperBoundCamPoint.transform.position.x - endPoint.transform.position.x;
            }else{
                lowerAxisDistance = lowerBoundCamPoint.transform.position.y - endPoint.transform.position.y;
                upperAxisDistance = upperBoundCamPoint.transform.position.y - endPoint.transform.position.y;
            }

            Debug.Log(upperAxisDistance);

            if(lowerAxisDistance < 0 && curImage == null){
                // spawn = true;
                
                curImage = Instantiate(imagePrefab, spawnPosition.position, Quaternion.identity);
                // Debug.Log("jaljadf;djs");
            }

            if(upperAxisDistance < 0 && curImage != null){
                Destroy(previousImage);
                previousImage = curImage;
                curImage = null;
            }
            
            


        }
    }

    private Vector2 updateSpawnPosition(){
        return mainCamera.WorldToViewportPoint(lowerBoundCamPoint.transform.position);
    }
    private Vector2 getScreenToWorldPoint(Vector2 pos){
        return mainCamera.ScreenToWorldPoint(pos);
    }
}
