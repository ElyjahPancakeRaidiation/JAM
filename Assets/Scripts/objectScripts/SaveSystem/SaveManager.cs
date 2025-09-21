using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;
using CustomFileFunc;
using UnityEditor.SearchService;
using UnityEditor.ShaderGraph.Serialization;
using Newtonsoft.Json;
using UnityEngine.InputSystem;
using System.Collections;

public class SaveManager : MonoBehaviour
{
    public static SaveManager saveManager;
    [SerializeField] private string dataFolderName;//For the specific scenes folder name.
    public const string ALLDATAFOLDER = "AllSavedData";
    public const string STOREDDATALOCATIONS = "StoredDataLocations";//This will hold all of the locations for the saved data of each folder.
    private const string SCENESTOREDATALOCATIONS = "SceneStoredDataPaths";

    private string allDataFolder;
    public string curDataFolder { get; private set; }

    private SaveDataPaths sceneData;

    public Dictionary<string, JSONObject> sceneSavedData = new Dictionary<string, JSONObject>();
    public List<GameObject> allSavedObjects = new List<GameObject>();

    public delegate void PushDataToSave(Dictionary<string, JSONObject> j);
    public PushDataToSave pushDataToSave;

    [SerializeField] private bool autoSave;
    [SerializeField] private float autoSaveTime;
    private Coroutine AutoSaveCoro;
    private bool saving = false;

    private void SetUpSceneData()
    {
        CustomFuncs.CreateFolder(curDataFolder);
        sceneData = new SaveDataPaths();
        sceneData.UpdateSavedDataPaths(curDataFolder);
        LoadDataAtStart();
    }

    private void LoadDataAtStart()
    {
        for (int i = 0; i < sceneData.savedDataPaths.Length; i++)
        {
            string fileName = sceneData.savedDataPaths[i].FullName.Replace(@"\", "/");
            if (File.Exists(fileName))
            {
                string jsonData = File.ReadAllText(fileName);
                JSONObject j = (JSONObject)JSON.Parse(jsonData);
                sceneSavedData.Add(fileName, j);
            }
            else
            {
                Debug.Log("Error: File location was either deleted or corrupted.");
            }
        }
    }

    private void Awake()
    {
        saveManager = this;
        allDataFolder = CustomFuncs.FindFileLocation(ALLDATAFOLDER);
        curDataFolder = CustomFuncs.FindFileLocation(dataFolderName, ALLDATAFOLDER);//Location of the specified scenes data.
        ///Checks if we have ever saved before. If we haven't saved before it'll create a folder.
        ///This folder will hold the folders of the scenes data 

        CustomFuncs.CreateFolder(allDataFolder);//The method checks if the folder exists already so there is no overwriting data.
        SetUpSceneData();
    }

    void Start()
    {
        if (autoSave && AutoSaveCoro == null)
        {
            AutoSaveCoro = StartCoroutine(AutoSave());
        }
    }
    void Update()
    {

        if (autoSave && AutoSaveCoro == null)
        {
            AutoSaveCoro = StartCoroutine(AutoSave());
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
        if (sceneSavedData.ContainsKey(filePath))
        {
            JSONObject j = sceneSavedData[filePath].AsObject;
            return j[ID].AsObject;
        }
        return null;
    }

    private IEnumerator AutoSave()
    {
        while (autoSave)
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

