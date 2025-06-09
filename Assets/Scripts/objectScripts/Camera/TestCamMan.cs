using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class TestCamMan : MonoBehaviour
{
    private TestCamOp camOP;

    private Collider2D col;
    [SerializeField] private Vector2 colSize;
    private float colAngle;
    [SerializeField] private LayerMask colMask;
    private bool triggered = false;


    [SerializeField] private GameObject newPosition;
    [SerializeField] private float wantedSpeed, oriSpeed;

    private void Start() => camOP = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TestCamOp>();

    private void FixedUpdate()
    {
        col = Physics2D.OverlapBox(transform.position, colSize, colAngle, colMask);

        if (col != null)
        {
            if (!triggered)
            {
                camOP.setTarget(newPosition);
                camOP.setSpeed(wantedSpeed);
                triggered = true;
            }
        }
        else
        {
            if (triggered)
            {
                camOP.setTarget(GameObject.FindGameObjectWithTag("Player"));
                camOP.setSpeed(oriSpeed);
                triggered = false;
            }
        }
    }

    void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, colSize);
}
