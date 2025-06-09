using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class PuzzleLighting : MonoBehaviour
{
    private readonly float normalGlobalLight = 2.33f;
    [SerializeField] private float initialFadeTime; //entering the puzzle section darkens the level
    [SerializeField] private float strikeLengthIn; //how long it takes to get full brightness
    [SerializeField] private float strikeLength; //how long it takes to return to darkness
    private Light2D globalLight;
    private Light2D lightningLight;
    private MovingPlatforms movingPlatforms;
    void Start()
    {
        globalLight = GetComponent<Light2D>();
        lightningLight = GameObject.FindGameObjectWithTag("Lightning").GetComponent<Light2D>();

        globalLight.intensity = normalGlobalLight;
        lightningLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator startLighting(MovingPlatforms movingPlatforms)
    {
        this.movingPlatforms = movingPlatforms;
        float elapsedTime = 0;
        while (elapsedTime < initialFadeTime)
        {
            globalLight.intensity = Mathf.Lerp(normalGlobalLight, 0, elapsedTime / initialFadeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        globalLight.intensity = 0;
        lightningLight.intensity = 0;
    }
    public IEnumerator lightning()
    {
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
    }
}
