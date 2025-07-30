using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroLevel1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.current.pauseEvent += GetComponent<AudioSource>().Pause;
        GameManager.current.unPauseEvent += GetComponent<AudioSource>().UnPause;
    }
    void OnDestroy()
    {
        GameManager.current.pauseEvent -= GetComponent<AudioSource>().Pause;
        GameManager.current.unPauseEvent -= GetComponent<AudioSource>().UnPause;
    }

}
