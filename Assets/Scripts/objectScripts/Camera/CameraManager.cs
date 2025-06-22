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
    private bool playAutomatically;
    [SerializeField] private float camWaitTime;
    
    // Start is called before the first frame update
    void Start()
    {
        camOperator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOperator>();
    }
    private void Update()
    {
        if (playAutomatically)
        {
            Debug.Log("FUCKAKSFKSD");
            StartCoroutine(PlayManagerAutomatically());
            playAutomatically = false;
        }
    }

    private void FixedUpdate()
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

    private void activate()
    {
        if (moveToTarget)
        {
            camOperator.MOVETOTARGET(camTarget);
            camOperator.SETSPEED(newSpeed);
            camOperator.SETINCREASESPEEDPERC(returningPercentageSpeed);
        }

        if (changeCameraSize)
        {
            camOperator.SETZOOMSPEED(cameraSizeSpeed);
            camOperator.SETCAMERASIZE(newCameraSize);
        }

        if (changeCameraOffset)
        {
            camOperator.SETCHANGINGOFFSETSPEED(cameraOffsetSpeed);
            camOperator.SETCAMERAOFFSET(newCameraOffset);
        }
    }

    private void deactivate()
    {
        if (moveToTarget && !keepSpeedSettings)
        {
            camOperator.MOVETOTARGET(GameObject.FindGameObjectWithTag("Player"));//Null will set it to player
        }

        if (changeCameraSize && !keepSizeSettings)
        {
            camOperator.SETZOOMSPEED(returningSizeSpeed);
            camOperator.SETCAMERASIZE(returningSize);
        }

        if (changeCameraOffset && !keepOffsetSettings)
        {
            camOperator.SETCHANGINGOFFSETSPEED(returningCameraOffsetSpeed);
            camOperator.SETCAMERAOFFSET(returningCameraOffset);
        }
    }

    private IEnumerator PlayManagerAutomatically()
    {
        Debug.Log("workijng??");
        activate();
        yield return new WaitForSecondsRealtime(camWaitTime);
        deactivate();
    }

    void OnDrawGizmosSelected() => Gizmos.DrawWireCube(transform.position + (Vector3)colliderOffsetSize, colliderSize);

    public void PlayAutomatically() => playAutomatically = true;


}
