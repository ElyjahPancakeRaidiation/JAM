using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAudioPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AudioSource source;

    [SerializeField] bool isPlayerWithinZone;

    ManageWind manageWind;

    public float pitchUpper, pitchLower, volumeower, volumeUpper;
    void Awake()
    {
        manageWind = GameObject.FindGameObjectWithTag("Wind").GetComponent<ManageWind>();
    }

    //     // Update is called once per frame
    void Update()
    {
        //isPlayerWithinZone = manageWind.IsPlayerWithinZone();
        PlayAudio();
    }

    private void Start()
    {

        //source.volume = 0f;


    }
    void PlayAudio()
    {
        if (isPlayerWithinZone)
        {
            // StartCoroutine(Fade(true, source, 8f, 1f));
            // StartCoroutine(Fade(false, source, 8f, .6f));
            source.Play();
            Debug.Log("Playing");
        }
        else source.Stop();

    }



    public IEnumerator Fade(bool fadeIn, AudioSource source, float duration, float targetVolume)
    {
        if (!fadeIn)
        {
            double lengthOfSource = (double)source.clip.samples / source.clip.frequency;
            yield return new WaitForSecondsRealtime((float)(lengthOfSource - duration));
        }

        float time = 0f;
        float startVol = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
            yield return null;
        }

        yield break;
    }

}
