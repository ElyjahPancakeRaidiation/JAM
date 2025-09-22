using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Camera Movement Data", menuName = "New Camera Data/Movement")]
public class CamMovementData : ScriptableObject
{

    private enum MovementType
    {
        MoveTowards,
        Lerp,
        SmoothDamp
    }
    [SerializeField] private MovementType movementType;

    [SerializeField] private float speed;
    [SerializeField] private Vector2 offsetStartMovingPosition;
    private Vector2 refVel;

    private Vector2 OffsetDestPosition(int i)
    {
        if (i < 0)
        {
            return new Vector2(-offsetStartMovingPosition.x, offsetStartMovingPosition.y);
        }
        else if (i == 0)
        {
            return Vector2.zero;
        }
        return offsetStartMovingPosition;
    }


    public Vector2 MovePosition(Vector2 current, int dir)
    {
        return Vector2.SmoothDamp(current, OffsetDestPosition(dir), ref refVel, speed);
    }
    
}
