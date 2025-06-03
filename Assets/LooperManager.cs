using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooperManager : MonoBehaviour
{
    [SerializeField] private LoopingBackgroundScript[] loopers;
    [SerializeField] private bool stopOnNext;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (LoopingBackgroundScript loop in loopers)
        {
            loop.setStopOnNext(stopOnNext);
        }
    }
}
