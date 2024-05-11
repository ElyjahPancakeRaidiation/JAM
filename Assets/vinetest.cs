using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vinetest : MonoBehaviour
{

    public Transform transformTest;
    private Rigidbody2D rb;
    public bool onVine;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (onVine)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 2;
        }
    }
}
