using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    private new PolygonCollider2D collider;
    private GameObject player;
    private int numContacts;
    public bool active = false;
    private float startBound;
    private float endBound;
    void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
        startBound = collider.bounds.min.x;
        endBound = collider.bounds.max.x;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        checkForPlayerBounds();
    }
    public bool IsActive()
    {
        return active;
    }
    public void SetPlatformActive(bool active)
    {
        this.active = active;
    }
    public void UpdateActive()
    {
        Collider2D collider = Physics2D.OverlapBox(this.collider.bounds.center, this.collider.bounds.size, 0f, LayerMask.GetMask("Player"));
        if (collider != null && collider.CompareTag("Player")) //all this just to check if this platform is in contact with player
        {
            Debug.Log("found active platform");
            active = true;
        }
        else
        {
            active = false;
        }
    }
    public void checkForPlayerBounds()
    {
        float playerX = player.transform.position.x;
        if (playerX > startBound && playerX < endBound){ active = true; }else{ active = false; }
    }
}
