using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightningScript : MonoBehaviour
{
    [System.Serializable]
    public class LightningBounds{
        public float minX;
        public float maxX;
        public LightningBounds(float minX, float maxX){
            this.minX = minX;
            this.maxX = maxX;
        }
    }
    public GameObject player;
    [SerializeField] private List<LightningBounds> lightningBounds;
    private bool playerInBounds = false;
    private Light2D light2D;
    [SerializeField] private float maxIntensity;
    [SerializeField]private float lightningDuration = 0.3f; //how long it takes for lighting to transition back to normal from a lightning strike
    private bool lightningActive = false; //is the lightning coroutine currently running
    private AudioSource audioSource;
    private bool playingLightning; //is lightning currently playing
    private float betweenTime; //how long between checking for lightning
    [SerializeField] private float timeBetweenLightning;
    [SerializeField] private int chanceOfLightning; //chance of lightning per check out of 100

    void Start()
    {
        light2D = GetComponent<Light2D>();
        light2D.intensity = 0.0f; //start with no additional light
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        checkPlayerBounds();
        if (playerInBounds && !lightningActive){
            StartCoroutine(lightning());
        }
    }
    private void checkPlayerBounds()
    {
        foreach (LightningBounds lb in lightningBounds)
        {
            if (player.transform.position.x >= lb.minX && player.transform.position.x <= lb.maxX)
            {
                playerInBounds = true;
                Debug.Log("Player in bounds");
                return;
            }
        }
        playerInBounds = false; //if player is not in any of the bounds, set to false
    }
    private IEnumerator lightning(){
        lightningActive = true;
        while (playerInBounds){
            int chance = new System.Random().Next(0, 100);
            Debug.Log(chance);
            if(chance < 50){ //50% chance of lightning every 5 sec
                playingLightning = true;
                light2D.intensity = maxIntensity;
                audioSource.Play();
                float elapsedTime = 0.0f;
                while (light2D.intensity > 0.0f)
                {
                    elapsedTime += Time.deltaTime;
                    light2D.intensity = Mathf.Lerp(maxIntensity, 0.0f, elapsedTime / lightningDuration);
                    yield return null;
                }
                playingLightning = false;
            }
            yield return new WaitForSeconds(timeBetweenLightning);
        }
        lightningActive = false;
    }
}
