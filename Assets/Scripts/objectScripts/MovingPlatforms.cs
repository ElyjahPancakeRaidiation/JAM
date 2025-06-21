using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] public List<GameObject> upperPrefabObstacles;
    [SerializeField] public List<GameObject> lowerPrefabObstacles;
    private GameObject activePlatform;
    private GameObject player;
    [SerializeField] private new BoxCollider2D collider; //yo ima be honest i only put the new keyword so that vsc could SHUT UP
    private GameObject globalLight;
    private PuzzleLighting puzzleLighting;
    private bool playerWithin = false;
    private bool playingReload = false;
    [SerializeField] private float padding;
    [SerializeField] private float paddingLeft;
    [SerializeField] private float paddingRight;
    [SerializeField] private float timeBetweenLightning;
    [SerializeField] private float rangeOfObstacleGap;
    [SerializeField] private float rangeOfVerticality;
    public bool inQueue = false;
    public float queueTime = 2.0f;
    private enum Verticality
    {
        UPPER,
        LOWER,
        PENIS,
        ELYJAH,
        PENIS2,
        MOHAMMED,
        PENIS3
    }

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        globalLight = GameObject.FindGameObjectWithTag("GlobalLighting");
        puzzleLighting = globalLight.GetComponent<PuzzleLighting>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (!playerWithin && checkPlayerWithin() && !inQueue)
        {
            playerWithin = true;
            StartCoroutine(initialize());
            Debug.Log("Entered");
        }
        else if (playerWithin && !checkPlayerWithin() && !inQueue)
        {
            playerWithin = false;
            StartCoroutine(end());
            Debug.Log("exited");
        }
        playerWithin = checkPlayerWithin();
    }
    private GameObject loadObstacleWorldSpace(GameObject prefab, Vector3 position)
    {
        GameObject obstacle = Instantiate(prefab, position, Quaternion.identity, transform);
        obstacle.AddComponent<PlatformScript>();
        return obstacle;
    }
    private IEnumerator reloadPlatforms()
    {
        playingReload = true;
        while (playerWithin)
        {
            yield return new WaitForSeconds(timeBetweenLightning);
            if (!playerWithin) { break; }
            activePlatform = null;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<PlatformScript>().IsActive() && activePlatform == null)
                {
                    activePlatform = child.gameObject;
                }
                else
                {
                    Destroy(child.gameObject);
                }
            }
            if (activePlatform != null)
            {
                randomizeObstaclesInRange(collider.bounds.min.x + paddingLeft, activePlatform.GetComponent<PolygonCollider2D>().bounds.min.x);
                randomizeObstaclesInRange(activePlatform.GetComponent<PolygonCollider2D>().bounds.max.x, collider.bounds.max.x - paddingRight);
            }
            else
            {
                randomizeObstaclesInRange(collider.bounds.min.x + paddingLeft, collider.bounds.max.x - paddingRight);
            }
            yield return StartCoroutine(puzzleLighting.lightning());
        }
        playingReload = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size - new Vector3(paddingLeft + paddingRight, 0, 0));
        Gizmos.DrawLine(collider.bounds.center - new Vector3(collider.bounds.size.x / 2 - paddingLeft, -collider.bounds.min.y, 0), collider.bounds.center - new Vector3(collider.bounds.size.x / 2 - paddingLeft, -collider.bounds.max.y, 0));
        Gizmos.DrawLine(collider.bounds.center + new Vector3(collider.bounds.size.x / 2 - paddingRight, collider.bounds.min.y, 0), collider.bounds.center + new Vector3(collider.bounds.size.x / 2 - paddingLeft, collider.bounds.max.y, 0));
        Gizmos.DrawLine(collider.bounds.center - new Vector3(collider.bounds.size.x / 2 - paddingLeft, 0, 0), collider.bounds.center + new Vector3(collider.bounds.size.x / 2 - paddingRight, 0, 0));
    }
    private IEnumerator initialize()
    {
        inQueue = true;
        yield return StartCoroutine(puzzleLighting.startLighting());
        StartCoroutine(reloadPlatforms());
        inQueue = false;
    }
    private IEnumerator end()
    {
        inQueue = true;
        Debug.Log("end ran");
        StopCoroutine(reloadPlatforms());
        yield return new WaitUntil(() => !puzzleLighting.playingLightning);
        yield return StartCoroutine(puzzleLighting.stopLighting());
        inQueue = false;
    }
    public void randomizeObstaclesInRange(float start, float end)
    {
        float x = start + UnityEngine.Random.Range(0, rangeOfObstacleGap); //starting point
        while (x < end)
        {
            Verticality verticality = (Verticality)UnityEngine.Random.Range(0, 2); //choose randomly between upper and lower
            List<GameObject> currentPrefabs = verticality == Verticality.UPPER ? upperPrefabObstacles : lowerPrefabObstacles;
            int prefabIndex = UnityEngine.Random.Range(0, currentPrefabs.Count); //choose a random prefab obstacle from the list
            float randomXGap = UnityEngine.Random.Range(0, rangeOfObstacleGap);
            float randomYShift = UnityEngine.Random.Range(0, rangeOfVerticality);


            Vector3 obstacleSize = currentPrefabs[prefabIndex].GetComponent<PolygonCollider2D>().bounds.size;
            float alignmentYOffset = (verticality == Verticality.UPPER ? -1 : 1) * (obstacleSize.y / 2 + randomYShift); //trying to get the surfaces to be aligned with the center of the puzzle bounds
            float alignmentXOffset = obstacleSize.x / 2; //want to spawn gameobjects with lefthand surface touching the cursor's x, typically spawning objects centers them at that coord
            if (x + obstacleSize.x > end) { break; } //if the x position we're currently at will put the object out of bounds, break

            Vector3 cursor = new Vector3(x + alignmentXOffset, collider.bounds.center.y - alignmentYOffset, 0); //thought of a mouse cursor drag and dropping gameobjects

            GameObject obstacle = loadObstacleWorldSpace(currentPrefabs[prefabIndex], cursor);
            x += obstacleSize.x + randomXGap;
        }
    }
    private bool checkPlayerWithin()
    {
        return Physics2D.OverlapBox(collider.bounds.center, collider.bounds.size, 0f, LayerMask.GetMask("Player")) != null;
    }
}

//notes for tomorrow
// why is initializing running more than once sometimes, works pretty well as of now