using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public abstract class AbilitySettingScriptable : ScriptableObject
{
    public Sprite sprite;
    public float movementSpeed;
    public string formName;
    public bool formAddOn = false;
    public Vector2 groundChecker;
    public Vector2 startPositionOffset;
    
    //Customizable method that will hold the form settings for each piece
    public abstract void formSetting(Rigidbody2D rb, SpriteRenderer sr, Collider2D cir, Collider2D box);

}
