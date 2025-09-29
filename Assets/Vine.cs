using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Vine : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject[] vineSegments;
    [SerializeField] public int numSegments;
    private float lengthOfVine;
    [SerializeField] private bool overwritePlayerPull;
    [SerializeField] private float newForce;
    void Start()
    {
        generateVine();
    }
    private void OnEnable()
    {
        if (vineSegments.Length < 1)
        {
            generateVine();
        }
    }
    void generateVine()
    {
        Rigidbody2D prevBod = hook;
        for (int i = 0; i < numSegments; i++)
        {
            GameObject newSegment = Instantiate(vineSegments[Random.Range(0, vineSegments.Length)]);
            newSegment.transform.parent = transform;
            newSegment.transform.position = transform.position;
            newSegment.GetComponent<HingeJoint2D>().connectedBody = prevBod;
            lengthOfVine += newSegment.GetComponent<BoxCollider2D>().bounds.size.y;
            prevBod = newSegment.GetComponent<Rigidbody2D>();
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(hook.transform.position, hook.transform.position - new Vector3(0, numSegments * vineSegments[0].transform.localScale.y, 0));
    }
    public bool PullForceOverwritten()
    {
        return overwritePlayerPull;
    }
    public float VineSpecificForce()
    {
        return newForce;
    }
}
