using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Piece", menuName = "Drip Piece/New Drip")]
public class DripStickman : Torso
{
    [SerializeField]private float boxSizeMultiplier;
    public override void formSetting(Rigidbody2D _rb, SpriteRenderer _sr, Collider2D _cir, Collider2D _box)
    {
        base.formSetting(_rb, _sr, _cir, _box);
        _box.GetComponent<BoxCollider2D>().size = _sr.size * boxSizeMultiplier;
        // _rb.gameObject.GetComponent<PlayerAbilities>().setGroundDistance(groundChecker);
        
    }
}
