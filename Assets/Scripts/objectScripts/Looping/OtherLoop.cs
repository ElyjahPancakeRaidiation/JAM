using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OtherLoop : LoopingBackGround1
{

    [Tooltip("Once the player gets close to this object it will slowly move to the object and freeze the axis chosen"), SerializeField]
    private Transform freezePlayerPosition;
    private float wantedAxisFreezePosition;

    [Tooltip("Gives the player a little bit of wiggle room before freezing in case it is not exactly on the object."), SerializeField]
    private float wiggleRoom;

    [SerializeField]private bool shouldPlayerFreeze;
    private bool freezePlayer, canFreezePlayer;
    [SerializeField]private CutSceneManager wantedCutSceneObject;

    
    // Start is called before the first frame update
    void Start()
    {
        _previousCol = previousImage.GetComponent<Collider2D>();
        previousImage.GetComponent<MovingGround>().setCanMove(false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(!getIsLooping()){
            // player.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
            if(previousImage != null){previousImage.GetComponent<MovingGround>().setCanMove(false);}
            if(curImage != null){curImage.GetComponent<MovingGround>().setCanMove(false);}
        }
        //Cur objective - Make the player only follow its y axis while being stuck in the x axis of another assigned object. This will be done with _rb.MovePosition.
        //How will we make it where once the player enters the vicinity of the object it will clamp the players x?
        
        if(shouldPlayerFreeze && !canFreezePlayer){

            if(!freezePlayer){
                var dist = false;
                if(!isDependantOnXAxis){
                    dist = Vector2.Distance(new Vector2(0, player.transform.position.y), new Vector2(0, freezePlayerPosition.transform.position.y)) < wiggleRoom;
                }else{dist = Vector2.Distance(new Vector2(player.transform.position.x, 0), new Vector2(freezePlayerPosition.transform.position.x, 0)) < wiggleRoom;}


                if(dist && !stopOnNext){
                    //flingPlayerScript.player.transform.position = Vector2.Lerp(flingPlayerScript.player.transform.position, freezeobject.position, 3);
                    previousImage.GetComponent<MovingGround>().setCanMove(true);
                    freezePlayer = true;
                    setIsLooping(true);
                }
                
            }

        }
        


        if(getIsLooping()){
            CheckForCurrentImage();

            float maxCoord = Camera.main.WorldToViewportPoint(_previousCol.bounds.max).y; 
            float minCoord = Camera.main.WorldToViewportPoint(_previousCol.bounds.min).y;
            
            Vector3 downPosition = new Vector3(_previousCol.bounds.max.x + offset.x, (_previousCol.bounds.min.y -_previousCol.bounds.extents.y)+offset.y);
            spawnObjectAfterAnother(maxCoord, minCoord, downPosition, downPosition);
        }

        if(stopOnNext && !wantedCutSceneObject.getIsFinished()){
            if(previousImage != null && curImage != null){
                wantedCutSceneObject.playCutScene();
            }
        }

    }

    void FixedUpdate()
    {
        if(getIsLooping()){
            // player.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None;
            if(wantedAxisFreezePosition == 0){wantedAxisFreezePosition = player.transform.position.x;}
            player.GetComponent<Rigidbody2D>().position = new Vector2(wantedAxisFreezePosition, player.GetComponent<Rigidbody2D>().position.y);
        }
    }
}
