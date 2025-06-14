using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class LoopingBackGround1 : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] protected GameObject imagePrefab;
    [SerializeField] protected GameObject curImage;
    [SerializeField] protected GameObject previousImage;

    private bool isLooping;

    [SerializeField] protected bool isDependantOnXAxis;

    [SerializeField] protected Vector2 offset;
    [SerializeField] protected Vector2 borderMaxOffset;
    [SerializeField] protected Vector2 borderMinOffset;
    [SerializeField] private Vector3 rotationOffset;

    public bool stopOnNext;

    /// <summary>
    /// upperBoundLimit - The Maximum it can go before looping. Think of it as the far right of the camera or the top part of the camera. 
    /// Highest is usually 1 or greater for both right and up
    /// 
    /// lowerBoundLimit - The minimum it can go before looping. Think of it as the far left of the camera or the bottom part of the camera. 
    /// Highest is usually 0 or less for both left and down
    /// </summary>
    [SerializeField, Range(-0.10f, 1.10f)] private float upperBoundLimit;
    [SerializeField, Range(-0.10f, 1.10f)] private float lowerBoundLimit;

    [SerializeField] protected Collider2D _currentCol;
    protected Collider2D _previousCol;

    //The direction of where the current background spawned
    private float curDir;
    private float earlyDeleteDistance;
    protected GameObject player;


    private void OnValidate()
    {
        _camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        _previousCol = previousImage.GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {

        if (isLooping)
        {
            //This block switches the current image with what ever bounds the player is currently in while setting the previous image to the previous current image.
            CheckForCurrentImage();
            // CheckForPassedImage();

            //These two variables are the spawn positions for the left and right background
            Vector3 rightPosition = new Vector3(_previousCol.bounds.max.x + _previousCol.bounds.extents.x, previousImage.transform.position.y);
            Vector3 leftPosition = new Vector3(_previousCol.bounds.min.x - _previousCol.bounds.extents.x, previousImage.transform.position.y);
            //These two variables are the current images far right and left positions
            float xMaxCurrentImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.max + (Vector3)borderMaxOffset).x;
            float xMinCurrentImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.min + (Vector3)borderMinOffset).x;

            spawnObjectAfterAnother(xMaxCurrentImage, xMinCurrentImage, rightPosition + (Vector3)offset, leftPosition + (Vector3)offset);//Need to change this to make it more accomidating for the x and y positions -- Note: Later
        }
    }

    public void CheckForCurrentImage()
    {
        // Debug.Log(Camera.main.WorldToViewportPoint(borderMinOffset).y);
        float playerPosAxis = 0;
        if (previousImage != null)
        {
            float prevMinAxis;
            float prevMaxAxis;
            if (isDependantOnXAxis)
            {
                playerPosAxis = player.transform.position.x;
                prevMaxAxis = Camera.main.WorldToViewportPoint(_previousCol.bounds.max + (Vector3)borderMaxOffset).x;
                prevMinAxis = Camera.main.WorldToViewportPoint(_previousCol.bounds.min + (Vector3)borderMinOffset).x;
            }
            else
            {
                playerPosAxis = player.transform.position.y;
                prevMaxAxis = Camera.main.WorldToViewportPoint(_previousCol.bounds.max + (Vector3)borderMaxOffset).y;
                prevMinAxis = Camera.main.WorldToViewportPoint(_previousCol.bounds.min + (Vector3)borderMinOffset).y;
            }

            // Debug.Log(prevMinAxis);
            // Debug.Log(Camera.main.ViewportToWorldPoint(new Vector2(0, prevMinAxis)));

            if (prevMinAxis > upperBoundLimit || prevMaxAxis < lowerBoundLimit)
            {
                GameObject.Destroy(previousImage);
                previousImage = curImage;
                _previousCol = previousImage.GetComponent<Collider2D>();
                curImage = null;
                _currentCol = null;
                curDir = 0;
            }

        }

        //This block of code checks to see if the player goes backwards when theres an already loaded background ahead of them and deletes the current image.
        if (curImage != null)
        {
            Vector2 curMaxAxis;
            Vector2 curMinAxis;
            curMaxAxis = _currentCol.bounds.max;
            curMinAxis = _currentCol.bounds.min;
            if (curDir == 0)
            {
                if (isDependantOnXAxis)
                {
                    curDir = Mathf.Sign(curImage.transform.position.x - playerPosAxis);
                }
                else
                {
                    curDir = Mathf.Sign(curImage.transform.position.y - playerPosAxis);
                }
            }

            //If the player was heading left but went right
            var checkCurandLower = false;
            var checkCurandUpper = false;

            if (isDependantOnXAxis)
            {
                checkCurandLower = Mathf.Abs(curMaxAxis.x - Camera.main.ViewportToWorldPoint(new Vector3(lowerBoundLimit, 0)).x) > 6;
                checkCurandUpper = Mathf.Abs(curMinAxis.x - Camera.main.ViewportToWorldPoint(new Vector3(upperBoundLimit, 0)).x) > 6;
            }
            else
            {
                checkCurandLower = Mathf.Abs(curMaxAxis.y - Camera.main.ViewportToWorldPoint(new Vector3(0, lowerBoundLimit)).y) > 6;
                checkCurandUpper = Mathf.Abs(curMinAxis.y - Camera.main.ViewportToWorldPoint(new Vector3(0, upperBoundLimit)).y) > 6;
            }

            // Debug.Log(checkCurandLower);
            // Debug.Log("Curmax: " + curMaxAxis); 

            // if (curDir == -1)
            // {

            //     if (checkCurandLower)
            //     {
            //         GameObject.Destroy(curImage);
            //         Debug.Log("Delteing curimage by going backwards?--curandlower--");
            //         curImage = null;
            //         _currentCol = null;
            //     }
            // }
            // else
            // {
            //     //If the player was heading right but went left
            //     if (checkCurandUpper)
            //     {
            //         GameObject.Destroy(curImage);
            //         Debug.Log("Delteing curimage by going backwards?--curandupper--");
            //         curImage = null;
            //         _currentCol = null;
            //     }
            // }
        }
    }

    public void spawnObjectAfterAnother(float maxCoord, float minCoord, Vector3 rightPosition, Vector3 leftPosition)
    {

        //This section spawns the next image depending where the cameras borders are going.
        if (maxCoord >= upperBoundLimit)
        {//If the camera is going right next image to the right
            if (curImage == null)
            {
                curImage = Instantiate(imagePrefab, rightPosition, Quaternion.identity);
                Debug.Log("Spwaning curimage");
                _currentCol = curImage.GetComponent<Collider2D>();
                if (stopOnNext)
                {
                    isLooping = false;
                }
            }
        }


        if (minCoord <= lowerBoundLimit)
        {//If the camera is going left next image to the left
            if (curImage == null)
            {
                curImage = Instantiate(imagePrefab, leftPosition, Quaternion.identity);
                _currentCol = curImage.GetComponent<Collider2D>();
                if (stopOnNext)
                {
                    isLooping = false;
                }
            }
        }
    }

    // public void CheckForPassedImage(){
    //     if(previousImage != null){
    //         //These two variables are the previous images far right and left positions
    //         float xMaxPreviousImage;
    //         float xMinPreviousImage;

    //         if(isDependantOnXAxis){
    //             xMaxPreviousImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.max).x;
    //             xMinPreviousImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.min).x;
    //         }else{
    //             xMaxPreviousImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.max).y;
    //             xMinPreviousImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.min).y;
    //         }
    //         //Checks if the object is out of the cameras view when its at the left of the camera
    //         if(xMaxPreviousImage <= lowerBoundLimit){
    //             GameObject.Destroy(previousImage);
    //         }
    //         //Checks if the object is out of the cameras view when its at the right of the camera
    //         if(xMinPreviousImage >= upperBoundLimit){
    //             GameObject.Destroy(previousImage);
    //         }
    //     }

    //     //does the same as the last two comments above but instead checks the next image infront of the current image
    //     if(nextImage != null){
    //         float MaxNextImage;
    //         float MinNextImage;

    //         if(isDependantOnXAxis){
    //             MaxNextImage = Camera.main.WorldToViewportPoint(_nextCol.bounds.max).x;
    //             MinNextImage = Camera.main.WorldToViewportPoint(_nextCol.bounds.min).x;
    //         }else{
    //             MaxNextImage = Camera.main.WorldToViewportPoint(_nextCol.bounds.max).y;
    //             MinNextImage = Camera.main.WorldToViewportPoint(_nextCol.bounds.min).y;
    //         }
    //         if(MinNextImage > upperBoundLimit){
    //             GameObject.Destroy(nextImage);
    //         }
    //         if(MaxNextImage < lowerBoundLimit){
    //             GameObject.Destroy(nextImage);

    //         }
    //     }
    // }
    public void setIsLooping(bool val) { isLooping = val; }
    public bool getIsLooping() { return isLooping; }

    private void OnDrawGizmosSelected()
    {
        if (isDependantOnXAxis)
        {
            Vector2 upperPosition1 = _camera.ViewportToWorldPoint(new Vector2(upperBoundLimit, _camera.rect.min.y));
            Vector2 upperPosition2 = _camera.ViewportToWorldPoint(new Vector2(upperBoundLimit, _camera.rect.max.y));
            Vector2 lowerPosition1 = _camera.ViewportToWorldPoint(new Vector2(lowerBoundLimit, _camera.rect.min.y));
            Vector2 lowerPosition2 = _camera.ViewportToWorldPoint(new Vector2(lowerBoundLimit, _camera.rect.max.y));
            Gizmos.DrawLine(upperPosition1, upperPosition2);
            Gizmos.DrawLine(lowerPosition1, lowerPosition2);
        }
        else
        {
            Vector2 upperPosition1 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.min.x, upperBoundLimit));
            Vector2 upperPosition2 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.max.x, upperBoundLimit));
            Vector2 lowerPosition1 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.min.x, lowerBoundLimit));
            Vector2 lowerPosition2 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.max.x, lowerBoundLimit));
            Gizmos.DrawLine(upperPosition1, upperPosition2);
            Gizmos.DrawLine(lowerPosition1, lowerPosition2);
        }
    }
    
    public void setStopOnNext(bool val){ stopOnNext = val; }
}
