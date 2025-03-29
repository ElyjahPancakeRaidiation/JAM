using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudScript : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private GameObject player;
    private PlayerMovement playerMovement;
    private PlayerAbilities playerAbilities;
    private GameObject dustSpawner;
    private DustScript dustScript;
    [SerializeField]private float defaultCOF;
    [SerializeField] private float mudCOF;
    void Start()
    {  
        _collider = GetComponent<PolygonCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAbilities = player.GetComponent<PlayerAbilities>();
        dustSpawner = GameObject.FindGameObjectWithTag("Dust");
        dustScript = dustSpawner.GetComponent<DustScript>();

        defaultCOF = playerMovement.getCoefficientOfFriction(); // Store the default coefficient of friction
    }
    void Update()
    {
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("bro in the mud");
        if(collision.CompareTag("Player"))
        {
            dustScript.setParticleColor(new Color(40f/255f, 20f/255f, 0f));
            //yuck dude
            dustScript.playDustParticles();
            playerMovement.setCoefficientOfFriction(mudCOF);
            playerAbilities.setUseAbility(false);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("bro out of the mud");
        if(collision.CompareTag("Player"))
        {
            dustScript.setParticleColor(Color.white);
            playerMovement.setCoefficientOfFriction(defaultCOF);
            playerAbilities.setUseAbility(true);
        }
    }
}
