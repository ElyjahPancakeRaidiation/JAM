using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleInteracable : MonoBehaviour
{
    [SerializeField] private ThoughtBubbleInteractive thoughtBubbleInteractive;
    private Collider2D col;
    private float colAngle;
    [SerializeField] private Vector2 colVector, colOffset;
    bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        if (thoughtBubbleInteractive == null)
        {
            Debug.LogError("ThoughtBubbleInteractive is not assigned on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!thoughtBubbleInteractive.isCompleted)
        {
            if (!active && col && !thoughtBubbleInteractive.isPlayingMG)//insane line
            {
                // thoughtBubbleInteractive.isPlayingMG = true;
                active = true;
                StartCoroutine(thoughtBubbleInteractive.StartMinigame());
            }
        }
    }

    void FixedUpdate()
    {
        if (!thoughtBubbleInteractive.isCompleted)
        {
            col = Physics2D.OverlapBox(transform.position + (Vector3)colOffset, colVector, colAngle, LayerMask.GetMask("Player"));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)colOffset, colVector);
    }
}
