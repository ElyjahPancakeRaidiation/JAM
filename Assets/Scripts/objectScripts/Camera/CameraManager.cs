using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.EditorTools;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CameraOperator camOperator;

    [Header("Collider size")]
    [SerializeField] private LayerMask colliderMask;
    [SerializeField] private Vector2 colliderSize, colliderOffsetSize;
    private float colliderAngular;
    private Collider2D cameraCol;

    [Header("Camera Options")]
    [SerializeField] private bool moveCamera;
    [SerializeField] private bool changeSize;
    [SerializeField] private bool keepNewSettings;


    [SerializeField] private float newCameraSize;

    [SerializeField, Tooltip("If this is set to 0 it will revert to the camera defualt size")]
    private float origCameraSize;

    [SerializeField, Tooltip("How fast the camera changes it's size: Higher = slower")]
    private float newSizeSpeed;

    [SerializeField, Tooltip("How fast the camera changes it's size back to the original: Higher = slower")]
    private float origSizeSpeed;

    [SerializeField] private GameObject newPosition;

    [SerializeField] private Vector2 newPositionOffset;

    [SerializeField, Tooltip("How fast it will travel to the new position: Higher = slower")]
    private float speedToPosition;

    [SerializeField, Tooltip("How fast it will speed up to get back to it's original speed: Lower = slower")]
    private float speedUpToOrigPosition;


    private GameObject origPosition;
    private bool triggered;
    private Coroutine changingSizeEnumerator;



    // Start is called before the first frame update
    void Start()
    {
        camOperator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOperator>();
        if (origCameraSize == 0) { origCameraSize = camOperator.getCameraDefualtSize(); }
    }

    private void FixedUpdate()
    {
        cameraCol = Physics2D.OverlapBox(transform.position + (Vector3)colliderOffsetSize, colliderSize, colliderAngular, colliderMask);
        if (cameraCol != null)
        {
            activate();
            triggered = true;
        }
        else
        {
            if (triggered)
            {
                disable();
            }
        }
    }

    private void activate()
    {

        if (moveCamera)
        {
            origPosition = camOperator.getTarget();
            camOperator.setFollowPlayer(false);
            camOperator.setSpeed(speedToPosition);
            camOperator.setCurSpeed(speedToPosition);
            camOperator.setDefualtOffset(newPositionOffset);
            camOperator.setTarget(newPosition);
        }

        if (changeSize)
        {
            if (changingSizeEnumerator != null) { StopCoroutine(changingSizeEnumerator); }
            changingSizeEnumerator = StartCoroutine(camOperator.changeCameraSize(newCameraSize, newSizeSpeed));
        }

        CameraOperator.settingsChanged = true;
    }

    private void disable()
    {
        if (triggered)
        {
            if (moveCamera)
            {
                camOperator.setSpeed(camOperator.getDefualtSpeed());
                // camOperator.setCurSpeed(speedUpToOrigPosition);
                // camOperator.setCurSpeed(camOperator.getCameraDefualtSize());
                camOperator.setCamSpeedUpAmount(speedUpToOrigPosition);
                camOperator.setDefualtOffset(camOperator.getDefualtOffset());
                camOperator.setTarget(origPosition);
                camOperator.setFollowPlayer(true);
            }

            if (changeSize)
            {
                if (changingSizeEnumerator != null) { StopCoroutine(changingSizeEnumerator); }
                if(!keepNewSettings) {changingSizeEnumerator = StartCoroutine(camOperator.changeCameraSize(origCameraSize, origSizeSpeed));}
            }

            triggered = false;
        }
    }

    void OnDrawGizmosSelected() => Gizmos.DrawWireCube(transform.position + (Vector3)colliderOffsetSize, colliderSize);




}
