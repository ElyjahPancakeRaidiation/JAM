using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class PuzzleLighting : MonoBehaviour
{
    private float normalGlobalLight = 2.33f;
    [SerializeField] private float initialFadeTime; //entering the puzzle section darkens the level
    [SerializeField] private float strikeLengthIn; //how long it takes to get full brightness
    [SerializeField] private float strikeLength; //how long it takes to return to darkness
    [SerializeField] private  float maxPlayerLightIntensity;
    [SerializeField] private float playerLightRadius;
    private Light2D globalLight;
    private Light2D lightningLight;
    private Light2D playerLight;
    private GameObject player;
    public bool playingLightning;
    public bool playingIntro;
    public bool playingOutro;

    void Start()
    {
        globalLight = GetComponent<Light2D>();
        normalGlobalLight = globalLight.intensity; //added
        lightningLight = GameObject.FindGameObjectWithTag("Lightning").GetComponent<Light2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerLight = player.GetComponent<Light2D>();
        playerLight.pointLightOuterRadius = playerLightRadius;

        //globalLight.intensity = normalGlobalLight;
        lightningLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator startLighting()
    {
        // player.SetActive(true);
        yield return new WaitUntil(() => !playingOutro);
        playingIntro = true;
        float elapsedTime = 0;
        //addPlayerLight();
        while (elapsedTime < initialFadeTime)
        {
            globalLight.intensity = Mathf.Lerp(normalGlobalLight, 0, elapsedTime / initialFadeTime);
            playerLight.intensity = Mathf.Lerp(0, maxPlayerLightIntensity, elapsedTime / initialFadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        globalLight.intensity = 0;
        lightningLight.intensity = 0;
        playingIntro = false;
    }
    public IEnumerator lightning()
    {
        playingLightning = true;
        float elapsedTime = 0;
        while (elapsedTime < strikeLengthIn)
        {
            lightningLight.intensity = Mathf.Lerp(0, 1, elapsedTime / strikeLengthIn);
            globalLight.intensity = Mathf.Lerp(0, normalGlobalLight, elapsedTime / strikeLengthIn);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0;
        while (elapsedTime < strikeLength)
        {
            lightningLight.intensity = Mathf.Lerp(1, 0, elapsedTime / strikeLength);
            globalLight.intensity = Mathf.Lerp(normalGlobalLight, 0, elapsedTime / strikeLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        lightningLight.intensity = 0;
        globalLight.intensity = 0;
        playingLightning = false;
    }
    public void addPlayerLight()
    {
        playerLight.lightType = Light2D.LightType.Point;
        playerLight.intensity = 0;
        playerLight.blendStyleIndex = 0;
        playerLight.pointLightOuterRadius = playerLightRadius;
    }
    public void removePlayerLight() {
        Destroy(player.GetComponent<Light2D>());
        // playerLight = null;
    }
    public IEnumerator stopLighting()
    {
        float elapsedTime = 0;
        while (elapsedTime < initialFadeTime)
        {
            globalLight.intensity = Mathf.Lerp(0, normalGlobalLight, elapsedTime / initialFadeTime);
            playerLight.intensity = Mathf.Lerp(maxPlayerLightIntensity, 0, elapsedTime / initialFadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        globalLight.intensity = normalGlobalLight;
        lightningLight.intensity = 0;
        //removePlayerLight();
    }
}
