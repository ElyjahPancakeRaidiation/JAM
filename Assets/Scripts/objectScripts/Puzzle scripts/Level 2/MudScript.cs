using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudScript : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private GameObject player;
    private PlayerManager playerManager;
    private ParticleSystem mudParticles;
    private bool playerWithin;
    //Default values that are saved on start
    private float defaultCOF;
    private Vector2 defaultPogoSpeed;
    private Vector3 defaultAbilityPower; //x is dashx, y is dashy, and z is the height of the megajump
                                         ///////////////////////////////////////////

    [SerializeField] private float mudCOF;
    [SerializeField] private float decreaseAbilityModifier;
    [SerializeField] private float decreaseMovementModifier=1;
    [SerializeField] private float splashLimit;

    void Start()
    {
        _collider = GetComponent<PolygonCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();

        defaultCOF = playerManager.PlayerMovement().getCoefficientOfFriction();
        // defaultCOF = playerManager.PlayerMovement().getCoefficientOfFriction(); // Store the default coefficient of friction
        // defaultAbilityPower = pa.getAbilityPower();
        //Debug.Log(defaultAbilityPower.z);
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("GHHGEOFGKIJOAIHNFA");
            if (playerManager.PlayerMovement().getCoefficientOfFriction() != mudCOF)
            {
                playerManager.PlayerMovement().setCoefficientOfFriction(mudCOF);
                var formFunctionality = playerManager.GetFormFunctionality();
                formFunctionality.movementMultipliers = DecreaseMovementPowerModifier(playerManager.GetCurPlayerForm().formName);
                formFunctionality.abilityMultipliers = DecreaseAbilityPowerModifier(playerManager.GetCurPlayerForm().formName);
                // pm.setCoefficientOfFriction(mudCOF);
                // pm.SetJumpSpeed(defaultPogoSpeed * 0.5f);
                // pa.setAbilityPower(defaultAbilityPower.x * decreaseAbilityModifier, defaultAbilityPower.y * decreaseAbilityModifier, defaultAbilityPower.z * (decreaseAbilityModifier / 2f));
            }
        }
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerManager.PlayerMovement().getCoefficientOfFriction() != defaultCOF)
            {
                playerManager.PlayerMovement().setCoefficientOfFriction(defaultCOF);
                var formFunctionality = playerManager.GetFormFunctionality();
                formFunctionality.movementMultipliers = 1;
                formFunctionality.abilityMultipliers = 1;

                // pm.setCoefficientOfFriction(defaultCOF);
                // pm.SetJumpSpeed(defaultPogoSpeed);
                // pa.setAbilityPower(defaultAbilityPower.x, defaultAbilityPower.y, defaultAbilityPower.z);
            }
        }
    }

    private float DecreaseAbilityPowerModifier(string formName)
    {
        switch (formName)
        {
            case "Torso/Arm":
                return decreaseAbilityModifier / 2f;
            case "Ultimate Ball":
                return 200f;
            default:
                return decreaseAbilityModifier;
        }
    }
    private float DecreaseMovementPowerModifier(string formName)
    {
        switch (formName)
        {
            case "Torso/Arm":
                return 0.5f;
            case "Ultimate Ball":
                return 500;
            default:
                return decreaseMovementModifier;
        }
    }
}
//gotta emit particles based on entering mud, and change from disabling abilities to simply altering their strength