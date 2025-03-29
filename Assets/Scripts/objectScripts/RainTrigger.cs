using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RainTrigger : MonoBehaviour
{
    private RainController rain; 
    private enum RainType{Decrease, Increase};
    [SerializeField]private RainType rainType;
    [SerializeField]private float amount;
    [SerializeField]private float goal;

    private Coroutine rainEnumerator;
    // Start is called before the first frame update
    void Start()
    {
        rain = GameObject.FindGameObjectWithTag("Rain Controller").GetComponent<RainController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            if(rainType == RainType.Increase){
                rainEnumerator = StartCoroutine(rain.increaseAmountOfParticles(amount, goal));
            }else if(rainType == RainType.Decrease){
                rainEnumerator = StartCoroutine(rain.decreaseAmountOfParticles(amount, goal));
            }
        }
    }
}
