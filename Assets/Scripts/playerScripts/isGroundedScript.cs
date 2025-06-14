using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class isGroundedScript : MonoBehaviour
{

    [SerializeField] private GameObject objectToFollow;
    [SerializeField] private Vector2 followOffset;
    private Vector2 startPosition, colSize;
    [SerializeField] LayerMask groundLayer;
    private float angle;
    private void Start()
    {
        // staticRotation = transform.rotation;
    }

    void Update()
    {

        transform.position = objectToFollow.transform.position + (Vector3)followOffset;
    }


    public bool isGrounded()
    {

        return Physics2D.OverlapBox(startPosition, colSize, angle, groundLayer);
    }

    public void setColSize(Vector2 val)
    {
        colSize = val;
    }

    public void setStartPosition(Vector2 val)
    {
        startPosition = val;
    }



    void OnDrawGizmosSelected() => Gizmos.DrawWireCube(startPosition, colSize);

}