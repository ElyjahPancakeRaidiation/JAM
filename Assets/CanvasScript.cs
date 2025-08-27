using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
    private GameObject pauseCanvas, blackBarCanvas, TransitionCanvas;

    private bool isThoughtBubbleFollow;

    private GameObject player;
    public Animator _transitionsAnim { get; set; }

    [SerializeField] public AnimationClip sceneTransitionEndClip;
    [SerializeField] public AnimationClip mainMenuTransitionClip;


    // Start is called before the first frame update
    void Start()
    {
        //Finds all of the objects according to their NAME(Except thought bubble).
        pauseCanvas = GameObject.Find("PauseCanvas") ?? null;
        blackBarCanvas = GameObject.Find("BlackBarCanvas") ?? null;
        TransitionCanvas = GameObject.Find("Transition") ?? null;
        _transitionsAnim = TransitionCanvas.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (pauseCanvas != null) { pauseCanvas.SetActive(false); }

        GameManager.current.pauseEvent += setActivePauseCanvas;
        GameManager.current.unPauseEvent += deactivateActivePauseCanvas;
    }

    private void setActivePauseCanvas()
    {
        if (pauseCanvas != null) { pauseCanvas.SetActive(true); }
    }
    private void deactivateActivePauseCanvas()
    {
        if (pauseCanvas != null) { pauseCanvas.SetActive(false); }
    }
    

    private void OnDestroy()
    {
        GameManager.current.pauseEvent -= setActivePauseCanvas;
        GameManager.current.unPauseEvent -= setActivePauseCanvas;
    }
}
