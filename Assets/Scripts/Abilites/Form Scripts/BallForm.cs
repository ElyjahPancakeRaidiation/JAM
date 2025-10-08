using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerForms", menuName = "Player Forms/New Ball Form")]
public class BallForm : PlayerFormsScriptables
{   
    [Header("Extra variables")]
    public Vector2 groundColSize;
    public float maxSpeedPoint;
    public float smoothStopSpeed;
    public float dashPowerX;
    public float dashPowerY;
    public float unchangedDashY;
    public int maxDashes;
    public GameObject prefabDustSpawner;

    public override void changeForm(Rigidbody2D _rb, SpriteRenderer _spr, Collider2D _circleCol, Collider2D _boxCol)
    {
        _boxCol.enabled = false;
        _circleCol.enabled = true;
        _spr.sprite = formSprite;
        _rb.freezeRotation = false;
    }

    public override void addComponent(GameObject thisObject)
    {
        if (functionality == null)
        {
            functionality = thisObject.AddComponent<BallFunctionality>();
            // functionality.ballVar = this;
        }
    }

    public class BallFunctionality : FormFunctionality
    {
        public BallForm ballVar { get; set; }

        private bool withEasing;
        private bool isEasingOn;
        private float oppositeInput;
        private float angularVelHalf;
        private int dashAmount;
        private GameObject _dustSpawner;
        private Coroutine turnEasingBackOn;

        public override void setFloat(int value){
            dashAmount = value;
        }
       
       
        public override void OnStartMethod(PlayerFormsScriptables f)
        {
            base.OnStartMethod(f);
            ballVar = (BallForm)f;
            dashAmount = ballVar.maxDashes;
            isEasingOn = true;
            if (_dustSpawner == null) { _dustSpawner = Instantiate(ballVar.prefabDustSpawner); }
            //May have to rethink about this line
            playerManager.PlayerMovement().playerImpact += _dustSpawner.GetComponent<DustScriptV2>().playLandingParticles;
            playerManager.PlayerAbility().GetGlobalWideAbiltiyEvent().AddListener(GiveMaxDash);
        }
        public override void UpdateMethodMovement()
        {
            float curFloat = 0;
            if (isEasingOn)
            {

                if (withEasing)
                {
                    if (_rb.velocity.x >= -0.1f && _rb.velocity.x <= 0.1f)
                    {
                        withEasing = false;
                    }

                    //This piece of code ensures that easing is never on when it doesn't have to be
                    //Since the angularvelocity is directyl related to the direction the player is rolling to.
                    if (oppositeInput == 1 && _rb.angularVelocity < 0)
                    {
                        withEasing = false;
                    }
                    else if (oppositeInput == -1 && _rb.angularVelocity > 0)
                    {
                        withEasing = false;
                    }
                }
                if (withEasing && playerManager.GetHorizontalInput() == oppositeInput)
                {
                    Vector2 velocity = _rb.velocity;
                    //Smoothly brings down the velocity's x to a 0 making it a smooth stop when the player turns.
                    velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref curFloat, ballVar.smoothStopSpeed);
                    //This doesn't really do much although it is similar to what Tarin did with the player controller
                    angularVelHalf = -(_rb.angularVelocity / 2) * 10;
                    /*
                    This is used for the players rotation in the rigidbody mainly when its a ball. It's supposed to make sure the balls rotation is going 
                    the same as the players input however with further inspection this was done with the gravity
                    */
                    _rb.angularVelocity += angularVelHalf * Time.fixedDeltaTime;
                    _rb.velocity = velocity;
                }
            }
        }

        public override void FormMovement()
        {

            _rb.AddForce(new Vector2(playerManager.GetHorizontalInput() * (ballVar.movementSpeed * movementMultipliers) * Time.deltaTime, 0), ForceMode2D.Impulse);

            if (_rb.velocity.x <= -ballVar.maxSpeedPoint || _rb.velocity.x >= ballVar.maxSpeedPoint)
            {
                oppositeInput = -1 * (_rb.velocity.x / Mathf.Abs(_rb.velocity.x));
                if (playerManager.GetHorizontalInput() != oppositeInput)
                {
                    withEasing = true;
                }
            }
        }
        public override void FormAbility() { Dash(); }

        public void Dash()
        {

            if (dashAmount > 0)
            {
                playerManager.PlayPlayerSfx("Dashing");
                //based of the horizontal input -1, 0, 1
                //0 will now only go up might be good for more movement combinations?
                var horInput = playerManager.GetHorizontalInput();

                if (horInput != 0)
                {
                    _rb.velocity = Vector2.zero;
                    _rb.AddForce(new Vector2(horInput * (ballVar.dashPowerX * abilityMultipliers), (ballVar.dashPowerY * abilityMultipliers)), ForceMode2D.Impulse);
                }

                if (horInput == 0)
                {

                    _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y / 2);
                    _rb.AddForce(new Vector2(_rb.velocity.x / 100, Mathf.Max(ballVar.unchangedDashY, (_rb.velocity.y *-1) + ballVar.unchangedDashY)), ForceMode2D.Impulse);
                }

                dashAmount--;
                if (dashAmount == 0) { StartCoroutine(DashAmountBack()); }
            }
        }
        private IEnumerator DashAmountBack()
        {
            //This function puts a short cool down when getting your dash back since if this isnt here it will
            //automatically give your dash allowing double dash
            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => playerManager.GlobalIsGrounded());
            dashAmount = ballVar.maxDashes;
        }

        private void GiveMaxDash()
        {
            dashAmount = ballVar.maxDashes;
            // Debug.Log("gave dah");
        }

        private IEnumerator EasingBackOn()
        {
            yield return new WaitForSeconds(2f);
            isEasingOn = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + (Vector3)ballVar.groundPointOffset, ballVar.groundColSize);
        }

        void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("RainShit"))
            {
                isEasingOn = false;
                if (turnEasingBackOn != null)
                {//This if statement ensures that easing is not turned back on while rain is hiting the player.
                    StopCoroutine(turnEasingBackOn);
                    turnEasingBackOn = null;
                }
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("RainShit"))
            {
                if (turnEasingBackOn == null)
                {
                    turnEasingBackOn = StartCoroutine(EasingBackOn());
                }
            }
        }


    }
    
    
}
