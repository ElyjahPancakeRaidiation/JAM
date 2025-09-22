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

    [Header("Arm variables")]//BTW I think the mass of the vine segments need to be half the mass of the player to be fluid
    //This states the scene in which the arm will be activated in
    [SerializeField] private int[] armsActiveScene;
    [SerializeField] private bool hasArms;
    [SerializeField] private float swingForce;//players force applied perpendicular to the vector pointed to the hook of the vine
    [SerializeField] private float armGrabZone;//zone where when ability button is pressed player grabs onto potential arms
    [SerializeField] private float armDetectionZone;//zone where when vine gameobjects enter, the nearest arm will begin to point towards it
    [SerializeField] private float pullForce;//how much force is applied in the direction of the vine's hook when pulling, per distance from hook
    [SerializeField] private float detachMultiplier;// multiplier of how much speed we get when letting go of vine


    #endregion

    public override void changeForm(Rigidbody2D _rb, SpriteRenderer _spr, Collider2D _circleCol, Collider2D _boxCol)
    {
        if (functionality != null) { functionality.changeFormFunctionality(); }
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
        private HingeJoint2D armJoint;
        private Transform currentVine; //the vine that the player is currently attached to
        private GameObject[] armsArray;

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
                armsArray = GameObject.FindGameObjectsWithTag("Arm");
                SetArms(false);
                armJoint = GetComponent<HingeJoint2D>();
                if (armJoint == null) { armJoint = gameObject.AddComponent<HingeJoint2D>(); }
                armJoint.enabled = false;
            }

        }

        public override void UpdateMethodMovement()
        {
            coyoteTimer -= Time.deltaTime;
            if (Input.GetKeyDown(playerManager.playerSwitchFormKey))
            {
                if (currentVine)
                {
                    Detach();
                }
            }
        }

        private void Update()
        {
            //Only using this for the visual and nothing important like movement wise or collider
            //This ensures that there isn't to many updates running at the same time in the player
            if (torsoVar.hasArms)
            {
                var isCurrentForm = playerManager.GetCurPlayerForm().formName == torsoVar.formName;
                if (isCurrentForm)
                {
                    SetArms(true);
                }
                else { SetArms(false); }
            }
        }

        public override void FormMovement()
        {
            //leaving out the friction being off while in vine for now
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
            Vector2 directionToHook = currentVine.GetComponent<Vine>().hook.transform.position - transform.position;
            float angle = Vector2.Angle(Vector2.down, directionToHook.normalized) * Mathf.Deg2Rad;
            float angleFactor = Mathf.Sin(angle);
            _rb.AddRelativeForce(playerManager.GetHorizontalInput() * -1 * Vector2.Perpendicular(directionToHook).normalized * Mathf.Lerp(torsoVar.swingForce, 0, angleFactor)); //HELP HELP ME HELP || I LOVE REWRTIGING I LOVE I LVOE I LOVEI LVOE I LOVEI LO VEIL VOE - E
        }

        #endregion

        #region Ability methods        
        //Ability methods
        GameObject GetArm(GameObject vine, int active) //get the active or inactive arm
        {
            foreach (GameObject arm in armsArray)
            {
                ArmScript armScript = arm.GetComponent<ArmScript>();
                if (active == 1)
                {
                    if (armScript.IsActiveArm(vine.transform.position)) return arm;
                }
                else
                {
                    if (!armScript.IsActiveArm(vine.transform.position)) return arm;
                }
            }
            return null;
        }
        public override void UpdateMethodAbility()
        {
            base.UpdateMethodAbility();
            //Check the vicinity for vine segments
            if (torsoVar.hasArms)
            {
                if (!currentVine)
                {   
                    Collider2D[] vines = Physics2D.OverlapCircleAll(transform.position, torsoVar.armDetectionZone, LayerMask.GetMask("Vine"));
                    if (vines.Length > 0)
                    {
                        GetArm(vines[0].gameObject, 1).GetComponent<ArmScript>().PointToGameObject(vines[0].gameObject);
                        GetArm(vines[0].gameObject, 0).GetComponent<ArmScript>().MoveToReset();
                    }
                    else
                    {
                        foreach (GameObject arm in armsArray)
                        {
                            arm.GetComponent<ArmScript>().MoveToReset();
                        }
                    }
                }
            }

        }
        private void JumpAbility()
        {
            if (!currentVine)
            {
                if (playerManager.IsGrounded().isGroundedBox(transform.position, torsoVar.jumpGroundOffsetSize, torsoVar.jumpGroundColliderSize))
                {
                    StartCoroutine(JumpAbilityIEnumerator());
                }
                else if (coyoteTimer > .4f && coyoteTimer < .65f)
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
            float jumpForce = Mathf.Sqrt(torsoVar.jumpHeight * abilityMultipliers * Physics2D.gravity.y * _rb.gravityScale * -2) * _rb.mass;
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
            Vector2 hopForce = new Vector2(playerManager.GetHorizontalInput() * (torsoVar.jumpSpeedX * movementMultipliers), (torsoVar.jumpSpeedY * movementMultipliers));
            _rb.velocity = hopForce;
            yield return new WaitForSeconds(.6f);
            //keep checking until the player touches the ground
            yield return new WaitUntil(() => playerManager.IsGrounded().isGroundedBox(transform.position, torsoVar.hoppingGroundOffsetSize, torsoVar.hoppingGroundColliderSize)/*playerAbility.groundedScript()*/);
        }

        #endregion

        //Felix arms method
        private void CheckForVines()
        {
            Debug.Log(armsArray.Length);
            foreach (GameObject arm in armsArray) //check each hand for a vine segment, if present then attach
            {
                Collider2D collider = Physics2D.OverlapCircle(arm.transform.position, torsoVar.armGrabZone, LayerMask.GetMask("Vine"));
                if (collider)
                {
                    playerManager.PlayerAbility().GetGlobalWideAbiltiyEvent().Invoke();
                    armJoint.enabled = true; //enable the hingejoint2d on player
                    armJoint.connectedBody = collider.gameObject.GetComponent<Rigidbody2D>(); //connect arms hinge to the vine segment
                    armJoint.connectedAnchor = arm.transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size; //this might have to change to make grabbing look more realistic
                    currentVine = collider.transform.parent; //update currentvine
                }
            }
        }
        private void Detach()
        {
            armJoint.connectedBody = null;
            armJoint.enabled = false;
            Vine currentVineScript = currentVine.GetComponent<Vine>();
            Vector2 directionToHook = currentVineScript.hook.transform.position - transform.position;
            float input = playerManager.GetHorizontalInput();
            if (input == 0f) //no input => pull
            {
                _rb.AddForce(directionToHook.normalized * (!currentVineScript.PullForceOverwritten() ?
                (10 + torsoVar.pullForce * directionToHook.magnitude) : currentVineScript.VineSpecificForce()), ForceMode2D.Impulse);
                if (currentVineScript.PullForceOverwritten())
                {
                    Debug.Log("fuck you.");
                }
            }
            else //let go and increase velocity for an impactful "boost"
            {
                _rb.velocity *= torsoVar.detachMultiplier;
            }
            currentVine = null;
        }

        private void SetArms(bool activeStatus)
        {
            if (torsoVar.hasArms)
            {
                foreach (GameObject arm in armsArray)
                {
                    arm.SetActive(activeStatus);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + (Vector3)torsoVar.hoppingGroundOffsetSize, torsoVar.hoppingGroundColliderSize);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + (Vector3)torsoVar.jumpGroundOffsetSize, torsoVar.jumpGroundColliderSize);
            if (torsoVar.hasArms)
            {
                foreach (GameObject arm in armsArray)
                {
                    Gizmos.DrawSphere(arm.transform.position, torsoVar.armGrabZone);
                }
            }
            if (currentVine)
            {
                Vector2 directionToHook = currentVine.GetComponent<Vine>().hook.transform.position - transform.position;
                Vector2 playerPos = transform.position;
                Gizmos.DrawLine(playerPos, playerPos + Vector2.Perpendicular(directionToHook));
            }
        }
    }
    
}
