using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    private CameraOperator camOperator;

    [Header("       Collider size       ")]
    [SerializeField] private LayerMask colliderMask;
    [SerializeField] private Vector2 colliderSize, colliderOffsetSize;
    private float colliderAngular;
    private Collider2D cameraCol;
    private bool triggered;


    [Header("       Camera events       ")]
    [SerializeField] private UnityEvent activationEvents;
    [SerializeField] private UnityEvent deactivationEvents;



    // Start is called before the first frame update
    void Start()
    {
        camOperator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOperator>();
    }

    private void FixedUpdate()
    {
        cameraCol = Physics2D.OverlapBox(transform.position + (Vector3)colliderOffsetSize, colliderSize, colliderAngular, colliderMask);
        if (cameraCol != null)
        {
            if (!triggered) { activationEvents.Invoke(); }
            triggered = true;
        }
        else
        {
            if (triggered)
            {
                deactivationEvents.Invoke();
                triggered = false;
            }
        }
    }

    void OnDrawGizmosSelected() => Gizmos.DrawWireCube(transform.position + (Vector3)colliderOffsetSize, colliderSize);




}
