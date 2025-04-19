using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "Pogo Piece/New Pogo")]
public class Torso : AbilitySettingScriptable
{
    [SerializeField]private Vector2 boxColliderSize;
    
    public override void formSetting(Rigidbody2D _rb, SpriteRenderer _sr, Collider2D _cir, Collider2D _box)
    {
        _box.enabled = true;
        _box.GetComponent<BoxCollider2D>().size = boxColliderSize;
        _cir.enabled = false;
        _sr.sprite = sprite;
        _rb.freezeRotation = true;
        _rb.gameObject.transform.rotation = quaternion.RotateZ(0);//Puts the character up straight
        _rb.gameObject.GetComponent<PlayerMovement>().setSpeed(movementSpeed);
        _rb.gameObject.GetComponent<PlayerAbilities>().setGroundDistance(groundChecker);
    }
}
