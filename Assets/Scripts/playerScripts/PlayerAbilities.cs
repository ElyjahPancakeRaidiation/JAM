using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAbilities : MonoBehaviour
{
    PlayerManager playerManager;

    private bool canUseAbility;

    //onUseAbility event makes it easier for other script to know when the ability happens without being coupled to the ability. Usually used for sound.
    private UnityEvent onUseAbilityEvent;
    //Global wide events are a way for abilities to interact with each other but in a limited way where it doesn't have to depend on that ability.
    private UnityEvent globalWideAbilityEvent;

    // Start is called before the first frame update
    void Start()
    {
        if (onUseAbilityEvent == null){ onUseAbilityEvent = new UnityEvent(); }
        if (globalWideAbilityEvent == null) { globalWideAbilityEvent = new UnityEvent(); }
        playerManager = GetComponent<PlayerManager>();
        canUseAbility = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(playerManager.playerAbilityKey))
        {
            UseAbility();
        }

        playerManager.GetFormFunctionality().UpdateMethodAbility();
    }

    public void UseAbility()
    {
        if (!playerManager.IsPlayerFormsEmpty() && canUseAbility)
        {
            playerManager.GetFormFunctionality().FormAbility();
            if (onUseAbilityEvent != null) { onUseAbilityEvent.Invoke(); }
        }
    }

    public bool GetCanUseAbility() { return canUseAbility; }
    public void SetCanUseAbility(bool val) { canUseAbility = val; }
    public UnityEvent GetOnUseAbilityEvent()
    {
        if (onUseAbilityEvent == null){ onUseAbilityEvent = new UnityEvent(); }
        return onUseAbilityEvent;
    }
    public UnityEvent GetGlobalWideAbiltiyEvent()
    {
        if(globalWideAbilityEvent==null){ globalWideAbilityEvent = new UnityEvent(); }
        return globalWideAbilityEvent;
    }
}
