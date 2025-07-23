using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasScript : MonoBehaviour
{
    private GameObject pauseCanvas, blackBarCanvas, thoughtBubbleObj, TransitionCanvas;

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
        thoughtBubbleObj = GameObject.FindGameObjectWithTag("ThoughtBubble") ?? null;
        _transitionsAnim = TransitionCanvas.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (pauseCanvas != null) { pauseCanvas.SetActive(false); }
        if (thoughtBubbleObj != null) { thoughtBubbleObj.SetActive(false); }

        thoughtBubble.triggerThoughtBubble += SetActiveThoughtBubble;
        GameManager.current.pauseEvent += setActivePauseCanvas;
        GameManager.current.unPauseEvent += setActivePauseCanvas;
    }

    // Update is called once per frame
    void Update()
    {
        if (isThoughtBubbleFollow)
        {
            thoughtBubbleObj.transform.position = player.transform.position;
        }
    }

    private void setActivePauseCanvas()
    {
        if (pauseCanvas != null) { pauseCanvas.SetActive(!pauseCanvas.activeSelf); }
    }

    private void SetActiveThoughtBubble(bool isActive)
    {
        thoughtBubbleObj.SetActive(isActive);
        isThoughtBubbleFollow = isActive;
    }
    

    private void OnDestroy()
    {
        thoughtBubble.triggerThoughtBubble -= SetActiveThoughtBubble;
        GameManager.current.pauseEvent -= setActivePauseCanvas;
        GameManager.current.unPauseEvent -= setActivePauseCanvas;
    }
}
