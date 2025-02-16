using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjSound : MonoBehaviour
{
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Find if player is in the collider and start the sound link the volume to the players speed(rigidbody)
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //Only start doing this section when the first audio from ontriggerenter stops and continuosly link the sound with the players speed?
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //Fade out the audio to 0 very quickly and make it stop once its 0.
    }
}
