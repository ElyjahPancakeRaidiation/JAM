using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class GameManager : MonoBehaviour
{
    #region events
    public static GameManager current;
    public event Action gameClose;
    public event Action pauseEvent;
    public event Action unPauseEvent;
    #endregion

    public bool completedGame { get; set; }

    #region GameManager settings
    [Header("GameManager Settings")]
    public KeyCode pauseKey;
    //When the player triggers the GM scene changer it'll base it off of this variable. Look at the build index
    public int sceneNum;
    [SerializeField] private bool hasSpawnPosition;
    //controls wheater or not the scene will start with the beginning transition
    [SerializeField] private bool onStartTransition = true;
    [SerializeField] private bool canPause = true;
    [SerializeField] private Transform playerSpawnPosition;

    #endregion


    #region build variables
    public enum Build { Mobile, PC }
    [SerializeField] private Build build;
    public Build GetBuildVer(){ return build; }
    private GameObject[] MobileObjects;
    private GameObject[] PCObjects;
    #endregion


    public bool isPaused { get; set; }
    private GameObject player;
    private CanvasScript allCanvasObj;


    private void Awake()
    {
        current = this;

        pauseEvent = null;
        unPauseEvent = null;
        MobileObjects = GameObject.FindGameObjectsWithTag("MobileObj");
        PCObjects = GameObject.FindGameObjectsWithTag("PCObj");
        //turns on all objects that are not apart of the builds versions
        switch (build)
        {
            case Build.Mobile:
                foreach (GameObject obj in MobileObjects)
                {
                    obj.SetActive(true);
                }
                foreach (GameObject obj in PCObjects)
                {
                    obj.SetActive(false);
                }
                break;
            case Build.PC:
                foreach (GameObject obj in MobileObjects)
                {
                    obj.SetActive(false);
                }
                foreach (GameObject obj in PCObjects)
                {
                    obj.SetActive(true);
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        allCanvasObj = GameObject.FindGameObjectWithTag("AllCanvas")?.GetComponent<CanvasScript>();
        if (allCanvasObj == null)
        {
            Debug.LogError("Yo you missing all canvases bro go into the prefab and get it. Thank you pookie wookie");
        }
        else
        {
            if (!onStartTransition)
            {
                allCanvasObj._transitionsAnim.SetBool("IdleOnStart", true);
            }
        }

        if (hasSpawnPosition)
        {
            Camera.main.transform.position = playerSpawnPosition.position;
            player.transform.position = playerSpawnPosition.position;
        }

        isPaused = false;
    }

    void Update()
    {
        if (canPause)
        {
            if (Input.GetKeyDown(pauseKey))
            {
                isPaused = !isPaused;
            }
        }

        if (isPaused)
        {
            if (Time.timeScale == 1)
            {
                if (pauseEvent != null) { pauseEvent(); }
                Time.timeScale = 0;
            }
        }
        else
        {
            if (Time.timeScale == 0)
            {
                if (unPauseEvent != null) { unPauseEvent(); }
                Time.timeScale = 1;
            }
        }
    }

    public void CloseGame()
    {
        if (gameClose != null) { gameClose(); }
        Application.Quit();
    }

    public void ResetLevel()
    {
        isPaused = false;
        sceneNum = SceneManager.GetActiveScene().buildIndex;
        transitionSceneAnimation(sceneNum);
    }
    public void ExitScene()
    {
        isPaused = false;
        StartCoroutine(ExitSceneEnum());
    }

    public void ChangeSceneInstant(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void transitionSceneAnimation(int sceneNum)
    {
        StartCoroutine(TransitionChangeScene(sceneNum));
    }

    //Both enumerators waits until their transition animation ends before switching scenes
    private IEnumerator TransitionChangeScene(int sceneNum)
    {
        if (allCanvasObj != null)
        {
            allCanvasObj._transitionsAnim.SetBool("IdleOnStart", false);
            allCanvasObj._transitionsAnim.SetTrigger("SceneTransition");
            yield return new WaitForSecondsRealtime(allCanvasObj.sceneTransitionEndClip.length);
        }
        ChangeSceneInstant(sceneNum);
    }

    private IEnumerator ExitSceneEnum()
    {
        if (allCanvasObj != null)
        {
            allCanvasObj._transitionsAnim.SetBool("IdleOnStart", false);
            allCanvasObj._transitionsAnim.SetTrigger("MainMenuTransition");
            yield return new WaitForSecondsRealtime(allCanvasObj.mainMenuTransitionClip.length);
        }
        ChangeSceneInstant(0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transitionSceneAnimation(sceneNum);
        }
    }
}
