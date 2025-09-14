using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    [SerializeField] private float angleOffset;
    [SerializeField] private Vector2 bodyOffset;
    [SerializeField] private HandType handType;
    private Transform bodyOrigin;

    public Vector3 updirection;
    
    private enum HandType
    {
        EvilRight,
        Right
    }
    void Start()
    {
        bodyOrigin = transform.parent;
        transform.position = transform.parent.position + (Vector3)bodyOffset;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleOffset);
        
        
    }

    void Update()
    {
        //Debug.Log(TestPointToMouse());
        
    }
    Vector2 TestPointToMouse()
    {
        Vector2 direction = (bodyOrigin.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        if (IsActiveArm(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle), Time.deltaTime * 10);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0), Time.deltaTime * 10);
        }
        //transform.RotateAround(bodyOrigin.position, Vector3.forward, angle);
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public void PointToGameObject(GameObject gameObject)
    {
        Vector2 direction = (bodyOrigin.position - gameObject.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angle), Time.deltaTime * 10);
    }
    public void MoveToReset()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, angleOffset), Time.deltaTime * 10);
    }
    public bool IsActiveArm(Vector2 point)
    {
        if (handType == HandType.Right)
        {
            return point.x > bodyOrigin.position.x; //ts ass
        }
        else
        {
            return point.x < bodyOrigin.position.x;
        }
    }
}
