using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RopeControllerScript : MonoBehaviour
{
    //where the rope is coming out of
    public GameObject ropeShooting;
    //the physics representation of rope
    private SpringJoint2D rope;

    //life cycle of the rope
    public int maxRopeframecount;

    //current life cycle
    private int ropeframeCount;

    //drawing the line, the path the rope takes
    public LineRenderer linerender;

    public float Oscilliation =.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        if (Input.GetMouseButtonDown(1))
        {
            DestroyImmediate(rope);
        }
    }

    void LateUpdate()
    {
        if (rope != null)
        {
            linerender.enabled = true;
            linerender.SetVertexCount(2);
            linerender.SetPosition(0, ropeShooting.transform.position);
            linerender.SetPosition(1, rope.connectedAnchor);

        }
        else
        {
            linerender.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (rope != null)
        {
            if (ropeframeCount > maxRopeframecount)
            {
                DestroyImmediate(rope);
                ropeframeCount = 0;

            }
        }
    }
    //using a raycast to detect if anything has been hit
    void Fire()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 Originposition = ropeShooting.transform.position;
        Vector3 direction = mousePosition - Originposition;

        RaycastHit2D hit = Physics2D.Raycast(Originposition, direction, Mathf.Infinity);

        if (hit.collider != null)
        {
            SpringJoint2D newRope = ropeShooting.AddComponent<SpringJoint2D>();
            newRope.enableCollision = false;
            newRope.frequency = Oscilliation;
            //where the raycast hits
            newRope.connectedAnchor = hit.point;
            //enable it
            newRope.enabled = true;
            DestroyImmediate(rope);
            rope = newRope;
            ropeframeCount = 0;
        }
    }
}
