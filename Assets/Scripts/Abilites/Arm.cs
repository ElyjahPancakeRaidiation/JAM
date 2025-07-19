using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Arm", menuName = "Arm Piece/New Arm")]
public class Arm : Torso
{
    // Start is called before the first frame update
    public override void formSetting(Rigidbody2D _rb, SpriteRenderer _sr, Collider2D _cir, Collider2D _box)
    {
        _box.enabled = false;
        _cir.enabled = true;
        _sr.sprite = sprite;
        _rb.freezeRotation = false;
        _rb.gameObject.GetComponent<PlayerMovement>().setSpeed(movementSpeed);
        _box.gameObject.GetComponent<PlayerMovement>().turnOnArms();

        // _rb.gameObject.GetComponent<PlayerAbilities>().setGroundDistance(groundChecker);
    }

}
