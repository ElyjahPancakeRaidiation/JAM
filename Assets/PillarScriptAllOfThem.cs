using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PillarScriptAllOfThem : MonoBehaviour
{
    public Vector3[] pillarsArray;
    
    public GameObject pillars;
    public bool time;
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        pillarSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(pillarSpawn());
    }

    public IEnumerator pillarSpawn()
    {
        foreach (Transform Pillar in transform)
        {

            Debug.Log("Pillar Name: " + Pillar.gameObject);
            yield return new WaitForSecondsRealtime(4);
           
            if (Pillar.gameObject != null)
            {
                Destroy(Pillar.gameObject);
            }


        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}


//try to get them to spawn while pressing a key first