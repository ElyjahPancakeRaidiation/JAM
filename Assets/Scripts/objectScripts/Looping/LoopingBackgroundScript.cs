using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LoopingBackgroundScript : MonoBehaviour
{

    private Camera _camera;

    [SerializeField] private GameObject[] imageToLoop;
    [SerializeField] protected GameObject startInstance;
    [SerializeField] protected bool isDependantOnXAxis;
    [SerializeField] protected bool stopOnNext;
    [SerializeField] private bool hasSpawnPosition;
    [SerializeField] private GameObject spawnPosition;
    [SerializeField] private Vector2 curMaxOffset;
    [SerializeField] private Vector2 curMinOffset;

    [SerializeField ]protected bool useCol;
    [SerializeField ]private bool testBool;
    protected bool canLoop;
    protected bool loopended;
    private bool multipleObjects;
    [SerializeField] private bool exactEndPosition; 

    [SerializeField] private float cameraUpperLimit;
    [SerializeField] private float cameraLowerLimit;
    [SerializeField] private int maxWaitTime;
    [SerializeField] Vector2 offsets;

    private Coroutine loopingCoroutine;

    public class ObjectInfo
    {
        private GameObject curObject;
        private Bounds bounds;
        private bool useCol;

        public ObjectInfo(bool useCol=false)
        {
            curObject = null;
            this.useCol = useCol;
        }
        public ObjectInfo(GameObject curObject, bool useCol=false)
        {
            this.curObject = curObject;
            this.useCol = useCol;
        }

        public void setCurObject(GameObject val){curObject = val;}
        public GameObject getCurObject(){ return curObject; }

        public Vector3 getBoundsMin()
        {
            if (useCol)
            {
                return curObject.GetComponent<Collider2D>().bounds.min;
            }

            return curObject.GetComponent<SpriteRenderer>().bounds.min;
        }
        public Vector3 getBoundsMax()
        {
            if (useCol)
            {
                return curObject.GetComponent<Collider2D>().bounds.max;
            }

            return curObject.GetComponent<SpriteRenderer>().bounds.max;
        }

        public Vector3 SpawnCoordRight(Vector3 max, Vector3 min) { return max + (curObject.transform.position - min); }
        public Vector3 SpawnCoordLeft(Vector3 max, Vector3 min){return min + (curObject.transform.position - max);}
    }

    protected ObjectInfo currentObj;
    protected ObjectInfo newObj;



    private void OnValidate()
    {
        _camera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        loopended = false;
        if (imageToLoop.Length > 1) { multipleObjects = true; }
        currentObj = new ObjectInfo(startInstance, useCol);
        newObj = new ObjectInfo(useCol);
    }

    // Update is called once per frame
    void Update()
    {
        if (canLoop)
        {
            CheckToDelete();
            CheckToSpawn();
        }
    }

    public virtual void CheckToDelete()
    {
        if (!isDependantOnXAxis)
        {
            //For the Y axis
            if (newObj.getCurObject() != null)
            {
                if (_camera.WorldToViewportPoint(currentObj.getBoundsMin() + (Vector3)curMinOffset).y > cameraUpperLimit)
                {
                    Debug.Log("DAMN BRO STOP LICKKING SO LOUD");
                    Destroy(currentObj.getCurObject());
                    currentObj.setCurObject(newObj.getCurObject());
                    newObj.setCurObject(null);
                }
            }
        }
        else
        {
            if (newObj.getCurObject() != null)
            {
            
                if (_camera.WorldToViewportPoint(currentObj.getBoundsMax() + (Vector3)curMaxOffset).x < cameraLowerLimit)
                {
                    Destroy(currentObj.getCurObject());
                    currentObj.setCurObject(newObj.getCurObject());
                    newObj.setCurObject(null);

                }
            }
        }
    }

    public IEnumerator CheckToSpawnCoroutine()
    {
        if (!isDependantOnXAxis)
        {
            //For Y axis
            if (newObj.getCurObject() == null)
            {
                if (_camera.WorldToViewportPoint(currentObj.getBoundsMax() + (Vector3)curMaxOffset).y > cameraUpperLimit)//Come back to this problem: will spawn no matter what if max.y > upper && min.y < lower
                {
                    if (hasSpawnPosition)
                    {
                        if (!multipleObjects)
                        {
                            //41.3
                            float y = spawnPosition.transform.position.y;
                            if (testBool) { y = currentObj.SpawnCoordLeft(currentObj.getBoundsMax() + (Vector3)curMaxOffset, currentObj.getBoundsMin() + (Vector3)curMinOffset).y + offsets.y; }
                            if(exactEndPosition && stopOnNext){y = spawnPosition.transform.position.y;}
                            newObj.setCurObject(Instantiate(imageToLoop[0], new Vector2(spawnPosition.transform.position.x, y), Quaternion.identity));
                        }
                        else
                        {
                            int randomObj = Random.Range(0, imageToLoop.Length);
                            // Debug.Log(randomObj);
                            newObj.setCurObject(Instantiate(imageToLoop[randomObj], spawnPosition.transform.position, Quaternion.identity));
                        }
                    }
                    else
                    {
                        //Put in the automatic version.
                    }
                    
                    if (stopOnNext && currentObj.getCurObject() != null && newObj.getCurObject() != null)
                    {
                        canLoop = false;
                        loopended = true;
                    }
                    else if (!stopOnNext && multipleObjects)
                    {

                        yield return new WaitForSecondsRealtime(Random.Range(0, maxWaitTime));
                    }
                }
            }
            
        }
        else
        {

            if (newObj.getCurObject() == null)
            {

                if (_camera.WorldToViewportPoint(currentObj.getBoundsMax() + (Vector3)curMaxOffset).x < cameraUpperLimit)//Come back to this problem: will spawn no matter what if max.y > upper && min.y < lower
                {
                    if (hasSpawnPosition)
                    {
                        if (!multipleObjects)
                        {
                            float x = currentObj.SpawnCoordRight(currentObj.getBoundsMax() + (Vector3)curMaxOffset, currentObj.getBoundsMin() + (Vector3)curMinOffset).x;
                            newObj.setCurObject(Instantiate(imageToLoop[0], new Vector2(x + offsets.x, spawnPosition.transform.position.y), Quaternion.identity));
                            if(testBool){ TestManager.isPaused = true; }
                        }
                        else
                        {
                            int randomObj = Random.Range(0, imageToLoop.Length - 1);
                            newObj.setCurObject(Instantiate(imageToLoop[randomObj], spawnPosition.transform.position, Quaternion.identity));
                        }
                    }
                    else
                    {
                        //Put in the automatic version.
                    }
                    
                    if (stopOnNext && currentObj.getCurObject() != null && newObj.getCurObject() != null)
                    {
                        canLoop = false;
                        loopended = true;
                    }
                    else if (!stopOnNext && multipleObjects)
                    {

                        yield return new WaitForSecondsRealtime(Random.Range(0, maxWaitTime));
                    }
                }
            }

        }

        loopingCoroutine = null;
    }

    public virtual void CheckToSpawn()
    {
        if (imageToLoop.Length > 1) { multipleObjects = true; }
        if (loopingCoroutine == null)
        {
            loopingCoroutine = StartCoroutine(CheckToSpawnCoroutine());
        }
    }

    private void OnDrawGizmos()
    {
        if (isDependantOnXAxis)
        {
            Vector2 upperPosition1 = _camera.ViewportToWorldPoint(new Vector2(cameraUpperLimit, _camera.rect.min.y));
            Vector2 upperPosition2 = _camera.ViewportToWorldPoint(new Vector2(cameraUpperLimit, _camera.rect.max.y));
            Vector2 lowerPosition1 = _camera.ViewportToWorldPoint(new Vector2(cameraLowerLimit, _camera.rect.min.y));
            Vector2 lowerPosition2 = _camera.ViewportToWorldPoint(new Vector2(cameraLowerLimit, _camera.rect.max.y));
            Gizmos.DrawLine(upperPosition1, upperPosition2);
            Gizmos.DrawLine(lowerPosition1, lowerPosition2);
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

    public void setStopOnNext(bool val) { stopOnNext = val; }
    public void setCanLoop(bool val){ canLoop = val; }
    

}

