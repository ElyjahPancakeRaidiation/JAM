using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IgnoreMovingColliders : MonoBehaviour
{

    private void Awake()
    {
        GameObject[] allMovingObjects = GameObject.FindGameObjectsWithTag("MovingGround");
        foreach (GameObject obj in allMovingObjects)
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), obj.GetComponent<Collider2D>());
        }
    }
}
