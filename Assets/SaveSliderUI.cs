using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class SaveSliderUI : SaveData
{

    [SerializeField] private Slider slider;
    // Start is called before the first frame update
    void OnEnable()
    {
        StoreGameObjectVariables();
        PublicStartMethod(ID);
    }

    public override void StoreGameObjectVariables()
    {
        base.StoreGameObjectVariables();
        ID = gameObject.name;
    }

    public override void JSONToVariables(JSONObject data)
    {
        base.JSONToVariables(data);
        slider.value = data["Slider Value"];
        ID = gameObject.name;
    }

    public override void VaribalesToJSON()
    {
        base.VaribalesToJSON();
        dataObj["Slider Value"] = slider.value;
    }

    void OnDestroy()
    {
        RunWhenDestroyed();
    }


}
