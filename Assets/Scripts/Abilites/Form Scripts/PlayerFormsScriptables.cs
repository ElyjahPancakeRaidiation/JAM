using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerForms", menuName = "Player Forms/New Player Form")]
public abstract class PlayerFormsScriptables : ScriptableObject
{
    public string formName;
    public float movementSpeed;
    public Sprite formSprite;

    [Header("Main IsGrounded variables")]
    public Vector2 groundPointOffset;
    [Header("Global ground cast variables")]
    public Vector2 globalGroundedColSize;
    public Vector2 glboalGroundedPointOffset;

    public FormFunctionality functionality { get; set; }
    
    public abstract void changeForm(Rigidbody2D _rb, SpriteRenderer _spr, Collider2D _circleCol, Collider2D _boxCol);
    //Adds the scriptables designated functionality script that inheirts from FormFunctionality
    public abstract void addComponent(GameObject thisObject);
    //Removes its scriptables designated functionality script
    public void removeComponent()
    {
        if (functionality != null)
        {
            Destroy(functionality);
        }
    }
    public void ChangeFormFunctionality()
    {
        //This is incase you want a specific action to happen everytime you change form
        functionality.changeFormFunctionality();
    }
    public bool isFunctionalityNull() { return functionality == null; }
    public void restartFormFunctionality(GameObject thisObject)
    {
        addComponent(thisObject);
        functionality.OnStartMethod(this);
    }
    
    
    public abstract class FormFunctionality : MonoBehaviour
    {
        protected PlayerManager playerManager;
        protected Rigidbody2D _rb;
        public float movementMultipliers { get; set; } = 1;
        public float abilityMultipliers { get; set; } = 1;

        public virtual void OnStartMethod(PlayerFormsScriptables f)
        {
            //These will be played in the start method in playermanager so there shouldn't be to much of a delay.
            playerManager = this.gameObject.GetComponent<PlayerManager>();
            _rb = playerManager._rb;
            MudScript.setAllAbilitesToDefault += setMultipliersToDefulat;
        }
        
        public virtual void changeFormFunctionality() {}

        //update movement and ability are virtual methods meaning scripts that inheirt from it do not require these methods
        public virtual void UpdateMethodMovement() { }
        public virtual void UpdateMethodAbility() {}
        public abstract void FormMovement();
        public abstract void FormAbility();
        private void setMultipliersToDefulat()
        {
            movementMultipliers = 1;
            abilityMultipliers = 1;
        }
    }

}
