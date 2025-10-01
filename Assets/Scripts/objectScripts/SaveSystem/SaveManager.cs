using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;
using CustomFileFunc;
using Newtonsoft.Json;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager saveManager;
    [SerializeField] private string dataFolderName;//For the specific scenes folder name.
    public const string ALLDATAFOLDER = "AllSavedData";
    public const string STOREDDATALOCATIONS = "StoredDataLocations";//This will hold all of the locations for the saved data of each folder.

    private string allDataFolder;
    public string curDataFolder { get; private set; }

    private SaveDataPaths sceneData;
    public Dictionary<string, JSONObject> sceneSavedData = new Dictionary<string, JSONObject>();

    public delegate void PushDataToSave(Dictionary<string, JSONObject> j);
    public PushDataToSave pushDataToSave;

    [SerializeField] private bool autoSave;
    [SerializeField] private float autoSaveTime = 10;
    [SerializeField] private bool saveOnce = false;
    [SerializeField] private int counter;
    private Coroutine AutoSaveCoro;
    private bool saving = false;
    private bool loaded = false;

    private void SetUpSceneData()
    {
        if (!loaded)
        {
            loaded = true;
            sceneData = new SaveDataPaths();
            CustomFuncs.CreateFolder(curDataFolder);//If it doesnt exist create a folder with the name that is assigned in dataFolderName
            sceneData.UpdateSavedDataPaths(curDataFolder);//Gets all the json files that are in the curDataFolder
            LoadDataAtStart();

        }
    }

    private void LoadDataAtStart()
    {
        if (sceneData.savedDataPaths.Length > 0)
        {
            for (int i = 0; i < sceneData.savedDataPaths.Length; i++)
            {
                string fileName = sceneData.savedDataPaths[i].FullName.Replace(@"\", "/");
                if (File.Exists(fileName))
                {
                    string jsonData = File.ReadAllText(fileName);
                    if (jsonData != "")
                    {
                        JSONObject j = (JSONObject)JSON.Parse(jsonData);
                        sceneSavedData.Add(fileName, j);
                    }
                }
                else
                {
                    Debug.Log("Error: File location was either deleted or corrupted.");
                }
            }
        }

        foreach (KeyValuePair<string, JSONObject> i in sceneSavedData)
        {
            Debug.Log(i.Key);
        }
    }

    private void Awake()
    {
        saveManager = this;
        allDataFolder = CustomFuncs.FindFileLocation(ALLDATAFOLDER);
        curDataFolder = CustomFuncs.FindFileLocation(dataFolderName, ALLDATAFOLDER);//Location of the specified scenes data.
        Debug.Log(curDataFolder);
        ///Checks if we have ever saved before. If we haven't saved before it'll create a folder.
        ///This folder will hold the folders of the scenes data 

        CustomFuncs.CreateFolder(allDataFolder);//The method checks if the folder exists already so there is no overwriting data.
        SetUpSceneData();
    }

    void Start()
    {
        if (counter == 0)
        {
            if (autoSave && AutoSaveCoro == null)
            {
                if (autoSave)
                {
                    counter++;
                }
                AutoSaveCoro = StartCoroutine(AutoSave());
            }
        }
        AddCommands();
    }

    void OnEnable()
    {
        AddCommands();
    }

    void Update()
    {

        if (counter == 0)
        {
            if (autoSave && AutoSaveCoro == null)
            {
                if (autoSave)
                {
                    counter++;
                }
                AutoSaveCoro = StartCoroutine(AutoSave());
            }
        }
    }

    private void AddCommands()
    {
        if (ConsoleScript.consoleScript != null)
        {
            ConsoleScript.consoleScript.AddCommand("Delete", gameObject.name, DeleteFile);
            ConsoleScript.consoleScript.AddCommand("Reset", gameObject.name, ResetFile);
        }
    }

    public void DeleteFile(string[] cmd)
    {
        string fileName = curDataFolder + Path.AltDirectorySeparatorChar + cmd[2] + ".json";
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        else
        {
            Debug.LogError("File was not found in " + dataFolderName);
        }
    }

    public void ResetFile(string[] cmd)
    {
        string fileName = curDataFolder + Path.AltDirectorySeparatorChar + cmd[2] + ".json";
        if (File.Exists(fileName))
        {
            File.WriteAllText(fileName, "");
        }
        else
        {
            Debug.LogError("File was not found in " + dataFolderName + " Folder");
        }
    }


    public void SaveDataToFile()
    {
        if (!saving)
        {
            saving = true;
            if (pushDataToSave != null)
            {
                pushDataToSave(sceneSavedData);
            }
            foreach (KeyValuePair<string, JSONObject> item in sceneSavedData)
            {
                CustomFuncs.CreateFile(item.Key);
                File.WriteAllText(item.Key, sceneSavedData[item.Key].ToString(4));
            }
            saving = false;
        }
    }

    public JSONObject PullData(string filePath, string ID)
    {
        SetUpSceneData();
        if (sceneSavedData.ContainsKey(filePath))
        {
            JSONObject j = sceneSavedData[filePath].AsObject;
            return j[ID].AsObject;
        }
        return null;
    }

    public string getCurDataFolder()
    {
        if (curDataFolder == null)
        {
            curDataFolder = CustomFuncs.FindFileLocation(dataFolderName, ALLDATAFOLDER);//Location of the specified scenes data.
            return curDataFolder;
        }
        return curDataFolder;
    }

    private IEnumerator AutoSave()
    {
        while (autoSave && counter == 0)
        {
            if (!saving)
            {
                yield return new WaitForSecondsRealtime(autoSaveTime);
                Debug.Log("ran");
                SaveDataToFile();
            }
            yield return null;
        }
    }

}


public class SaveDataPaths
{
    public FileInfo[] savedDataPaths;

    public void UpdateSavedDataPaths(string filePath)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(filePath);
        savedDataPaths = dirInfo.GetFiles();
    }
}

