using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAbilities : MonoBehaviour
{
    PlayerManager playerManager;

    private bool canUseAbility;

    private UnityEvent abilityEvent;

    // Start is called before the first frame update
    void Start()
    {
        abilityEvent = new UnityEvent();
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
            if (abilityEvent != null) { abilityEvent.Invoke(); }
        }
    }

    public bool GetCanUseAbility() { return canUseAbility; }
    public void SetCanUseAbility(bool val) { canUseAbility = val; }
    public UnityEvent GetAbilityEvent()
    {
        return abilityEvent;
    }
}
