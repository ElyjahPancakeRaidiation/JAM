using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LoopingBackGround1 : MonoBehaviour
{
    [SerializeField]protected GameObject imagePrefab;
    [SerializeField]protected GameObject curImage;
    [SerializeField]protected GameObject previousImage, nextImage;

    public static bool isLooping;

    [SerializeField]private bool isDependantOnXAxis;

    [SerializeField]protected Vector2 offset;
    [SerializeField]private Vector3 rotationOffset;

    public bool stopOnNext;

    /// <summary>
    /// upperBoundLimit - The Maximum it can go before looping. Think of it as the far right of the camera or the top part of the camera. 
    /// Highest is usually 1 or greater for both right and up
    /// 
    /// lowerBoundLimit - The minimum it can go before looping. Think of it as the far left of the camera or the bottom part of the camera. 
    /// Highest is usually 0 or less for both left and down
    /// </summary>
    [SerializeField]private float upperBoundLimit;
    [SerializeField]private float lowerBoundLimit;

    protected Collider2D _currentCol;
    private Collider2D _previousCol;
    protected Collider2D _nextCol;

    protected GameObject player;
    public GameObject test;
    // Start is called before the first frame update
    void Start()
    {
        _currentCol = curImage.GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    // Update is called once per frame
    void Update()
    {

        //This block switches the current image with what ever bounds the player is currently in while setting the previous image to the previous current image.
        CheckForCurrentImage();

        CheckForPassedImage();

        //These two variables are the spawn positions for the left and right background
        Vector3 rightPosition = new Vector3(_currentCol.bounds.max.x + _currentCol.bounds.extents.x, curImage.transform.position.y);
        Vector3 leftPosition = new Vector3(_currentCol.bounds.min.x - _currentCol.bounds.extents.x, curImage.transform.position.y);
        //These two variables are the current images far right and left positions
        float xMaxCurrentImage = Camera.main.WorldToViewportPoint(_currentCol.bounds.max).x;
        float xMinCurrentImage = Camera.main.WorldToViewportPoint(_currentCol.bounds.min).x;
        
        spawnObjectAfterAnother(xMaxCurrentImage, xMinCurrentImage, rightPosition + (Vector3)offset, leftPosition + (Vector3)offset);//Need to change this to make it more accomidating for the x and y positions -- Note: Later
    }

    public void CheckForCurrentImage(){
        if(nextImage != null){
            if(player.transform.position.x >= _nextCol.bounds.min.x && player.transform.position.x <= _nextCol.bounds.max.x){
                previousImage = curImage;
                _previousCol = _currentCol;
                curImage = _nextCol.gameObject;
                _currentCol = _nextCol.GetComponent<Collider2D>();
                _nextCol = null;
                nextImage = null;
            }
        }
    }

    public void spawnObjectAfterAnother(float maxCoord, float minCoord, Vector3 rightPosition, Vector3 leftPosition){

        //This section spawns the next image depending where the cameras borders are going.
        if(maxCoord <= upperBoundLimit){//If the camera is going right next image to the right
            if(nextImage == null){
                nextImage = Instantiate(imagePrefab, rightPosition, Quaternion.identity);
                _nextCol = nextImage.GetComponent<Collider2D>();
            }
        }

        if(minCoord >= lowerBoundLimit){//If the camera is going left next image to the left
            if(nextImage == null){
                nextImage = Instantiate(imagePrefab, leftPosition, Quaternion.identity);
                _nextCol = nextImage.GetComponent<Collider2D>();
            }
        }
    }
    
    public void CheckForPassedImage(){
        if(previousImage != null){
            //These two variables are the previous images far right and left positions
            float xMaxPreviousImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.max).x;
            float xMinPreviousImage = Camera.main.WorldToViewportPoint(_previousCol.bounds.min).x;
            //Checks if the object is out of the cameras view when its at the left of the camera
            if(xMaxPreviousImage <= lowerBoundLimit){
                GameObject.Destroy(previousImage);
            }
            //Checks if the object is out of the cameras view when its at the right of the camera
            if(xMinPreviousImage >= upperBoundLimit){
                GameObject.Destroy(previousImage);
            }
        }

        //does the same as the last two comments above but instead checks the next image infront of the current image
        if(nextImage != null){
            if(Camera.main.WorldToViewportPoint(_nextCol.bounds.min).x > upperBoundLimit){
                GameObject.Destroy(nextImage);
            }
            if(Camera.main.WorldToViewportPoint(_nextCol.bounds.max).x < lowerBoundLimit){
                GameObject.Destroy(nextImage);
                
            }
        }
    }

}
