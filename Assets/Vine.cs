using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Vine : MonoBehaviour
{
    public Rigidbody2D hook;
    public GameObject[] vineSegments;
    [SerializeField] private int numSegments;
    private float lengthOfVine;
    void Start()
    {
        generateVine();
    }
    void generateVine()
    {
        Rigidbody2D prevBod = hook;
        for (int i = 0; i < numSegments; i++)
        {
            GameObject newSegment = Instantiate(vineSegments[0]);
            newSegment.transform.parent = transform;
            newSegment.transform.position = transform.position;
            newSegment.GetComponent<HingeJoint2D>().connectedBody = prevBod;
            lengthOfVine += newSegment.GetComponent<BoxCollider2D>().bounds.size.y;
            prevBod = newSegment.GetComponent<Rigidbody2D>();
        }
    }
}
