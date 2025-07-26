using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerForms", menuName = "Player Forms/New Torso Form")]
public class TorsoForm : PlayerFormsScriptables
{
    #region Extra variables
    [Header("Extra variables")]
    public float jumpSpeedX;
    public float jumpSpeedY;
    public float jumpHeight;
    [SerializeField] private float floatTime;
    [SerializeField] private Vector2 boxColliderSize;
    [SerializeField] private Vector2 hoppingGroundColliderSize;
    [SerializeField] private Vector2 hoppingGroundOffsetSize;

    [SerializeField] private Vector2 jumpGroundColliderSize;
    [SerializeField] private Vector2 jumpGroundOffsetSize;

    [Header("Arm variables")]
    //This states the scene in which the arm will be activated in
    [SerializeField] private int[] armsActiveScene;
    [SerializeField] private bool hasArms;
    [SerializeField] private float swingForce;


    #endregion

    public override void changeForm(Rigidbody2D _rb, SpriteRenderer _spr, Collider2D _circleCol, Collider2D _boxCol)
    {
        if(functionality!=null){ functionality.changeFormFunctionality(); }
        _boxCol.enabled = true;
        _boxCol.GetComponent<BoxCollider2D>().size = boxColliderSize;
        _circleCol.enabled = false;
        _spr.sprite = formSprite;
        _rb.freezeRotation = true;
        _rb.gameObject.transform.rotation = quaternion.RotateZ(0);//Puts the character up straight
    }

    public override void addComponent(GameObject thisObject)
    {
        if (functionality == null)
        {
            functionality = thisObject.AddComponent<TorsoFunctionality>();
        }
    }

    public class TorsoFunctionality : FormFunctionality
    {
        public TorsoForm torsoVar { get; set; }
        private bool jumpedClicked;
        public float coyoteTimer { get; set; }

        //Arm variables
        private HingeJoint2D arms;
        private Transform currentVine;
        private GameObject leftArm;
        private GameObject rightArm;

        public override void OnStartMethod(PlayerFormsScriptables f)
        {
            base.OnStartMethod(f);
            torsoVar = (TorsoForm)f;

            jumpedClicked = false;

            //Arms on start variables
            if (torsoVar.armsActiveScene.Length > 0)
            {
                for (int i = 0; i < torsoVar.armsActiveScene.Length; i++)
                {
                    if (torsoVar.armsActiveScene[i] == SceneManager.GetActiveScene().buildIndex)
                    {
                        torsoVar.hasArms = true;
                        break;
                    }
                }
            }

            if (torsoVar.hasArms)
            {
                rightArm = GameObject.FindGameObjectWithTag("Player Right Arm");
                leftArm = GameObject.FindGameObjectWithTag("Player Left Arm");
                SetArms(false);
                arms = GetComponent<HingeJoint2D>();
                if (arms == null) { arms = gameObject.AddComponent<HingeJoint2D>(); }
                arms.enabled = false;
            }

        }

        public override void UpdateMethodMovement()
        {
            coyoteTimer -= Time.deltaTime;
        }

        private void Update()
        {
            //Only using this for the visual and nothing important like movement wise or collider
            //This ensures that there isn't to many updates running at the same time in the player
            var isCurrentForm = playerManager.GetCurPlayerForm().formName == torsoVar.formName;
            if (isCurrentForm)
            {
                SetArms(true);
            }else{ SetArms(false); }
        }

        public override void FormMovement()
        {
            if (currentVine)
            {
                VineMovement();
            }
            else
            {
                TorsoMovement();
            }
        }

        public override void FormAbility() { JumpAbility(); }
        public override void changeFormFunctionality()
        {
            SetArms(true);
        }

