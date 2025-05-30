using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]private AudioManager audioManager;
    [SerializeField]private AudioClip transitionAudio;
    [SerializeField]private float fadeOutTime, fadeInTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isPlayerIn = other.gameObject.CompareTag("Player");
        if(isPlayerIn){
            StartCoroutine(audioManager.transitionClipE(transitionAudio, fadeOutTime, fadeInTime));
        }
    }
}
