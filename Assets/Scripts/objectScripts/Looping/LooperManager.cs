using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooperManager : MonoBehaviour
{
    [SerializeField] private LoopingBackgroundScript[] loopers;
    [SerializeField] private bool stopOnNext;
    [SerializeField] private AudioManagerV2 audioManager;
    [SerializeField] private float stopPosition;


    // Start is called before the first frame update
    void Start()
    {
        if(audioManager == null ){ Debug.LogError("AudioManager is not set"); }
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

        if (audioManager.currentSource != null)
        {
            if (audioManager.currentSource.time >= stopPosition)
            {
                stopOnNext = true;
            }
        }

    }
}
