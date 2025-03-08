using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1FR : MonoBehaviour
{
    public GameObject obj1, obj2, obj3;

    public float midPoint, dist;
    // Start is called before the first frame update
    void Start()
    {
        midPoint = obj1.transform.position.x + obj2.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(midPoint/2, transform.position.y, transform.position.z);

        dist = obj3.transform.position.x - midPoint/2; 
        
    }
}
