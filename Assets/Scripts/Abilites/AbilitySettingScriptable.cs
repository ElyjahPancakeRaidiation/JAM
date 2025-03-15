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
    public float groundChecker;
    
    //Customizable method that will hold the form settings for each piece
    public abstract void formSetting(Rigidbody2D rb, SpriteRenderer sr, Collider2D cir, Collider2D box);

}
