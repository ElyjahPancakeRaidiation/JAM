using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CameraOperator : MonoBehaviour
{

    private Camera _cam;

    [SerializeField, Range(0, 100), Header("Camera speed and position settigns")]
    private float speed;
    private float curSpeed;

    [SerializeField]
    private float speedUpAmount = 2, slowDownAmount = 0.5f;

    //This is the max speed point for the player, for the camera instead
    [SerializeField]private float playerMaxSpeedPoint;

     //This is specifically for the y axis so it still has a little delay when the player goes over the y axis max point
    [SerializeField] private float addedYOffsetMaxSpeed;
    [SerializeField]private Vector2 offset;
    

    [Header("Camera border and size settings")]
    [SerializeField, Tooltip("If it is 0 then it will revert to the defualt size being 8.")]
    private float camStartSize;

    private const float CAMDEFAULTSIZE = 8f;

    [SerializeField]
    private float leftBorder, rightBorder, upBorder, downBorder;

    private bool canMove = true, followPlayer = true;
    private GameObject target;
    private Vector2 refVec = Vector2.zero;
    private float refFloat = 0;
    [SerializeField]private bool changingSize;


    // Start is called before the first frame update
    void Start()
    {
        curSpeed = speed;
        _cam = GetComponent<Camera>();
        if (camStartSize == 0) { camStartSize = CAMDEFAULTSIZE; }
        _cam.orthographicSize = camStartSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayer)
        {
            target = GameObject.FindGameObjectWithTag("Player");

            if (getPastMaxSpeedPoint(target.GetComponent<PlayerMovement>()))
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
    }
    private void FixedUpdate()
    {
        if (canMove) { moveCamera(target); }
    }


    public void moveCamera(GameObject target)
    {
        //Have it offset a little by the y axis when it gets to max speed.
        float xSmoothDamp = Mathf.SmoothDamp(transform.position.x, target.transform.position.x + offset.x, ref refVec.x, curSpeed * Time.deltaTime);
        float ySmoothDamp = Mathf.SmoothDamp(transform.position.y, target.transform.position.y + offset.y, ref refVec.y, (curSpeed * Time.deltaTime)+addedYOffsetMaxSpeed);
        transform.position = new Vector2(xSmoothDamp, ySmoothDamp);
        
        //If the borders are 0 then the camera can go anywere. Otherwise clamp the camera between the specficied borders
        if (leftBorder != 0 && rightBorder != 0 && upBorder != 0 && downBorder != 0)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftBorder, rightBorder), Mathf.Clamp(transform.position.y, downBorder, upBorder), -10);
        }
        else { transform.position = new Vector3(transform.position.x, transform.position.y, -10); }

    }

    public IEnumerator changeCameraSize(float wantedFOV, float fovSpeed)
    {

        while (_cam.orthographicSize != wantedFOV)
        {
            _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, wantedFOV, ref refFloat, fovSpeed * Time.deltaTime);
            yield return null;
        }
        
    }

    public void setTarget(GameObject val) { target = val; }
    public GameObject getTarget(){ return target; }
    public void setCanMove(bool val) { canMove = val; }
    public void setFollowPlayer(bool val){ followPlayer = val; }
    public void setSpeed(float val)
    {
        speed = val;
        resetCurSpeed();
    }
    public float getCameraSize(){ return _cam.orthographicSize; }
    public float getCameraDefualtSize(){ return CAMDEFAULTSIZE; }
    public void setChangingSize(bool val){ changingSize = val; }

    //Slowly brings the current speed value back to the defualt speed value.
    public void resetCurSpeed() { curSpeed = Mathf.Lerp(curSpeed, speed, slowDownAmount * Time.deltaTime); }

    //This is to check if the player has gone past the cameras max speed threshold for the player in either the x or y axis.
    public bool getPastMaxSpeedPoint(PlayerMovement player) { return ((player.getCurVelocity().x >= playerMaxSpeedPoint || player.getCurVelocity().x <= -playerMaxSpeedPoint) || (player.getCurVelocity().y >= playerMaxSpeedPoint || player.getCurVelocity().y <= -playerMaxSpeedPoint)); }
}
