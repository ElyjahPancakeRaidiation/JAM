using System;
using System.Collections.Generic;
using System.IO;
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

    void Awake()
    {
        current = this;
        //Read from a text file that will contain the java data
        //If it is null make a new object of SaveSettings
        //If not than retrieve the data from the text in the file.
        dataPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveSettings.json";
        // nameDataPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "Names.json";
        //For windows datapath it should be in Appdata/localLow/defualtcompany/JustAMind
        if (!File.Exists(dataPath))
        {
            File.CreateText(dataPath);
            savedSettings = new SaveSettings();
        }
        else
        {
            string savedJson = File.ReadAllText(dataPath);
            savedSettings = JsonUtility.FromJson<SaveSettings>(savedJson);
        }

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

        GameManager.current.gameClose += SaveGame;
        GameManager.current.completedGame = savedSettings.completedGame;
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
