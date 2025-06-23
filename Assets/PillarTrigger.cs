using System;
using System.Collections;
using UnityEngine;

public class PillarTrigger : MonoBehaviour
{
    Vector2 startingPosition;
    
    [SerializeField] private float maxDistance;
    [SerializeField] private float time, startingTime;

    private Coroutine respawnCoro;
    private GameObject spriteObj;

    void OnValidate() => startingPosition = transform.position;

    // Start is called before the first frame update
    void Start()
    {
        //Subscribes the Ienumerator FallingPillar to the event in PillarManager
        PillarManager.current.startTrigger += wrapperFallingPillar;

        startingPosition = transform.position;

        //Gets the first child in the 
        spriteObj = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (PillarManager.current.startPuzzle)
        {
            if (transform.position != (Vector3)startingPosition)
            {
                //Checks if the pillar is at the max distance with it's original position
                if (Vector2.Distance(startingPosition, transform.position) >= maxDistance)
                {
                    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                }
            }
        }
    }

    //This turns off and on the object in a certain time frame. Also replays the FallingPillar method.
    private IEnumerator PillarLoop()
    {
        yield return new WaitForSecondsRealtime(time);
        spriteObj.SetActive(false);
        gameObject.transform.position = startingPosition;
        yield return new WaitForSecondsRealtime(2f);
        spriteObj.SetActive(true);
        yield return FallingPillar();
        respawnCoro = null;
    }

    private IEnumerator FallingPillar()
    {
        yield return new WaitForSecondsRealtime(startingTime);
        changePhysics();
    }

    //Wrapper for the Ienumerator FallingPillar because events dont like Ienumerator.
    private void wrapperFallingPillar() => StartCoroutine(FallingPillar());

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (respawnCoro == null) { respawnCoro = StartCoroutine(PillarLoop()); }
        }
    }

    private void changePhysics() => GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(startingPosition, new Vector2(startingPosition.x, startingPosition.y - maxDistance));
    }
    void OnDestroy()
    {
        //Unscribes the method so it doesn't cause an error if it gets deleted mid game.
        PillarManager.current.startTrigger -= wrapperFallingPillar;
    }
}
