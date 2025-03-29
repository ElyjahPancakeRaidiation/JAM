using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    [SerializeField]private ParticleSystem rainShit;//contains rain particles componenent
    [SerializeField]private float amountOfParticles;
    [SerializeField]private float speed;

    // Start is called before the first frame update
    void Start()
    {
        if(rainShit == null){Debug.LogError("Rai shit particle system in null");}//saftey incase its null
        var emissions = rainShit.emission;
        var main = rainShit.main;
        emissions.rateOverTime = amountOfParticles;
        main.simulationSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        var emissions = rainShit.emission;
        var main = rainShit.main;
        emissions.rateOverTime = amountOfParticles;
        main.simulationSpeed = speed;
    }

    public IEnumerator decreaseAmountOfParticles(float decreaseAmount, float goal){
        
        var emissions = rainShit.emission;
        float rateOverTimeAmount = emissions.rateOverTime.constant;

        if(rateOverTimeAmount == 0){yield break;}//Make sure it doesnt go to the negatives

        while(rateOverTimeAmount > goal){
            rateOverTimeAmount -= decreaseAmount * Time.deltaTime;
            emissions.rateOverTime = rateOverTimeAmount;
            rateOverTimeAmount = emissions.rateOverTime.constant;
            yield return null;
        }
        if(rateOverTimeAmount < 0){rateOverTimeAmount = 0;}
    }

    public IEnumerator increaseAmountOfParticles(float increaseAmount, float goal){
        var emissions = rainShit.emission;
        float rateOverTimeAmount = emissions.rateOverTime.constant;
        while(rateOverTimeAmount < goal){
            rateOverTimeAmount += increaseAmount * Time.deltaTime;
            emissions.rateOverTime = rateOverTimeAmount;
            rateOverTimeAmount = emissions.rateOverTime.constant;
            yield return null;
        }
    }
}
