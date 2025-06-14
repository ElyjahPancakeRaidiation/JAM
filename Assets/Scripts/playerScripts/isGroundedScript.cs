using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{

    [SerializeField] private GameObject objectToFollow;
    private Vector2 startPosition, colSize;
    [SerializeField] LayerMask groundLayer;
    private float angle;

    void Update()
    {

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