using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using SimpleJSON;
using UnityEngine;

public class SaveButtonUI : SavePosition
{

    private RectTransform rectTransform;
    [SerializeField] private Vector2 sizeDelta;
    [SerializeField] private string specificID;


    public override void StoreGameObjectVariables()
    {
        base.StoreGameObjectVariables();
        ID = specificID;
    }

    public override void OnStart()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        base.OnStart();
    }

    public override void VaribalesToJSON()
    {
        Debug.Log("RUNNING??");
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        base.VaribalesToJSON();
        sizeDelta = rectTransform.sizeDelta;
        dataObj.Add("Size Delta", sizeDelta);
    }

    public override void JSONToVariables(JSONObject data)
    {
        Debug.Log("Json to variable runs");
        position = data["Position"];
        sizeDelta = data["Size Delta"];
    }

    public Vector2 GetPosition()
    {
        if (position == Vector2.zero)
        {
            OnStart();
            if (position == Vector2.zero)
            {
                position = this.gameObject.transform.position;
            }
            return position;
        }
        return position;
    }
    public Vector2 GetSizeDelta()
    {
        if (sizeDelta == Vector2.zero)
        {
            OnStart();
            if (sizeDelta == Vector2.zero)
            {
                //This extra check is when there are no saved files. If this wasnt here than it would just return nothing do some crazy number like 1.23423e23 in the file.
                sizeDelta = rectTransform.sizeDelta;
            }
            return sizeDelta;
        }
        return sizeDelta;
    }

    void OnDestroy()
    {
        RunWhenDestroyed();
    }

    public void ChangeSizeDelta(Vector2 val)
    {
        sizeDelta = val;
    }
}
