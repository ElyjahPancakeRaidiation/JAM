using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudScript : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private GameObject player;
    private PlayerMovement pm;
    private ParticleSystem mudParticles;
    [SerializeField]private float defaultCOF;
    [SerializeField] private float mudCOF;
    [SerializeField]private float splashLimit;

    void Start()
    {  
        _collider = GetComponent<PolygonCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        pm = player.GetComponent<PlayerMovement>();

        defaultCOF = pm.getCoefficientOfFriction(); // Store the default coefficient of friction
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
                pm.GetComponent<PlayerAbilities>().setUseAbility(false);
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
                pm.GetComponent<PlayerAbilities>().setUseAbility(true);
            }
        }
    }
}
