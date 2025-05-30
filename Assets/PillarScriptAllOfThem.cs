using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarScriptAllOfThem : MonoBehaviour
{
    public Vector2[] pillarsArray;
    public GameObject pillars;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pillarSpawn();
    }

    void pillarSpawn()
    {
        for (int i = 0; i < pillarsArray.Length; i++)
        {
            Instantiate(pillars, pillarsArray[i], Quaternion.identity);
        }
    }
}
