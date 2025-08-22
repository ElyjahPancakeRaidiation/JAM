using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CameraOperator camOperator;

    [Header("Collider size       ")]
    [SerializeField] private LayerMask colliderMask;
    [SerializeField] private Vector2 colliderSize, colliderOffsetSize;
    private float colliderAngular;
    private Collider2D cameraCol;
    private bool triggered;

    [Header("Camera Actions")]
    public bool moveToTarget;
    public bool changeCameraSize;
    public bool changeCameraOffset;


    [Header("Camera Speed & Target Settings")]

    [Tooltip("If nothing is set than it will automatically go to player")]
    public GameObject camTarget;
    public float newSpeed;

    [Tooltip("How fast the camera goes when returning to the player. Value: 0-1")]
    public float returningPercentageSpeed;
    public bool keepSpeedSettings;

    [Header("Camera Size Settings")]
    public float newCameraSize;
    public float cameraSizeSpeed;
    public float returningSize;
    public float returningSizeSpeed;
    public bool keepSizeSettings;

    [Header("Camera Offset Settings")]
    public Vector2 newCameraOffset;
    public float cameraOffsetSpeed;
    public Vector2 returningCameraOffset;
    public float returningCameraOffsetSpeed;
    public bool keepOffsetSettings;

    [Header("Cutscene settings")]
    [SerializeField] private float camWaitTime;
    private bool playAutomatically;

    [Header("Effects")]
    public bool shakeCamera;
    public float shakeDuration;
    public float shakeStrength;
    //if true it will repeat the effect when ever the player enters the area
    public bool repeat = false;
    private bool effectsActivated = false;//Track whether it was activated already or not

    // Start is called before the first frame update
    void Start()
    {
        camOperator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOperator>();
    }
    private void Update()
    {
        if (playAutomatically)
        {
            StartCoroutine(PlayManagerAutomatically());
            playAutomatically = false;
        }
    }

    private void FixedUpdate()
    {
        if (colliderSize != Vector2.zero)
        {
            cameraCol = Physics2D.OverlapBox(transform.position + (Vector3)colliderOffsetSize, colliderSize, colliderAngular, colliderMask);
            if (cameraCol != null)
            {
                if (!triggered) { activate(); }
                triggered = true;
            }
            else
            {
                if (triggered)
                {
                    deactivate();
                    triggered = false;
                }
            }
        }
    }

    private void activate()
    {
        if (moveToTarget)
        {
            camOperator.moveToTarget(camTarget);
            camOperator.setSpeed(newSpeed);
            camOperator.setIncreaseSpeedPerc(returningPercentageSpeed);
        }

        if (changeCameraSize)
        {
            camOperator.setZoomSpeed(cameraSizeSpeed);
            camOperator.setCameraSize(newCameraSize);
        }

        if (changeCameraOffset)
        {
            camOperator.setChangingOffsetSpeed(cameraOffsetSpeed);
            camOperator.setCameraOffset(newCameraOffset);
        }

        if (shakeCamera)
        {
            if (!effectsActivated)
            {
                camOperator.shakeCamera(shakeDuration, shakeStrength);
                effectsActivated = true;
            }
            else if(effectsActivated && repeat)
            {
                camOperator.shakeCamera(shakeDuration, shakeStrength);
            }
        }
    }

    private void deactivate()
    {
        if (moveToTarget && !keepSpeedSettings)
        {
            camOperator.moveToTarget(GameObject.FindGameObjectWithTag("Player"));//Null will set it to player
        }

        if (changeCameraSize && !keepSizeSettings)
        {
            camOperator.setZoomSpeed(returningSizeSpeed);
            camOperator.setCameraSize(returningSize);
        }

        if (changeCameraOffset && !keepOffsetSettings)
        {
            camOperator.setChangingOffsetSpeed(returningCameraOffsetSpeed);
            camOperator.setCameraOffset(returningCameraOffset);
        }
    }

    private IEnumerator PlayManagerAutomatically()
    {
        activate();
        yield return new WaitForSecondsRealtime(camWaitTime);
        deactivate();
    }

    void OnDrawGizmosSelected() => Gizmos.DrawWireCube(transform.position + (Vector3)colliderOffsetSize, colliderSize);

    public void PlayAutomatically() => playAutomatically = true;


}
