using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LoopingBackgroundScript : MonoBehaviour
{

    private Camera _camera;

    [SerializeField] private GameObject imageToLoop;
    [SerializeField] protected GameObject currentInstance;
    protected GameObject newInstance;
    [SerializeField] protected bool isDependantOnXAxis;
    [SerializeField] private bool stopOnNext;
    [SerializeField] private bool hasSpawnPosition;
    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private Vector2 curMaxOffset;
    [SerializeField] private Vector2 curMinOffset;

    protected bool canLoop;
    protected bool loopended;

    [SerializeField] private float cameraUpperLimit;
    [SerializeField] private float cameraLowerLimit;

    private Vector2 curInstanceMax;
    private Vector2 curInstanceMin;
    private Vector2 curInstanceExtents;

    private Vector2 newInstanceMax;
    private Vector2 newInstanceMin;
    private Vector2 newInstanceExtents;


    private void OnValidate()
    {
        _camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        loopended = false;
        Updateinstances();
    }

    // Update is called once per frame
    void Update()
    {
        if (canLoop)
        {
            Updateinstances();
            CheckToDelete();
            CheckToSpawn();
        }
    }


    public void Updateinstances()
    {
        if (currentInstance != null)
        {
            curInstanceMax = _camera.WorldToViewportPoint(currentInstance.GetComponent<SpriteRenderer>().bounds.max + (Vector3)curMaxOffset);
            curInstanceMin = _camera.WorldToViewportPoint(currentInstance.GetComponent<SpriteRenderer>().bounds.min + (Vector3)curMinOffset);
            curInstanceExtents = _camera.WorldToViewportPoint(currentInstance.GetComponent<SpriteRenderer>().bounds.extents);
        }

        if (newInstance != null)
        {
            newInstanceMax = _camera.WorldToViewportPoint(newInstance.GetComponent<SpriteRenderer>().bounds.max);
            newInstanceMin = _camera.WorldToViewportPoint(newInstance.GetComponent<SpriteRenderer>().bounds.min);
            newInstanceExtents = _camera.WorldToViewportPoint(newInstance.GetComponent<SpriteRenderer>().bounds.extents);
        }
    }

    public virtual void CheckToDelete()
    {
        if (!isDependantOnXAxis)
        {
            //For the Y axis
            if (newInstance != null)
            {
                //Checks to see if the current instance lowest point of the sprite is past the upper limit or if the highest point past the lowest limit
                //if lowest point past the upper limit the looping object will either be going up or right
                //if highest point past the lowest limit the looping object will either be going down or left
                if (curInstanceMin.y > cameraUpperLimit)
                {
                    Destroy(currentInstance);
                    currentInstance = newInstance;
                    newInstance = null;
                    Updateinstances();
                }
            }
        }
    }

    public virtual void CheckToSpawn()
    {
        if (!isDependantOnXAxis)
        {
            //For Y axis
            if (newInstance == null)
            {

                if (curInstanceMax.y > cameraUpperLimit)//Come back to this problem: will spawn no matter what if max.y > upper && min.y < lower
                {
                    if (hasSpawnPosition)
                    {
                        newInstance = Instantiate(imageToLoop, spawnPosition.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        //Put in the automatic version.
                    }
                    Updateinstances();
                    if (stopOnNext)
                    {
                        canLoop = false;
                        loopended = true;
                    }
                }
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (isDependantOnXAxis)
        {
            Vector2 upperPosition = _camera.ViewportToWorldPoint(new Vector2(cameraUpperLimit, _camera.rect.max.y));
            Vector2 lowerPosition = _camera.ViewportToWorldPoint(new Vector2(cameraLowerLimit, _camera.rect.min.y));
            Gizmos.DrawLine(upperPosition, lowerPosition);
        }
        else
        {
            Vector2 upperPosition1 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.min.x, cameraUpperLimit));
            Vector2 upperPosition2 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.max.x, cameraUpperLimit));
            Vector2 lowerPosition1 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.min.x, cameraLowerLimit));
            Vector2 lowerPosition2 = _camera.ViewportToWorldPoint(new Vector2(_camera.rect.max.x, cameraLowerLimit));
            Gizmos.DrawLine(upperPosition1, upperPosition2);
            Gizmos.DrawLine(lowerPosition1, lowerPosition2);
        }
    }
    

}

