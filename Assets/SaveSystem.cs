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
            }else{ savedSettings = new SaveSettings(); }
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

        // if (!File.Exists(nameDataPath))
        // {
        //     File.CreateText(nameDataPath);
        //     weird = new weird();
        // }
        // else
        // {
        //     string savedJson = File.ReadAllText(nameDataPath);
        //     weird = JsonUtility.FromJson<weird>(savedJson);
        // }
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
        //Converts the data into Json format to write to the text file.
        string contents = JsonUtility.ToJson(savedSettings);
        File.WriteAllText(dataPath, contents);
    }

    public void ResetGame()
    {

        savedSettings.completedGame = false;
        SaveGame();
        GameManager.current.completedGame = savedSettings.completedGame;

    }
    // public void SaveName()
    // {
    //     string contents = JsonUtility.ToJson(weird);
    //     File.WriteAllText(nameDataPath, contents);
    // }
    // public void loadName()
    // {
    //     weird = JsonUtility.FromJson<weird>(nameDataPath);
    //     for (int i = 0; i < weird.saveSettings.Count; i++)
    //     {
    //         GameObject obj = GameObject.Find(weird.saveSettings[i].name);
    //         var test = obj.GetComponent<SaveSettingsV2>();

    //     }
    // }

    // public void saveSetting(SaveSettingsV2 s) => weird.addSetting(s);

    void OnDestroy()
    {
        SaveGame();
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

// [Serializable]
// public class SaveSettingsV2
// {
//     public string name;
//     public int id;
//     public string compName;
//     public SaveSettingsV2 other;
//     public SaveSettingsV2()
//     {
        
//     }
//     public void giveData(SaveSettingsV2 data){ other = data; }
// }

// public class weird
// {
//     public List<SaveSettingsV2> saveSettings;
//     public weird()
//     {
//         saveSettings = new List<SaveSettingsV2>();
//     }
//     public void addSetting(SaveSettingsV2 s)
//     {
//         saveSettings.Add(s);
//     }
// }
