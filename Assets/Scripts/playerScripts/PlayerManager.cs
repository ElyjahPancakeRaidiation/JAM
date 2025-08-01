using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerObj;
    private PlayerMovement playerMovement;
    private PlayerAbilities playerAbilities;
    private isGroundedScript isGroundedScript;

    public PlayerMovement getplayerMovement()
    {
        if (playerMovement != null) { return playerMovement; }
        return null;
    }

    
    
    
}
