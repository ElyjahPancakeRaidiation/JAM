using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    [SerializeField]private Vector2 vecSpeed;
    [SerializeField]private float totalSpeed;//Multiples to both the y and x
    [SerializeField]private bool canMove = true;
    [SerializeField] private Transform point;
    [SerializeField] private string tagName;

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if(point == null){point = GameObject.Find(tagName).GetComponent<Transform>();}
            transform.position = Vector2.MoveTowards(transform.position, point.position, totalSpeed * Time.deltaTime);
        }
    }

    public void setCanMove(bool val){canMove = val;}
}
