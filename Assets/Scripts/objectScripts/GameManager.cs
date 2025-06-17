using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public KeyCode playerAbilityKey;
    public int sceneNum;

    [SerializeField] private bool hasSpawnPosition;
    [SerializeField] private Transform playerSpawnPosition;
    private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (hasSpawnPosition) { player.transform.position = playerSpawnPosition.position; }
    }


    public void changeScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
}
