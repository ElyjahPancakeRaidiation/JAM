using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    public UnityEvent test; 
    private const float _TRANSITIONAMOUNT = .2f;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
       
    }


    public IEnumerator transitionClipE(AudioClip clip, float fadeOutTime, float fadeInTime){
        float originalVolume = audioSource.volume;
        while(audioSource.volume > 0){
            audioSource.volume -= _TRANSITIONAMOUNT; 
            yield return new WaitForSeconds(fadeOutTime/10);
        }
        //yield return new WaitWhile(() => audioSource.volume == 0);
        audioSource.clip = clip;
        audioSource.PlayDelayed(.1f);
        while(audioSource.volume < originalVolume){
            audioSource.volume += _TRANSITIONAMOUNT; 
            yield return new WaitForSeconds(fadeInTime/10);
        }

    }


    
}
