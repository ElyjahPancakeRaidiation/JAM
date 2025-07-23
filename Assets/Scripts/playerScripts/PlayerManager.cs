using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager playerManager;
    public bool canControl { get; set; }

    public Rigidbody2D _rb { get; private set; }
    public SpriteRenderer _spr { get; private set; }
    public CircleCollider2D _circleCol { get; private set; }
    public BoxCollider2D _boxCol { get; private set; }

    #region Player scripts
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAbilities playerAbilites;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private isGroundedScript groundedScript;
    [SerializeField] private AudioManagerV2 audioManager;
    #endregion

    #region Global isgrounded variables
    [SerializeField] private Vector2 globalGroundColSize;
    [SerializeField] private Vector2 globalGroundColPointOffset;
    private float angle;
    #endregion

    #region Forms
    public List<PlayerFormsScriptables> playerForms;
    public int curForm;
    private int maxForms => playerForms.Count - 1;
    #endregion


    #region Player Keybinds
    public KeyCode playerAbilityKey;
    public KeyCode playerSwitchFormKey;

    #endregion

    private GameObject failedObject;

    public PlayerMovement PlayerMovement()
    {
        if (playerMovement != null)
        {
            return playerMovement;
        }
        else
        {
            failedObject.GetOrAddComponent<PlayerMovement>().enabled = false;
        }
        Debug.LogError(MissingMovement());
        return failedObject.GetComponent<PlayerMovement>();
    }
    public PlayerAbilities PlayerAbility()
    {
        if (playerAbilites != null)
        {
            return playerAbilites;
        }
        else
        {
            failedObject.GetOrAddComponent<PlayerAbilities>().enabled = false;
        }
        Debug.LogError(MissingAbilities());
        return failedObject.GetComponent<PlayerAbilities>();
    }
    public isGroundedScript IsGrounded()
    {
        if (groundedScript != null)
        {
            return groundedScript;
        }
        else
        {
            failedObject.GetOrAddComponent<isGroundedScript>().enabled = false;
        }
        Debug.LogError(MissingIsGroundedScript());
        return failedObject.GetComponent<isGroundedScript>();
    }
    public InputManager InputManager()
    {
        if (inputManager != null)
        {
            return inputManager;
        }
        else
        {
            failedObject.GetOrAddComponent<InputManager>().enabled = false;
        }
        Debug.LogError(InputManager());
        return failedObject.GetComponent<InputManager>();
    }

    //Add all of the other keybinds into this
    private void Awake()
    {
        if (playerManager == null) { playerManager = this; }
    }

    private void Start()
    {
        if(failedObject==null){failedObject = Instantiate(new GameObject("FailedInstances"), new Vector3(0, 0), quaternion.identity);}

        _rb = GetComponent<Rigidbody2D>();
        _spr = GetComponent<SpriteRenderer>();
        _circleCol = GetComponent<CircleCollider2D>();
        _boxCol = GetComponent<BoxCollider2D>();
        canControl = true;

        //Adds all the necessary componenets that comes with the forms
        AddFormsComponents();
        OnStartForm();

        if (!IsPlayerFormsEmpty())
        {
            playerForms[curForm].changeForm(_rb, _spr, _circleCol, _boxCol);
            SetGlobalGroundedColSize(GetCurPlayerForm().globalGroundedColSize);
            SetGlobalGroundedPointOffset(GetCurPlayerForm().glboalGroundedPointOffset);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(playerSwitchFormKey))
        {
            ChangeForm();
        }
    }

    public void PlayPlayerSfx(string sfxName)
    {
        if (audioManager != null)
        {
            StartCoroutine(audioManager.playPlayerSFX(sfxName));
        }
        else { Debug.LogError("Missing AudioManager"); }
    }

    #region Forms functions
    public PlayerFormsScriptables GetCurPlayerForm()
    {
        if (playerForms[curForm] == null || curForm > playerForms.Count - 1)
        {
            ChangeForm();
        }
        return playerForms[curForm];
    }
    public PlayerFormsScriptables.FormFunctionality GetFormFunctionality()
    {
        if (GetCurPlayerForm().isFunctionalityNull())
        {
            ///Saftey proofs the function so if the player is missing its abilities functionality for any reason
            ///it will automatically add it back to the player
            Debug.Log(missingFunctionality());
            GetCurPlayerForm().restartFormFunctionality(this.gameObject);
        }
        return GetCurPlayerForm().functionality;
    }
    public void AddForm(PlayerFormsScriptables newForm)
    {
        playerForms.Add(newForm);
        newForm.restartFormFunctionality(this.gameObject);
    }
    public void AddFormsComponents()
    {
        foreach (PlayerFormsScriptables ability in playerForms)
        {
            if (ability != null)
            {
                ability.addComponent(this.gameObject);
            }
        }
    }
    public void OnStartForm()
    {
        foreach (PlayerFormsScriptables ability in playerForms)
        {
            if (ability != null)
            {
                ability.functionality.OnStartMethod(ability);

            }
        }
    }
    public void ChangeForm()
    {
        if (canControl)
        {
            if (curForm == maxForms)
            {
                curForm = 0;
            }
            else
            {
                curForm++;
                if (playerForms[curForm] == null) { curForm = 0; }
            }

            playerForms[curForm].changeForm(_rb, _spr, _circleCol, _boxCol);
            SetGlobalGroundedColSize(GetCurPlayerForm().globalGroundedColSize);
            SetGlobalGroundedPointOffset(GetCurPlayerForm().glboalGroundedPointOffset);
        }
    }
    public bool IsPlayerFormsEmpty()
    {
        if (playerForms.Count == 0)
        {
            return true;
        }
        return false;
    }

    #endregion

    public float GetHorizontalInput()
    {
        if (inputManager != null) { return inputManager.getHorizontalInput(); }
        MissingInput();
        return 0;
    }

    public bool GlobalIsGrounded()
    {
        if (groundedScript != null)
        {
            return groundedScript.isGroundedBox(transform.position, globalGroundColPointOffset, globalGroundColSize);
        }
        MissingIsGroundedScript();
        return false;
    }
    public void SetGlobalGroundedColSize(Vector2 val){ globalGroundColSize = val; }
    public void SetGlobalGroundedPointOffset(Vector2 val){ globalGroundColPointOffset = val; }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)globalGroundColPointOffset, globalGroundColSize);
    }

    #region Error messages
    private string missingFunctionality()
    {
        return "Missing " + GetCurPlayerForm().formName + " functionality is missing. It has now been added to " + this.gameObject.name;
    }
    private string MissingIsGroundedScript() { return "Missing isGroundedScript"; }
    private string MissingMovement() { return "Missing Movement script"; }
    private string MissingAbilities() { return "Missing Abilities script"; }
    private string MissingInput() { return "Missing Input Manager script"; }

    #endregion

}
