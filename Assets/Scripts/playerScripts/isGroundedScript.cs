using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    [SerializeField] private GameObject followObj;
    float angle;
    [SerializeField] private LayerMask mask;

    //Since ball and pogo have different heights this adds an offset to the y to make it a little more even
    private float centerYOffset;

    // Start is called before the first frame update
    void Update()
    {
        transform.position = followObj.transform.position + new Vector3(0, -1 * centerYOffset + .2f, 0);
    }

    //isgrounded for ball and pogo jumping
    public bool isGroundedCircle(Vector2 point, Vector2 pointOffset, float radius){
        return Physics2D.OverlapCircle(point + pointOffset, radius, mask);
    }
    public bool isGroundedCircle(Vector2 point, Vector2 pointOffset, float radius, LayerMask mask){
        return Physics2D.OverlapCircle(point + pointOffset, radius, mask);
    }
    public bool isGroundedBox(Vector2 point, Vector2 pointOffset, Vector2 size)
    {
        return Physics2D.OverlapBox(point + pointOffset, size, angle, mask);
    }
    public bool isGroundedBox(Vector2 point, Vector2 pointOffset, Vector2 size, LayerMask mask)
    {
        return Physics2D.OverlapBox(point + pointOffset, size, angle, mask);
    }
    public bool isGroundedRay(Vector2 point, Vector2 pointOffset, Vector2 endPosition, float distance)
    {
        return Physics2D.Raycast(point + pointOffset, endPosition, distance, mask);
    }
    public bool isGroundedRay(Vector2 point, Vector2 pointOffset, Vector2 endPosition, float distance, LayerMask mask)
    {
        return Physics2D.Raycast(point + pointOffset, endPosition, distance, mask);
    }
    public bool isGroundedRay(Vector2 pointOffset, Vector2 direction, float distance, LayerMask mask)
    {
        return Physics2D.Raycast(transform.position + (Vector3)pointOffset, direction, distance, mask);
    }

    public void DrawCustomRay(Vector2 pointOffset, Vector2 direction, float distance)
    {
        Debug.DrawRay(transform.position + (Vector3)pointOffset, direction * distance, Color.yellow);
    }
 
    public void setCenterYOffset(float val) { centerYOffset = val; }
       
       
    
}