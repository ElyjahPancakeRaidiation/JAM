using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingManager : MonoBehaviour
{
    private Light2D globalLight;
    [Range(0.0f, 10.0f)]
    [Tooltip("Controls base lighting intensity of the scene without any freeforms. Will indirectly determine how dark the memory puzzle is, so don't set it too high.")]
    [SerializeField] private float ambientLighting;
    [Range(0.0f, 10.0f)]
    [Tooltip("Controls main lighting, basically self explanatory.")]
    [SerializeField] private float mainLighting;
    [SerializeField] private GameObject mainLightHolder;
    [SerializeField] private GameObject mushroomLightHolder;
    private GameObject player;
    private Light2D playerLight;
    [SerializeField] private AnimationCurve lightCurve;
    [SerializeField] private float playerLightRadius;
    [SerializeField] private float mushroomLightInnerRadius;
    [SerializeField] private float mushroomLightOuterRadius;
    [SerializeField] private float maxPlayerLightIntensity;
    [SerializeField] private float maxLightningLightIntensity;
    [SerializeField] private float maxMushroomLightIntensity;
    [SerializeField] private Light2D lightningSource;
    [HideInInspector] public bool playingLightning;
    [SerializeField] private float strikeLengthIn; //how long it takes to get full brightness
    [SerializeField] private float strikeLength; //how long it takes to return to darkness
    private List<Light2D> mushroomLightList;
    [SerializeField] private float mushroomOffset;

    void Start()
    {
        globalLight = GetComponent<Light2D>();
        globalLight.intensity = ambientLighting;

        foreach (Transform t in mainLightHolder.transform)
        {
            t.GetComponent<Light2D>().intensity = mainLighting;
        }

        player = GameObject.FindGameObjectWithTag("Player");
        playerLight = player.GetComponent<Light2D>();
        playerLight.pointLightOuterRadius = playerLightRadius;

        mushroomLightList = new List<Light2D>();
        foreach (Transform mushroom in mushroomLightHolder.transform)
        {
            Light2D mushroomLight = mushroom.GetComponent<Light2D>();
            mushroomLight.intensity = 0;
            if (mushroomLight) mushroomLightList.Add(mushroomLight);
        }

        if (!lightningSource)
        {
            Debug.Log("You haven't assigned a light2d to the lightning source in the lighting manager! Create one, place it in the holder, and pass it to the " +
            "LightingManager script. Make sure it is within the area of the moving platforms puzzle or things will look chopped.");

            lightningSource = Instantiate(new GameObject(), transform).AddComponent<Light2D>(); //temporary fix so we dont KILL the code if the source is missing
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        globalLight.intensity = ambientLighting;
        foreach (Transform t in mainLightHolder.transform)
        {
            t.GetComponent<Light2D>().intensity = mainLighting;
        }
    }
    public IEnumerator lightning()
    {
        playingLightning = true;
        float elapsedTime = 0;
        StopCoroutine(PulseMushrooms());
        StartCoroutine(PulseMushrooms());
        while (elapsedTime < strikeLengthIn)
        {
            lightningSource.intensity = Mathf.Lerp(0, maxLightningLightIntensity, elapsedTime / strikeLengthIn);
            // globalLight.intensity = Mathf.Lerp(0, normalGlobalLight, elapsedTime / strikeLengthIn);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0;
        while (elapsedTime < strikeLength)
        {
            lightningSource.intensity = Mathf.Lerp(maxLightningLightIntensity, 0, elapsedTime / strikeLength);
            // globalLight.intensity = Mathf.Lerp(normalGlobalLight, 0, elapsedTime / strikeLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        lightningSource.intensity = 0;
        //globalLight.intensity = 0;
        playingLightning = false;
    }
    public void addPlayerLight()
    {
        playerLight.lightType = Light2D.LightType.Point;
        playerLight.intensity = 0;
        playerLight.blendStyleIndex = 0;
        playerLight.pointLightOuterRadius = playerLightRadius;
    }

    public IEnumerator SetPlayerLight(bool v, float enterWaitTime)
    {
        if (v)
        {
            Debug.Log("player light needs tob e turned on here");
            float elapsedTime = 0;
            while (elapsedTime < enterWaitTime)
            {
                playerLight.intensity = Mathf.Lerp(0, maxPlayerLightIntensity, elapsedTime / enterWaitTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            float elapsedTime = 0;
            while (elapsedTime < enterWaitTime)
            {
                playerLight.intensity = Mathf.Lerp(maxPlayerLightIntensity, 0, elapsedTime / enterWaitTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
    public IEnumerator PulseMushrooms()
    {

        //create method that iterates through a list of mushroom lights, and essentially does the lightning effect
        //yield return new WaitForSeconds(0); //slight delay between lightnign and mushroom
        float elapsedTime = 0;
        while (elapsedTime < strikeLengthIn)
        {
            foreach (Light2D mushroomLight in mushroomLightList)
            {
                mushroomLight.intensity = Mathf.Lerp(0, maxMushroomLightIntensity, lightCurve.Evaluate(elapsedTime / strikeLengthIn));
                mushroomLight.pointLightOuterRadius = Mathf.Lerp(mushroomLightInnerRadius, mushroomLightOuterRadius, lightCurve.Evaluate(elapsedTime / strikeLengthIn));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(mushroomOffset);
        elapsedTime = 0;
        while (elapsedTime < strikeLength)
        {
            foreach (Light2D mushroomLight in mushroomLightList)
            {
                mushroomLight.intensity = Mathf.Lerp(maxMushroomLightIntensity, 0, lightCurve.Evaluate(elapsedTime / strikeLength));
                mushroomLight.pointLightOuterRadius = Mathf.Lerp(mushroomLightOuterRadius, mushroomLightInnerRadius, lightCurve.Evaluate(elapsedTime / strikeLength));
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        foreach (Light2D mushroomLight in mushroomLightList)
        {
            mushroomLight.intensity = 0;
        }
        yield return null;
    }
}
