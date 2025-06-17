using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundLooper : LoopingBackgroundScript
{

    private GameObject player;
    [SerializeField] private GameObject freezePlayerPosition;
    [SerializeField] private float wiggleRoom;


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
            canLoop = true;
        }

        if (canLoop)
        {
            CheckToDelete(_camera.WorldToViewportPoint(currentObj.getBoundsMin() + (Vector3)curMinOffset), _camera.WorldToViewportPoint(currentObj.getBoundsMax() + (Vector3)curMaxOffset));
            CheckToSpawn(_camera.WorldToViewportPoint(currentObj.getBoundsMin() + (Vector3)curMinOffset), _camera.WorldToViewportPoint(currentObj.getBoundsMax() + (Vector3)curMaxOffset));
        }

        if (loopended)
        {
            Destroy(currentObj.getCurObject());
            Destroy(newObj.getCurObject());
        }
    }
}
