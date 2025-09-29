using SimpleJSON;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SavePosition : SaveData
{

    protected Vector2 position;
    private void OnEnable()
    {
        OnStart();
    }

    public virtual void OnStart()
    {
        if (assignedSaveManager == null)
        {
            Debug.LogError("Save Manager is not assigned");
        }
        StoreGameObjectVariables();
        PublicStartMethod(ID);
    }

    public override void StoreGameObjectVariables()
    {
        base.StoreGameObjectVariables();
    }

    public override void VaribalesToJSON()
    {
        base.VaribalesToJSON();
        position = gameObject.transform.position;
        dataObj.Add("Position", position);
    }

    public override void JSONToVariables(JSONObject data)
    {
        base.JSONToVariables(data);
        position = data["Position"];
        gameObject.transform.position = position;
    }

    
    private void OnDestroy()
    {
        RunWhenDestroyed();
    }



}
