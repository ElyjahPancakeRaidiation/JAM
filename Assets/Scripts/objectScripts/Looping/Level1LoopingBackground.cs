using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1LoopingBackground : LoopingBackgroundScript
{

    //This cutscene will play at the end of the loop(just adds force to the player giving it the feel of the player moving down a hill)
    [SerializeField] private CutSceneManager endLoopCutscene;
    [SerializeField] private GameObject treeCoverUpObj;
    [SerializeField] private GameObject freezePlayerPosition;
    [SerializeField] private float wiggleRoom;
    private float wantedAxisFreezePosition;
    private GameObject player;
    [SerializeField]private GameObject bigAssObjectAhhh;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentObj = new ObjectInfo(startInstance, useCol);
        newObj = new ObjectInfo(useCol);
        currentObj.getCurObject().GetComponent<MovingGround>().setCanMove(false);
    }

    // Update is called once per frame
    void Update()
    {
        var dist = false;
        if (!isDependantOnXAxis)
        {
            dist = Vector2.Distance(new Vector2(0, player.transform.position.y), new Vector2(0, freezePlayerPosition.transform.position.y)) < wiggleRoom;
        }
        else { dist = Vector2.Distance(new Vector2(player.transform.position.x, 0), new Vector2(freezePlayerPosition.transform.position.x, 0)) < wiggleRoom; }


        if (dist && !loopended)
        {
            //flingPlayerScript.player.transform.position = Vector2.Lerp(flingPlayerScript.player.transform.position, freezeobject.position, 3);
            currentObj.getCurObject().GetComponent<MovingGround>().setCanMove(true);
            // bigAssObjectAhhh.SetActive(true);
            bigAssObjectAhhh.GetComponent<MovingGround>().setCanMove(true);
            canLoop = true;
        }

        if (stopOnNext)
        {
            treeCoverUpObj.SetActive(true);
        }

        
        if (canLoop)
        {
            CheckToDelete();
            CheckToSpawn();
        }

        if (loopended)
        {
            currentObj.getCurObject().GetComponent<MovingGround>().setCanMove(false);
            newObj.getCurObject().GetComponent<MovingGround>().setCanMove(false);
            bigAssObjectAhhh.GetComponent<MovingGround>().setCanMove(false);
        }

        if (loopended && !endLoopCutscene.getIsFinished())
        {
            endLoopCutscene.playCutScene();
        }
    }

    public override void CheckToDelete()
    {
        base.CheckToDelete();
    }

    public override void CheckToSpawn()
    {
        base.CheckToSpawn();
    }

    private void FixedUpdate()
    {
        if (!loopended && canLoop)
        {
            if(wantedAxisFreezePosition == 0){wantedAxisFreezePosition = player.transform.position.x;}
            player.GetComponent<Rigidbody2D>().position = new Vector2(wantedAxisFreezePosition, player.GetComponent<Rigidbody2D>().position.y);
        }
    }
    
}
