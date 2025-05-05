using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudScript : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private GameObject player;
    private PlayerMovement playerMovement;
    private PlayerAbilities playerAbilities;
    private GameObject splashObject;
    private ParticleSystem mudParticles;
    private AudioSource audio;
    [SerializeField]private float defaultCOF;
    [SerializeField] private float mudCOF;
    [SerializeField]private float splashLimit;

    void Start()
    {  
        _collider = GetComponent<PolygonCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerAbilities = player.GetComponent<PlayerAbilities>();
        audio = GetComponent<AudioSource>();

        splashObject = GameObject.FindGameObjectWithTag("SplashParticles");
        mudParticles = splashObject.GetComponent<ParticleSystem>();
        audio = splashObject.GetComponent<AudioSource>();

        defaultCOF = playerMovement.getCoefficientOfFriction(); // Store the default coefficient of friction
    }
    void Update()
    {
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("bro in the mud");
            splashObject.transform.position = new Vector2(collision.transform.position.x, collision.transform.position.y - 0.5f);
            float ySpeed = Mathf.Abs(collision.GetComponent<Rigidbody2D>().velocity.y);
            if(ySpeed > splashLimit){
                if(!audio.isPlaying)
                {
                    audio.Play();
                }
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.startColor = new Color(0.60f, 0.2f, 0.0f); //change this so that we get the color from the sprite renderer
                emitParams.startSize = 0.2f;
                mudParticles.Emit(emitParams, (int)ySpeed); 
            }
            playerMovement.setCoefficientOfFriction(mudCOF);
            playerAbilities.setUseAbility(false);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("bro out of the mud");
            playerMovement.setCoefficientOfFriction(defaultCOF);
            playerAbilities.setUseAbility(true);
        }
    }
}
