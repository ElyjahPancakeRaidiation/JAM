using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.EditorTools;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CameraManager : MonoBehaviour
{

    private CameraOperator camOperator;
    [SerializeField] private bool moveCamera;
    [SerializeField] private bool changeSize;
    [SerializeField] private bool keepNewSettings, keepNewPosition;


    [SerializeField] private float newCameraSize;

    [SerializeField, Tooltip("If this is set to 0 it will revert to the camera defualt size")]
    private float origCameraSize;

    [SerializeField, Tooltip("How fast the camera changes it's size")]
    private float newSizeSpeed;

    [SerializeField, Tooltip("How fast the camera changes it's size back to the original")]
    private float origSizeSpeed;

    [SerializeField] private GameObject newPosition;
    [SerializeField] private float speedToPosition;
    [SerializeField] private float speedToOriginalPosition;

    private GameObject origPosition;
    [SerializeField]private bool triggered;
    private Coroutine changingSizeEnumerator;



    // Start is called before the first frame update
    void Start()
    {
        camOperator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOperator>();
        if(origCameraSize == 0){ origCameraSize = camOperator.getCameraDefualtSize(); }
    }

    private void activate()
    {

        if (moveCamera)
        {
            camOperator.setSpeed(speedToPosition);
            if(!triggered){origPosition = camOperator.getTarget();}
            camOperator.setTarget(newPosition);
        }

        if (changeSize)
        {
            if(changingSizeEnumerator != null){StopCoroutine(changingSizeEnumerator);}
            changingSizeEnumerator = StartCoroutine(camOperator.changeCameraSize(newCameraSize, newSizeSpeed));
        }
    }

    private void disable()
    {
        if (triggered)
        {
            if (moveCamera)
            {
                camOperator.setSpeed(speedToOriginalPosition);
                camOperator.setTarget(origPosition);
            }

            if (changeSize)
            {
                if(changingSizeEnumerator != null){StopCoroutine(changingSizeEnumerator);}
                changingSizeEnumerator = StartCoroutine(camOperator.changeCameraSize(origCameraSize, origSizeSpeed));
            }
            
            triggered = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            activate();
            triggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (triggered)
        {
            disable();
        }
    }


}
