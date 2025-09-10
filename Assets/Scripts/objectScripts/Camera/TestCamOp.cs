using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamOp : MonoBehaviour
{

    [SerializeField] private Vector2 testSize;
    [SerializeField] private Vector2 testOffsetSize;
    private float angle;
    [SerializeField] private BoxCollider2D col;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)testOffsetSize, testSize);
        col.offset = testOffsetSize;
        col.size = testSize;

    }

}
