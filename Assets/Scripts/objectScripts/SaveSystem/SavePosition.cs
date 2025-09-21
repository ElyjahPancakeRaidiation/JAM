using SimpleJSON;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SavePosition : SaveData
{

    [SerializeField] public float numbers;
    [SerializeField] public Vector2 position;

    public class SaveDataVariablesP : SaveDataVariables
    {
        public Vector2 position;
    }
    private SaveDataVariablesP saveDataVariablesP;
    public override void InitializeSaveDataVariables()
    {
        base.InitializeSaveDataVariables();
        saveDataVariablesP = new SaveDataVariablesP();
    }

    void Start()
    {
        InitializeSaveDataVariables();
        PublicStartMethod();
    }

    void Update()
    {
        StoreGameObjectVariables();
    }

    public override void StoreGameObjectVariables()
    {
        base.StoreGameObjectVariables();
        saveDataVariablesP.position = this.gameObject.transform.position;
    }

    public override void VaribalesToJSON()
    {
        base.VaribalesToJSON();
        dataObj.Add("Numbers", numbers);
        dataObj.Add("Position", saveDataVariablesP.position);
    }

    public override void PullData(JSONObject data)
    {
        base.PullData(data);
        numbers = data["Numbers"];
        position = data["Position"];
        gameObject.transform.position = position;
    }



}