        #region Movement methods
        //Movement methods
        private void TorsoMovement()
        {
            if (playerManager.GetHorizontalInput() != 0)
            {
                var checkForground = playerManager.IsGrounded().isGroundedBox(transform.position, torsoVar.hoppingGroundOffsetSize, torsoVar.hoppingGroundColliderSize);
                if (checkForground && !jumpedClicked)
                {
                    StartCoroutine(Hopping());
                }
                else
                {
                    _rb.AddForce(new Vector2(playerManager.GetHorizontalInput() * torsoVar.movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);
                }
            }
        }
        private void VineMovement()
        {
            _rb.AddRelativeForce(new Vector2(playerManager.GetHorizontalInput(), 0) * torsoVar.swingForce);
        }

        #endregion

        #region Ability methods        
        //Ability methods
        private void JumpAbility()
        {
            if (!currentVine)
            {
                if (playerManager.IsGrounded().isGroundedBox(transform.position, torsoVar.jumpGroundOffsetSize, torsoVar.jumpGroundColliderSize))
                {
                    StartCoroutine(JumpAbilityIEnumerator());
                }
                else if (coyoteTimer > .56f && coyoteTimer < .65f)
                {
                    StartCoroutine(JumpCoyoteTimerIEnumerator());
                    StartCoroutine(JumpAbilityIEnumerator());

                }
                else
                {
                    if (torsoVar.hasArms)
                    {
                        CheckForVines();
                    }
                }
            }
            else
            {
                Detach();
            }
        }

        #endregion


        #region IEnumerators
        //IEnumerator
        private IEnumerator JumpAbilityIEnumerator()
        {
            jumpedClicked = true;
            float jumpForce = Mathf.Sqrt((torsoVar.jumpHeight * abilityMultipliers) * Physics2D.gravity.y * _rb.gravityScale * -2) * _rb.mass;
            Vector2 Verticaldirection = new Vector2(_rb.velocity.x, jumpForce);
            _rb.velocity = Verticaldirection;
            yield return new WaitForSeconds(.1f);
            jumpedClicked = false;
        }

        private IEnumerator JumpCoyoteTimerIEnumerator()
        {
            yield return new WaitForSeconds(.03f);
            coyoteTimer = 0;
        }

        private IEnumerator Hopping()
        {
            coyoteTimer = torsoVar.floatTime;
            Vector2 jumpForce = new Vector2(playerManager.GetHorizontalInput() * (torsoVar.jumpSpeedX * movementMultipliers), (torsoVar.jumpSpeedY * movementMultipliers));
            _rb.velocity = jumpForce;
            yield return new WaitForSeconds(.6f);
            //keep checking until the player touches the ground
            yield return new WaitUntil(() => playerManager.IsGrounded().isGroundedBox(transform.position, torsoVar.hoppingGroundOffsetSize, torsoVar.hoppingGroundColliderSize)/*playerAbility.groundedScript()*/);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + (Vector3)torsoVar.hoppingGroundOffsetSize, torsoVar.hoppingGroundColliderSize);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (Vector3)torsoVar.jumpGroundOffsetSize, torsoVar.jumpGroundColliderSize);
        }

        //Felix arms method
        private void CheckForVines()
        {
            Debug.Log("inside check for vines");
            Collider2D collider = Physics2D.OverlapBox(gameObject.transform.position, GetComponent<BoxCollider2D>().bounds.size, 0f, LayerMask.GetMask("Vine"));
            if (collider)
            {
                arms.enabled = true;
                arms.connectedBody = collider.gameObject.GetComponent<Rigidbody2D>(); //connect arms hinge to the vine segment
                currentVine = collider.transform.parent;
            }
        }
        private void Detach()
        {
            arms.connectedBody = null;
            arms.enabled = false;
            currentVine = null;
        }

        private void SetArms(bool activeStatus)
        {
            rightArm?.SetActive(activeStatus);
            leftArm?.SetActive(activeStatus);
        }

        private bool IsArmsActive() {
            return rightArm.activeSelf && leftArm.activeSelf;
        }
        

    }
    
    
    
}
