using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudScript : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private GameObject player;
    private PlayerMovement playerMovement;
    void Start()
    {  
        _collider = GetComponent<PolygonCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
    }
    void Update()
    {
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("BRO IN THE MUD LMFAOOOO");
    }
}
