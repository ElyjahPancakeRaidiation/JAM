using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class TestManager : MonoBehaviour
{
    public static event Action pauseEvent;
    public static event Action unPauseEvent;
    
    
    [SerializeField] private int nextSceneNum;

    [Header("Pause Menu")]
    [SerializeField]private Animator transitionAnim;
    [SerializeField]private AnimationClip start, end;
    // public static bool transitioned;
    private GameObject buttonCotainer;
    private GameObject pauseMenu;
    [SerializeField]private GameObject exitBallTransition;//exit ball transition relates to the games exit animation.
    //Also exit ball object starts off inactive making us have to store it manually in the inspector. Sucks ass.
    public static bool isPaused;
    [SerializeField]private AnimationClip mainMenuTransition;

    [Header("Player")]
    public bool isMobileControls;
    private PlayerController playerController;
    private Transform player;
    private GameObject mobileControlPanel;

    [Header("Level 3 Respawn")]
    [SerializeField]private Animator respawnAnim;
    [SerializeField]private AnimationClip respawnStart, respawnEnd;

    // Start is called before the first frame update


    private void Awake()
    {
        pauseEvent = null;
        unPauseEvent = null;
    }
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        pauseMenu = GameObject.Find("PauseCanvas");
        mobileControlPanel = GameObject.Find("MobileLayout") ?? null;


        playerController = player.GetComponent<PlayerController>();
        if (respawnAnim != null) { respawnAnim.gameObject.SetActive(false); }
        exitBallTransition.SetActive(false);

        if (mobileControlPanel != null)
        {
            if (isMobileControls)
            {
                mobileControlPanel.SetActive(true);
            }
            else
            {
                mobileControlPanel.SetActive(false);
            }
        }

        isPaused = false;

    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            Time.timeScale = 0;
            if(pauseEvent != null){pauseEvent();}
            pauseMenu.SetActive(true);
        }else{
            Time.timeScale = 1;
            if(unPauseEvent != null){unPauseEvent();}
            pauseMenu.SetActive(false);
        }
		
    }

    //Gamemanger also acts as a scene transitioner for the player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { StartCoroutine(Transition(nextSceneNum)); }
    }



    private IEnumerator Transition(int scene)
    {
        transitionAnim.SetTrigger("Transition");
        // transitioned = true;
        yield return new WaitForSeconds(start.length);
        SceneManager.LoadScene(scene);
        // transitioned = false;
    }

    public IEnumerator RespawnLevel3(){
        respawnAnim.gameObject.SetActive(true);
        yield return new WaitForSeconds(respawnStart.length);
        playerController.canMove = false;
        player.position = playerController.spawner.transform.position;
        yield return new WaitForSeconds(0.6f);
        respawnAnim.SetTrigger("RespawnEnd");
        yield return new WaitForSeconds(respawnEnd.length);
        respawnAnim.ResetTrigger("RespawnEnd");
        respawnAnim.gameObject.SetActive(false);

    }

    private IEnumerator ExitTransition(){
        //exit = false;
        exitBallTransition.SetActive(true);
        // transitioned = true;
        yield return new WaitForSeconds(mainMenuTransition.length);
        SceneManager.LoadScene(0);
        // transitioned = false;
    }

    //Buttons for the main menu
    public void Exit(){
        isPaused = false;
        StartCoroutine(ExitTransition());
    }

    public void Restart(){
        isPaused = false;
        StartCoroutine(Transition(SceneManager.GetActiveScene().buildIndex));
    }

    public void setIsPaused(bool val){isPaused = val;}

    //For button config it can be changed to just use the button function in unity instead of functions here....

    // public void ButtonConfig() 
    // {
    //     isPaused = false;
	// 	if (!buttonConfig)
	// 	{
    //         buttonConfig = true;
	// 	}
    // }

   /* public void MusicTriggerEnd()
    {
        if (pc.musicHasChangedOne)
		{
            musicChanger.SetActive(false);
		}
		if (pc.musicHasChangedTwo)
		{
           Destroy(musicChangerTwo);
		}
    }*/
   /* public void ButtonExit() 
    {
        buttonConfig = true;
		if (buttonConfig)
		{
            buttonCotainer.SetActive(false);
		}
    }*/
    
    
    
}
