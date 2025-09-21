using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;
using System;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using System.Runtime.CompilerServices;

[Serializable]
public class SaveData : MonoBehaviour
{
    [SerializeField] private string saveFileName = "UntitledData";
    protected JSONObject dataObj;
    protected int saveManagerListID;

    public class SaveDataVariables
    {
        public string ID;
        public string objectName;
        public SaveDataVariables()
        {

        }
    }
    protected SaveDataVariables saveDataVariables;

    void Start()
    {
        InitializeSaveDataVariables();
        PublicStartMethod();
    }

    void Update()
    {
        StoreGameObjectVariables();
    }

    public virtual void InitializeSaveDataVariables()
    {
        saveDataVariables = new SaveDataVariables();
    }

    public virtual void PublicStartMethod()
    {
        if (SaveManager.saveManager != null)
        {
            SaveManager.saveManager.pushDataToSave += PushData;
            saveFileName = SaveManager.saveManager.curDataFolder + Path.AltDirectorySeparatorChar + saveFileName + ".json";
            dataObj = new JSONObject();
            var o = SaveManager.saveManager.PullData(saveFileName, gameObject.GetInstanceID().ToString());
            if (o != null)
            {
                if (o.ToString() != "{}")//{} is null for javascript so if this is empty than it'll create a new JSONObject
                {
                    dataObj = o;
                    PullData(dataObj);
                }
            }
        }
    }

    public virtual void StoreGameObjectVariables()
    {
        saveDataVariables.ID = gameObject.GetInstanceID().ToString();
        saveDataVariables.objectName = gameObject.name;

    }

    public void PushData(Dictionary<string, JSONObject> jsonDic)
    {
        VaribalesToJSON();
        if (!jsonDic.ContainsKey(saveFileName))
        {
            JSONObject jObj = new JSONObject();
            jObj.Add(saveDataVariables.ID, dataObj);
            jsonDic.Add(saveFileName, jObj);
        }
        else
        {
            JSONObject j = jsonDic[saveFileName];
            if (j.HasKey(saveDataVariables.ID))//Prevents duplicates from apperaing in the files
            {
                jsonDic[saveFileName].AsObject[saveDataVariables.ID] = dataObj;
            }
            else
            {
                jsonDic[saveFileName].Add(saveDataVariables.ID, dataObj);
            }
        }
    }

    public virtual void VaribalesToJSON()
    {
        //This method will be used to transfer all of the variables you want to the JSONObject
        dataObj.Add("Object Name", saveDataVariables.objectName);
    }

    //Used to load from the JSONObject to the variables.
    public virtual void PullData(JSONObject data) { }

}
