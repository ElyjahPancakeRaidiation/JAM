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
    [SerializeField] private float speedUpAmount = 2;
    [SerializeField] private float slowDownAmount = 0.5f;
    private float increaseSpeedPercentage;
    
    //This is the max speed point for the player, for the camera instead
    [SerializeField] private float playerMaxSpeedPoint;
    [SerializeField] private Vector2 offset;
    private float changingOffsetSpeed;
    private Vector2 defualtOffset;


    [Header("       Camera border and size settings     ")]
    [SerializeField, Tooltip("If it is 0 then it will revert to the defualt size being 8.")]
    private float camStartSize;
    private const float CAMDEFAULTSIZE = 8f;

    [SerializeField]
    private float leftBorder, rightBorder, upBorder, downBorder;

    private bool canMove = true, followPlayer = true;
    [SerializeField] private bool farFromPlayer;
    private GameObject target;

    private Vector2 refVec = Vector2.zero;
    private float refFloat = 0;

    private Coroutine changingSizeEnumerator;
    private Coroutine changingOffsetXEnumerator;
    private Coroutine changingOffsetYEnumerator;


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
        
        if (followPlayer)
        {
            CameraCatchUp();
        } else {
            if (!farFromPlayer)
            {
                if (Vector2.Distance(transform.position, player.transform.position) > 10)
                {
                    farFromPlayer = true;
                }
            }
        }
    }
    void FixedUpdate()
    {
        if (target != null)
        {
            if (canMove) { moveCamera(target); }
        }
    }


    private void moveCamera(GameObject target)
    {

        //Have it offset a little by the y axis when it gets to max speed.
        float xSmoothDamp = Mathf.SmoothDamp(transform.position.x, target.transform.position.x + offset.x, ref refVec.x, curSpeed * Time.deltaTime);
        float ySmoothDamp = Mathf.SmoothDamp(transform.position.y, target.transform.position.y + offset.y, ref refVec.y, (curSpeed * Time.deltaTime) + 0.1f);

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
            if (getPastMaxSpeedPoint(player.GetComponent<PlayerMovement>()))
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
            Debug.Log("Being called");
            HeadingTowardsPlayer();
        }

    }
    public void HeadingTowardsPlayer()
    {
        if (curSpeed > defualtSpeed)
        {
            if (Vector2.Distance(transform.position, player.transform.position) > 1)
            {
                curSpeed -= Mathf.Sqrt(player.GetComponent<Rigidbody2D>().angularVelocity) * increaseSpeedPercentage * Time.deltaTime;
            }
        }
        else
        {
            farFromPlayer = false;
        }
    }

    public void MOVETOTARGET(GameObject target)
    {
        if (target != player)
        {
            followPlayer = false;
        }else{
            followPlayer = true;
        }
        this.target = target;
    }
    public void SETSPEED(float speed) => curSpeed = speed;
    public void SETDEFUALTSPEED(float defualtSpeed) => this.defualtSpeed = defualtSpeed;
    public void SETINCREASESPEEDPERC(float percentage) => increaseSpeedPercentage = percentage;
    public void SETCAMERASIZE(float cameraSize)
    {
        if (changingSizeEnumerator != null) { StopCoroutine(changingSizeEnumerator); }
        changingSizeEnumerator = StartCoroutine(changeCameraSize(cameraSize, zoomSpeed));
    }
    public void SETCAMERAOFFSETX(float x)
    {
        if (changingOffsetXEnumerator != null) { StopCoroutine(changingOffsetXEnumerator); }
        changingOffsetXEnumerator = StartCoroutine(changeCameraOffsetX(x, changingOffsetSpeed));
    }
    public void SETCAMERAOFFSETY(float y)
    {
        if (changingOffsetYEnumerator != null) { StopCoroutine(changingOffsetYEnumerator); }
        changingOffsetYEnumerator = StartCoroutine(changeCameraOffsetY(y, changingOffsetSpeed));
    }
    public void SETCHANGINGOFFSETSPEED(float val) => changingOffsetSpeed = val;
    public void SETZOOMSPEED(float changingSizeSpeed) => this.zoomSpeed = changingSizeSpeed;
    public void DEFUALTSETTINGS()
    {
        followPlayer = true;
        target = player;

        SETCAMERASIZE(camStartSize);

        SETCAMERAOFFSETX(defualtOffset.x);
        SETCAMERAOFFSETY(defualtOffset.y);

    }
    public void SETDEFUALTOFFSETX(float newOffset) => defualtOffset.x = newOffset;
    public void SETDEFUALTOFFSETY(float newOffset) => defualtOffset.y = newOffset;

    //Setters
    public void setFollowPlayer(bool val) => followPlayer = val;
    public void setCanMove(bool val) => canMove = val;
    public void setSpeed(float val)
    {
        defualtSpeed = val;
        resetCurSpeed();
    }

    //Slowly brings the current speed value back to the speed value.
    public void resetCurSpeed() => curSpeed = Mathf.Lerp(curSpeed, defualtSpeed, slowDownAmount * Time.deltaTime);

    //This is to check if the player has gone past the cameras max speed threshold for the player in either the x or y axis.
    public bool getPastMaxSpeedPoint(PlayerMovement player) { return ((player.getCurVelocity().x >= playerMaxSpeedPoint || player.getCurVelocity().x <= -playerMaxSpeedPoint) || (player.getCurVelocity().y >= playerMaxSpeedPoint || player.getCurVelocity().y <= -playerMaxSpeedPoint)); }

    private IEnumerator changeCameraSize(float wantedFOV, float fovSpeed)
    {

        while (_cam.orthographicSize != wantedFOV)
        {
            _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, wantedFOV, ref refFloat, Time.deltaTime * fovSpeed);
            yield return null;//waits a frame before moving on
        }

    }

    private IEnumerator changeCameraOffsetX(float newOffset, float changingOffsetSpeed)
    {
        //Makes sure when the players offset changes it doesn't move at an increase speed
        if(newOffset != defualtOffset.x){curSpeed = defualtSpeed;}
        while (offset.x != newOffset)
        {
            offset = Vector2.MoveTowards(offset, new Vector2(newOffset, offset.y), changingOffsetSpeed);
            yield return null;
        }
    }
    private IEnumerator changeCameraOffsetY(float newOffset, float changingOffsetSpeed)
    {
        //Makes sure when the players offset changes it doesn't move at an increase speed
        if(newOffset != defualtOffset.y){curSpeed = defualtSpeed;}
        while (offset.y != newOffset)
        {
            offset = Vector2.MoveTowards(offset, new Vector2(offset.x, newOffset), changingOffsetSpeed);
            yield return null;
        }
    }

}
