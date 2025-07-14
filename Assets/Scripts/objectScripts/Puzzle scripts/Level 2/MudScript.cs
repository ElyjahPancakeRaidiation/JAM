using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudScript : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private GameObject player;
    private PlayerMovement pm;
    private PlayerAbilities pa;
    private ParticleSystem mudParticles;
    private bool playerWithin;
    //Default values that are saved on start
    private float defaultCOF;
    private Vector2 defaultPogoSpeed;
    private Vector3 defaultAbilityPower; //x is dashx, y is dashy, and z is the height of the megajump
    ///////////////////////////////////////////
    
    [SerializeField] private float mudCOF;
    [SerializeField] private float dashPowerModifier;
    [SerializeField] private float splashLimit;

    void Start()
    {
        _collider = GetComponent<PolygonCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        pm = player.GetComponent<PlayerMovement>();
        pa = player.GetComponent<PlayerAbilities>();

        defaultCOF = pm.getCoefficientOfFriction(); // Store the default coefficient of friction
        defaultPogoSpeed = pm.GetJumpSpeed();
        defaultAbilityPower = pa.getAbilityPower();
        Debug.Log(defaultAbilityPower.z);
    }
    void Update()
    {
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("GHHGEOFGKIJOAIHNFA");
            if (pm.getCoefficientOfFriction() != mudCOF)
            {
                pm.setCoefficientOfFriction(mudCOF);
                pm.SetJumpSpeed(defaultPogoSpeed*0.5f);
                pa.setAbilityPower(defaultAbilityPower.x * dashPowerModifier, defaultAbilityPower.y * dashPowerModifier, defaultAbilityPower.z * (dashPowerModifier/2f));
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (pm.getCoefficientOfFriction() != defaultCOF)
            {
                pm.setCoefficientOfFriction(defaultCOF);
                pm.SetJumpSpeed(defaultPogoSpeed);
                pa.setAbilityPower(defaultAbilityPower.x, defaultAbilityPower.y, defaultAbilityPower.z);
            }
        }
    }
}
//gotta emit particles based on entering mud, and change from disabling abilities to simply altering their strength