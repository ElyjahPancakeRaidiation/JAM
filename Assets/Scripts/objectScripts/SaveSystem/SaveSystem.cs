using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem current;
    string dataPath;
    // string nameDataPath;
    SaveSettings savedSettings;
    // weird weird;
    [SerializeField] private bool dontSaveCurScene;

    private void SearchForFile(string path)
    {

        if (!File.Exists(path))
        {
            File.CreateText(path);
            savedSettings = new SaveSettings();
        }
        else
        {
            FileInfo info = new FileInfo(dataPath);
            string savedJson = File.ReadAllText(path);
            if (info.Length != 0)
            {
                savedSettings = JsonUtility.FromJson<SaveSettings>(savedJson);
            }
            else { savedSettings = new SaveSettings(); }
        }
    }

    void Awake()
    {
        current = this;
        //Read from a text file that will contain the java data
        //If it is null make a new object of SaveSettings
        //If not than retrieve the data from the text in the file.
        dataPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveSettings.json";
        // nameDataPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Names.json";
        //For windows datapath it should be in Appdata/localLow/defualtcompany/JustAMind
        SearchForFile(dataPath);
    }


    private void Start()
    {
        // savedSettings = new SaveSettings();
        // SearchForFile(dataPath);
        GameManager.current.gameClose += SaveGame;
        if (GameManager.current != null) { GameManager.current.completedGame = savedSettings.completedGame; }
        if (SceneManager.GetActiveScene().name == "EndScreen")
        {
            savedSettings.completedGame = true;
        }
    }


    public void SaveGame()
    {
        if (!dontSaveCurScene)
        {
            //Converts the data into Json format to write to the text file.
            string contents = JsonUtility.ToJson(savedSettings, true);
            File.WriteAllText(dataPath, contents);
        }
    }

    public void ResetGame()
    {

        savedSettings.completedGame = false;
        SaveGame();
        GameManager.current.completedGame = savedSettings.completedGame;

    }

    void OnDestroy()
    {
        SaveGame();
    }

    public bool GetCompletedGame()
    {
        if (savedSettings != null)
        {
            return savedSettings.completedGame;
        }
        return false;
    }
}

[Serializable]
public class SaveSettings
{
    public bool completedGame;
    public SaveSettings()
    {
        completedGame = false;
    }
}
