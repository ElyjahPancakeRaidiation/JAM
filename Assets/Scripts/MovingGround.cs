using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    [SerializeField]private Vector2 vecSpeed;
    [SerializeField]private float totalSpeed;//Multiples to both the y and x
    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove){
            float speedX = vecSpeed.x * totalSpeed;
            float speedY = vecSpeed.y * totalSpeed;
            transform.position = new Vector3(transform.position.x + speedX * Time.deltaTime, transform.position.y + speedY * Time.deltaTime);
        }
    }
}
