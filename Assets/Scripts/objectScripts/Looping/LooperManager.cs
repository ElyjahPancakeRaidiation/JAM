using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooperManager : MonoBehaviour
{
    [SerializeField] private LoopingBackgroundScript[] loopers;
    private bool stopOnNext;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float stopPosition;


    // Start is called before the first frame update
    void Start()
    {
        if(audioSource == null ){ Debug.LogError("AudioManager is not set"); }
    }

    // Update is called once per frame
    void Update()
    {
        if (stopOnNext)
        {
            foreach (LoopingBackgroundScript loop in loopers)
            {
                loop.setStopOnNext(stopOnNext);
            }
        }

        if (audioSource != null)
        {
            if (audioSource.time >= stopPosition)
            {
                stopOnNext = true;
            }
        }

    }
}
