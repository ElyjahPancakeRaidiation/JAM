using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level1LoopingBackground : LoopingBackgroundScript
{

    //This cutscene will play at the end of the loop(just adds force to the player giving it the feel of the player moving down a hill)
    [SerializeField] private GameObject startingNewObject;
    [SerializeField] private CutSceneManager endLoopCutscene;
    [SerializeField] private GameObject treeCoverUpObj;
    [SerializeField] private GameObject freezePlayerPosition;
    [SerializeField] private float wiggleRoom;
    private float wantedAxisFreezePosition;
    private GameObject player;
    [SerializeField] private GameObject bigAssObjectAhhh;
    // [SerializeField] private Camera _staticCamera;
    private bool dist;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        currentObj = new ObjectInfo(startInstance, useCol);
        newObj = new ObjectInfo(startingNewObject, useCol);
        currentObj.getCurObject().GetComponent<MovingGround>().setCanMove(false);
        newObj.getCurObject().GetComponent<MovingGround>().setCanMove(false);
        bigAssObjectAhhh.GetComponent<MovingGround>().setCanMove(false);
        // _staticCamera.orthographicSize = _camera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {

        if (!loopended)
        {
            if (!canLoop)
            {
                if (!isDependantOnXAxis)
                {
                    dist = Vector2.Distance(new Vector2(0, player.transform.position.y), new Vector2(0, freezePlayerPosition.transform.position.y)) < wiggleRoom;
                }
                else { dist = Vector2.Distance(new Vector2(player.transform.position.x, 0), new Vector2(freezePlayerPosition.transform.position.x, 0)) < wiggleRoom; }

                if (dist)
                {
                    currentObj.getCurObject().GetComponent<MovingGround>().setCanMove(true);
                    newObj.getCurObject().GetComponent<MovingGround>().setCanMove(true);
                    bigAssObjectAhhh.GetComponent<MovingGround>().setCanMove(true);
                    canLoop = true;
                }
            }
            else
            {
                CheckToLoop(currentObj.getViewportMin(_camera, curMinOffset), newObj.getViewportMin(_camera, curMinOffset));
            }
        }

        if (loopended && !endLoopCutscene.getIsFinished())
        {
            endLoopCutscene.playCutScene();
        }
    }

    private void CheckToLoop(Vector3 curMin, Vector3 newMin)
    {
        // if (!beingPlayed2)
        // {
        //     testtext.text += "I'M BEING PLAYED?!?!'";
            
        // }
        if (curMin.y > cameraUpperLimit && newMin.y >= cameraLowerLimit)
        {
            GameObject obj = currentObj.getCurObject();
            currentObj.setCurObject(newObj.getCurObject());
            newObj.setCurObject(obj);
            newObj.getCurObject().transform.position = spawnPosition.transform.position;
            if (stopOnNext)
            {
                treeCoverUpObj.SetActive(true);
                canLoop = false;
                loopended = true;
                currentObj.getCurObject().GetComponent<MovingGround>().setCanMove(false);
                newObj.getCurObject().GetComponent<MovingGround>().setCanMove(false);
                bigAssObjectAhhh.GetComponent<MovingGround>().setCanMove(false);
                player.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }

    }

    private void FixedUpdate()
    {
        if (!loopended && canLoop)
        {
            if (player.GetComponent<Rigidbody2D>().interpolation == RigidbodyInterpolation2D.Interpolate) { player.GetComponent<Rigidbody2D>().interpolation = RigidbodyInterpolation2D.None; }
            if (wantedAxisFreezePosition == 0) { wantedAxisFreezePosition = player.transform.position.x; }
            player.GetComponent<Rigidbody2D>().position = new Vector2(wantedAxisFreezePosition, player.GetComponent<Rigidbody2D>().position.y);
        }
    }

    
}
