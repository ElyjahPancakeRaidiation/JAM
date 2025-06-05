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
    [SerializeField] private new BoxCollider2D collider; //yo ima be honest i only put the new keyword so that vsc could SHUT UP
    private GameObject globalLight;
    private PuzzleLighting puzzleLighting;
    private bool playerWithin = false;
    private bool playingReload = false;
    [SerializeField] private float padding;
    [SerializeField] private float timeBetweenLightning;
    [SerializeField] private float rangeOfObstacleGap;
    [SerializeField] private float rangeOfVerticality;
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
    }
    void Update()
    {
    }
    private GameObject loadObstacleWorldSpace(GameObject prefab, Vector3 position)
    {
        GameObject obstacle = Instantiate(prefab, position, Quaternion.identity, transform);
        obstacle.AddComponent<PlatformScript>();
        return obstacle;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerWithin = true;
            if (!playingReload)
            {
                StartCoroutine(initialize());
            }
        }
    }
    private IEnumerator reloadPlatforms()
    {
        playingReload = true;
        while (playerWithin)
        {
            //updateAllPlatforms();
            yield return new WaitForSeconds(timeBetweenLightning);
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
                randomizeObstaclesInRange(collider.bounds.min.x + padding, activePlatform.GetComponent<PolygonCollider2D>().bounds.min.x, 0);
                randomizeObstaclesInRange(activePlatform.GetComponent<PolygonCollider2D>().bounds.max.x, collider.bounds.max.x - padding, 1);
            }
            else
            {
                randomizeObstaclesInRange(collider.bounds.min.x + padding, collider.bounds.max.x - padding);
            }
            yield return StartCoroutine(puzzleLighting.lightning());
        }
        playingReload = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size - new Vector3(padding * 2, 0, 0));
        Gizmos.DrawLine(collider.bounds.center - new Vector3(collider.bounds.size.x / 2 - padding, 0, 0), collider.bounds.center + new Vector3(collider.bounds.size.x / 2 - padding, 0, 0));
    }
    private IEnumerator initialize()
    {
        yield return StartCoroutine(puzzleLighting.startLighting(this));
        StartCoroutine(reloadPlatforms());
    }
    public void randomizeObstaclesInRange(float start, float end, int half)
    {
        float x = start + UnityEngine.Random.Range(0, rangeOfObstacleGap); //starting point
        while (x < end)
        {
            //get a random number to choose between upper and lower obstacles, use this to affect the yOffset 

            Verticality verticality = (Verticality)UnityEngine.Random.Range(0, 2); //choose randomly between upper and lower
            List<GameObject> currentPrefabs = verticality == Verticality.UPPER ? upperPrefabObstacles : lowerPrefabObstacles;
            int prefabIndex = UnityEngine.Random.Range(0, currentPrefabs.Count); //choose a random prefab obstacle from the list
            float randomXGap = UnityEngine.Random.Range(0, rangeOfObstacleGap);
            float randomYShift = UnityEngine.Random.Range(0, rangeOfVerticality);

            Vector3 obstacleSize = currentPrefabs[prefabIndex].GetComponent<PolygonCollider2D>().bounds.size;
            float alignmentYOffset = (verticality == Verticality.UPPER ? -1 : 1) * (obstacleSize.y/2 + randomYShift); //trying to get the surfaces to be aligned with the center of the puzzle bounds
            float alignmentXOffset = obstacleSize.x / 2; //want to spawn gameobjects with lefthand surface touching the cursor's x, typically spawning objects centers them at that x coord
            if (x + obstacleSize.x > end) { break; } //if the x position we're currently at will put the object out of bounds, break

            Vector3 cursor = new Vector3(x + alignmentXOffset, collider.bounds.center.y - alignmentYOffset, 0); //thought of like a mouse cursor drag and dropping gameobjects

            GameObject obstacle = loadObstacleWorldSpace(currentPrefabs[prefabIndex], cursor);
            x += obstacleSize.x + randomXGap;
        }
    }
    public void randomizeObstaclesInRange(float start, float end)
    {
        float x = start + UnityEngine.Random.Range(0, rangeOfObstacleGap); //starting point
        while (x < end)
        {
            //get a random number to choose between upper and lower obstacles, use this to affect the yOffset 

            Verticality verticality = (Verticality)UnityEngine.Random.Range(0, 2); //choose randomly between upper and lower
            List<GameObject> currentPrefabs = verticality == Verticality.UPPER ? upperPrefabObstacles : lowerPrefabObstacles;
            int prefabIndex = UnityEngine.Random.Range(0, currentPrefabs.Count); //choose a random prefab obstacle from the list
            float randomXGap = UnityEngine.Random.Range(0, rangeOfObstacleGap);
            float randomYShift = UnityEngine.Random.Range(0, rangeOfVerticality);


            Vector3 obstacleSize = currentPrefabs[prefabIndex].GetComponent<PolygonCollider2D>().bounds.size;
            float alignmentYOffset = (verticality == Verticality.UPPER ? -1 : 1) * (obstacleSize.y/2 + randomYShift); //trying to get the surfaces to be aligned with the center of the puzzle bounds
            float alignmentXOffset = obstacleSize.x / 2; //want to spawn gameobjects with lefthand surface touching the cursor's x, typically spawning objects centers them at that x coord
            if (x + obstacleSize.x > end) { break; } //if the x position we're currently at will put the object out of bounds, break

            Vector3 cursor = new Vector3(x + alignmentXOffset, collider.bounds.center.y - alignmentYOffset, 0); //thought of like a mouse cursor drag and dropping gameobjects

            GameObject obstacle = loadObstacleWorldSpace(currentPrefabs[prefabIndex], cursor);
            x += obstacleSize.x + randomXGap;
        }
    }
}

//notes for tomorrow
//check puzzlelighting script. we need to add a light2d component to player upon entering the puzzle area and make sure to remove it when leaving
//maybe a fade for the intensity so it looks more smooth