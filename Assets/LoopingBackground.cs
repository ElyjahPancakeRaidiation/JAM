using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    //[SerializeField]private float speed;
    [SerializeField]private GameObject imagePrefab, upperBoundCamPoint, lowerBoundCamPoint;
    private GameObject curImage;
    private Vector3 objectSpawnPosition;
    private Camera mainCamera;
    [SerializeField]private bool isLooping;


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {

        if(isLooping){

            objectSpawnPosition = updateSpawnPosition();
            Debug.Log(objectSpawnPosition);
            // if(curImage == null){
            //     curImage = Instantiate(imagePrefab, objectSpawnPosition, Quaternion.identity);
            // }


        }
    }

    private Vector2 updateSpawnPosition(){
        return mainCamera.WorldToViewportPoint(lowerBoundCamPoint.transform.position);
    }
}
