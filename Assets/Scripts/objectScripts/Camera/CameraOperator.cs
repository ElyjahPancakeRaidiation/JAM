using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraOperator : MonoBehaviour
{

    private Camera _cam;
    private GameObject player;

    [SerializeField] private float defualtSpeed;
    [SerializeField] private float curSpeed;

    //Used for how fast the camera should zoom in or out.
    [SerializeField, Tooltip("How fast the camera will zome in and out this is mainly changed in the manager")]
    private float zoomSpeed = 8;

    [Header("       Camera speed and position settigns      ")]
    //Controls how fast the camera is when trying to catch up to the player
    private float speedUpAmount = 2;
    //Controls how fast the camera is when going back to its normal speed
    private float slowDownAmount = 0.5f;

    //This is for the camera manager to control how fast the camera will go back to the player after focusing on a seperate object.
    private float increaseSpeedPercentage;

    //This is the max speed point for the player, for the camera instead
    [SerializeField] private float objectsMaxSpeedPoint;

    //Offset from the currently focused object's position
    [SerializeField] private Vector2 offset;

    //How fast the offset changes when the camera manager activates
    private float changingOffsetSpeed;
    private Vector2 defualtOffset;


    [Header("       Camera border and size settings     ")]
    [SerializeField, Tooltip("If it is 0 then it will revert to the defualt size being 8.")]
    private float camStartSize;
    private const float CAMDEFAULTSIZE = 8f;

    [SerializeField]
    private float leftBorder, rightBorder, upBorder, downBorder;

    /// <summary>
    /// These two bools use auto-property which is basically this line of code but in a smaller version
    /// private bool canMove;
    /// public bool canMove{
    ///     get{return canMove;}
    ///     set{canMove = value;}
    /// }
    /// After doing research people typically use properties over fields to make changes to the implimination instead of changing the visible surface of the class
    /// Yes that line was directly copied from reddit. 
    /// </summary>
    public bool canMove { get; set; } = true;
    public bool followPlayer { get; set; } = true;

    [SerializeField] private bool farFromPlayer;
    private GameObject target;

    private Vector2 refVec = Vector2.zero;
    private float refFloat = 0;

    //static position bools is used to shut off a cameras axis that is following the player
    public bool staticXPosition { get; set; }
    public bool staticYPosition { get; set; }

    private Coroutine changingSizeEnumerator;
    private Coroutine changingOffsetEnumerator;


    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
        target = player;
        if (camStartSize == 0) { camStartSize = CAMDEFAULTSIZE; }
        _cam.orthographicSize = camStartSize;
        curSpeed = defualtSpeed;
        defualtOffset = offset;
    }

    // Update is called once per frame
    void Update()
    {
        // float screenAspect = (float)Screen.width/Screen.height;
        // float camHeight = _cam.orthographicSize * 2f;
        // Bounds bounds = new(
        //     _cam.transform.position,
        //     new Vector2(camHeight * screenAspect, camHeight)
        // );
        // Debug.Log("Min: " + bounds.min); 

        if (followPlayer)
        {
            CameraCatchUp();
        }
        else
        {
            if (!farFromPlayer)
            {
                if (Vector2.Distance(transform.position, player.transform.position) > 10)
                {
                    farFromPlayer = true;
                }
            }
        }

        if (target != null)
        {
            if (canMove) { moveCamera(target); }
        }
    }


    private void moveCamera(GameObject target)
    {

        //Have it offset a little by the y axis when it gets to max speed.
        float xSmoothDamp = (!staticXPosition) ? Mathf.SmoothDamp(transform.position.x, target.transform.position.x + offset.x, ref refVec.x, curSpeed * Time.deltaTime) : transform.position.x;
        float ySmoothDamp = (!staticYPosition) ? Mathf.SmoothDamp(transform.position.y, target.transform.position.y + offset.y, ref refVec.y, (curSpeed * Time.deltaTime) + 0.1f) : transform.position.y;

        //If the borders are 0 then the camera can go anywere. Otherwise clamp the camera between the specficied borders
        if (leftBorder != 0 && rightBorder != 0 && upBorder != 0 && downBorder != 0)
        {
            xSmoothDamp = Mathf.Clamp(xSmoothDamp, leftBorder, rightBorder);
            ySmoothDamp = Mathf.Clamp(ySmoothDamp, downBorder, upBorder);
        }
        transform.position = new Vector3(xSmoothDamp, ySmoothDamp, -10f);
    }
    private void CameraCatchUp()
    {
        //Once the player goes past the max speed point the camera will speed up by decreasing our curspeed.

        if (!farFromPlayer && followPlayer)
        {
            if (GetPastMaxSpeedPoint())
            {
                if (curSpeed > 0.2f)
                {
                    curSpeed -= Time.deltaTime * speedUpAmount;
                }
            }
            else
            {
                resetCurSpeed();
            }
        }
        else if (farFromPlayer && followPlayer)
        {
            HeadingTowardsPlayer();
        }

    }
    private void HeadingTowardsPlayer()
    {
        if (curSpeed > defualtSpeed && Vector2.Distance(transform.position, player.transform.position) > 1)
        {
            curSpeed -= Mathf.Abs(player.GetComponent<Rigidbody2D>().velocity.x) * increaseSpeedPercentage * Time.deltaTime;
        }
        else
        {
            farFromPlayer = false;
        }
    }

    public void shakeCamera(float duration, float strength)
    {
        StartCoroutine(CameraShake(duration, strength));
    }
    public void moveToTarget(GameObject target)
    {
        if (target != player && target != null)
        {
            followPlayer = false;
        }
        else if (target == null)
        {
            followPlayer = true;
        }
        else
        {
            followPlayer = true;
        }
        this.target = target;
    }
    public void setSpeed(float speed) => curSpeed = speed;
    public void setIncreaseSpeedPerc(float percentage) => increaseSpeedPercentage = percentage;
    public void setCameraSize(float cameraSize)
    {
        if (changingSizeEnumerator != null) { StopCoroutine(changingSizeEnumerator); }
        changingSizeEnumerator = StartCoroutine(ChangeCameraSize(cameraSize, zoomSpeed));
    }

    public void setCameraOffset(Vector2 newOffset)
    {
        if (changingOffsetEnumerator != null) { StopCoroutine(changingOffsetEnumerator); }
        changingOffsetEnumerator = StartCoroutine(ChangeCameraOffset(newOffset, changingOffsetSpeed));
    }
    public void setChangingOffsetSpeed(float val) => changingOffsetSpeed = val;
    public void setZoomSpeed(float changingSizeSpeed) => this.zoomSpeed = changingSizeSpeed;
    //Slowly brings the current speed value back to the speed value.
    private void resetCurSpeed() => curSpeed = Mathf.Lerp(curSpeed, defualtSpeed, slowDownAmount * Time.deltaTime);
    private bool GetPastMaxSpeedPoint()
    {
        Rigidbody2D _rb = target.GetComponent<Rigidbody2D>();
        if (target == null)
        {
            return false;
        }
        var checkXVelocity = _rb.velocity.x >= objectsMaxSpeedPoint || _rb.velocity.x <= -objectsMaxSpeedPoint;
        var checkYVelocity = _rb.velocity.y >= objectsMaxSpeedPoint || _rb.velocity.y <= -objectsMaxSpeedPoint;
        return (checkXVelocity || checkYVelocity);

    }
    //IEnumerators
    private IEnumerator ChangeCameraSize(float wantedFOV, float fovSpeed)
    {

        while (_cam.orthographicSize != wantedFOV)
        {
            _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, wantedFOV, ref refFloat, Time.deltaTime * fovSpeed);
            yield return null;//waits a frame before moving on
        }

    }

    private IEnumerator ChangeCameraOffset(Vector2 newOffset, float changingOffsetSpeed)
    {
        if (newOffset != defualtOffset) { curSpeed = defualtSpeed; }
        while (offset != newOffset)
        {
            offset = Vector2.MoveTowards(offset, newOffset, changingOffsetSpeed);
            yield return null;
        }
    }

    private IEnumerator CameraShake(float duration, float strength)
    {
        float time = 0;
        //this takes place of the strength variable so it can change its value while not affecting the strength variable in the inspector
        float curStrength = strength;
        //ref variables are for the smoothdamps
        float refStrengthVel = 0;

        while (time < duration || curStrength > 0.1f)
        {
            //lowers the strength to zero so its a smooth transition
            if (time > duration - 2f)
            {
                curStrength = Mathf.SmoothDamp(curStrength, 0, ref refStrengthVel, 1.5f);
            }
            else
            {
                curStrength = strength;
            }
            float randX = UnityEngine.Random.value - 0.5f;
            float randY = UnityEngine.Random.value - 0.5f;
            float randZ = UnityEngine.Random.value - 0.5f;
            transform.localEulerAngles = new Vector3(randX, randY, randZ) * curStrength;
            //Bro who ever is reading this and hasn't seen the camera shake go look at that shit now, funny as hell (we dont curse.).
            time += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = Vector3.zero;
    }


}
