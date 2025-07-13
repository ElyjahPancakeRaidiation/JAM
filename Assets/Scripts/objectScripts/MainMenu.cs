using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject levelSelectorButton;


    private void Update()
    {
        if (GameManager.current.completedGame)
        {
            levelSelectorButton.SetActive(true);
            Debug.Log("On");
        }
        else
        {
            Debug.Log("off");

            levelSelectorButton.SetActive(false);
        }
    }

    
}
