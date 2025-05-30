using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DustMovement : MonoBehaviour
{
    public GameObject player;
    public float yOffset;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        yOffset = player.GetComponent<CircleCollider2D>().radius;
    }
    void Update()
    {
        transform.position = new Vector2(player.transform.position.x, player.transform.position.y - yOffset);
    }
}
