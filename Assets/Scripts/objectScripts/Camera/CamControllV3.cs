using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControllV3 : MonoBehaviour
{
    private float dampTime;

    private Vector3 velocity = Vector3.zero;

    public Transform playerTarget;

    // Start is called before the first frame update
    void Start()
    {
        dampTime = .3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTarget==true)
        {
            newFollowPlayer();
        }

    }

    private void newFollowPlayer()
    {
        Vector3 point = GetComponent<Camera>().WorldToViewportPoint(playerTarget.position);
        //Find the difference between the playerposition to the portpoints of .5
        
        Vector3 delta = playerTarget.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        //The total distance you want the camera to stay from the player
        Vector3 destination = transform.position + delta;
   
        // camera's position is set to destination with a dampTime of 0.3f;
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        
    }
}
