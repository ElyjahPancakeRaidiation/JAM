using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public bool completedGame { get; set; }

    public event Action gameClose;


    public KeyCode playerAbilityKey;
    public int sceneNum{ get; set; }

    [SerializeField] private bool hasSpawnPosition;
    [SerializeField] private Transform playerSpawnPosition;
    private GameObject player;

    private enum Build { Mobile, PC }
    [SerializeField] private Build build;
    private GameObject[] MobileObjects;
    private GameObject[] PCObjects;

    [SerializeField] private AnimationClip clipToPlay;

    private void Awake()
    {
        current = this;
        MobileObjects = GameObject.FindGameObjectsWithTag("MobileObj");
        PCObjects = GameObject.FindGameObjectsWithTag("PCObj");
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
        
        if (hasSpawnPosition)
        {
            Camera.main.transform.position = playerSpawnPosition.position;
            player.transform.position = playerSpawnPosition.position;
        }


    }

    public void closeGame()
    {
        if(gameClose != null){gameClose();}
        Application.Quit();
    }


    public void changeScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void changeSceneDelay()
    {
        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSecondsRealtime(clipToPlay.length);
        changeScene(sceneNum);
    }
}
