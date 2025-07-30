using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneBlackBars : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        CutSceneManager.startCutsceneEvent?.AddListener(moveCutsceneBarsIn);
        CutSceneManager.endCutsceneEvent?.AddListener(moveCutsceneBarsOut);
    }

    private void moveCutsceneBarsIn()
    {
        anim.SetBool("Transition", true);
    }
    private void moveCutsceneBarsOut()
    {
        anim.SetBool("Transition", false);
        
    }
}
