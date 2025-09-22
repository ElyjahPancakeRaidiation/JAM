using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineSegment : MonoBehaviour
{
    public GameObject above, below;
    void Start()
    {
        gameObject.tag = "Vine";
        above = GetComponent<HingeJoint2D>().connectedBody.gameObject;
        VineSegment aboveSegment = above.GetComponent<VineSegment>();
        if (aboveSegment)
        {
            aboveSegment.below = gameObject;
            float bottomOfSprite = above.GetComponent<SpriteRenderer>().bounds.size.y;
            GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -1 * bottomOfSprite);
        }
        else
        {
            GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
