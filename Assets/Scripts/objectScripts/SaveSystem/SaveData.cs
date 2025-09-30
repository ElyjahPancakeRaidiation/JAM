using UnityEngine;
using SimpleJSON;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public class SaveData : MonoBehaviour
{
    [SerializeField] private string saveFileName = "UntitledData";
    protected JSONObject dataObj;
    [SerializeField] protected SaveManager assignedSaveManager;
    private bool setUpComplete = false;//This checks if PublicStartMethod was ran.

    #region Variables To Save
    [SerializeField] protected string ID;

    #endregion

    void OnEnable()
    {
        if (assignedSaveManager == null)
        {
            Debug.LogError("Save Manager is not assigned");
        }
        StoreGameObjectVariables();
        PublicStartMethod(ID);
    }

    void Update()
    {
        StoreGameObjectVariables();
    }

    public virtual void PublicStartMethod(string ID)
    {
        if (!setUpComplete && assignedSaveManager != null)
        {
            assignedSaveManager.pushDataToSave += PushData;
            saveFileName = assignedSaveManager.getCurDataFolder() + Path.AltDirectorySeparatorChar + saveFileName + ".json";
            dataObj = new JSONObject();
            var o = assignedSaveManager.PullData(saveFileName, ID);
            if (o != null)
            {
                if (o.ToString() != "{}")//{} is null for javascript so if this is empty than it'll create a new JSONObject
                {
                    dataObj = o;
                    JSONToVariables(dataObj);
                }
            }
            else
            {
                Debug.Log("Data null");
            }
            setUpComplete = true;
        }
    }

    public virtual void StoreGameObjectVariables()
    {
        ID = gameObject.name;

    }

    public void PushData(Dictionary<string, JSONObject> jsonDic)
    {
        VaribalesToJSON();
        if (!jsonDic.ContainsKey(saveFileName))
        {
            JSONObject jObj = new JSONObject();
            jObj.Add(ID, dataObj);
            jsonDic.Add(saveFileName, jObj);
        }
        else
        {
            JSONObject j = jsonDic[saveFileName];
            if (j.HasKey(ID))//Prevents duplicates from apperaing in the files
            {
                jsonDic[saveFileName].AsObject[ID] = dataObj;
            }
            else
            {
                jsonDic[saveFileName].Add(ID, dataObj);
            }
        }
    }

    public virtual void VaribalesToJSON(){}

    //Used to load from the JSONObject to the variables.
    public virtual void JSONToVariables(JSONObject data) { }


    public SaveManager GetAssignedManager()
    {
        return assignedSaveManager;
    }

    public void RunWhenDestroyed()
    {
        if(assignedSaveManager!=null){assignedSaveManager.pushDataToSave -= PushData;}
    }

    void OnDestroy()
    {
        RunWhenDestroyed();
    }
}
