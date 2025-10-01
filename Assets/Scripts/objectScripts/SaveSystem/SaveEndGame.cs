using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveEndGame : SaveData
{
    [SerializeField] private GameManager gm;
    public bool completedGame;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "EndScreen")
        {
            completedGame = true;
            Debug.Log(completedGame);
        }

        if (assignedSaveManager == null)
        {
            Debug.LogError("Save Manager is not assigned");
        }
        else
        {
            ID = "CompletedGameObj";
            PublicStartMethod(ID);
            assignedSaveManager.SaveDataToFile();

        }

        if (GameManager.current != null)
        {
            gm.completedGame = completedGame;
        }
    }

    public override void VaribalesToJSON()
    {
        dataObj.Add("Completed Game", completedGame);
    }

    public override void JSONToVariables(JSONObject data)
    {
        completedGame = data["Completed Game"];
    }

    void OnEnable()
    {
        ID = "CompletedGameObj";
        PublicStartMethod(ID);
    }

    void OnDestroy()
    {
        assignedSaveManager.pushDataToSave -= PushData;
    }
}
