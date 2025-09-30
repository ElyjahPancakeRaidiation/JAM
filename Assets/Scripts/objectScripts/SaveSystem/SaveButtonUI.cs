using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonUI : SavePosition
{

    private RectTransform rectTransform;
    private Image image;
    private Vector2 sizeDelta;
    private Color opacity;
    [SerializeField] private string specificID;


    public override void StoreGameObjectVariables()
    {
        base.StoreGameObjectVariables();
        ID = specificID;
    }

    public override void OnStart()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        image = this.gameObject.GetComponent<Image>();
        base.OnStart();
    }

    public override void VaribalesToJSON()
    {
        Debug.Log("RUNNING??");
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        image = this.gameObject.GetComponent<Image>();
        base.VaribalesToJSON();
        sizeDelta = rectTransform.sizeDelta;
        opacity = image.color;
        dataObj.Add("Size Delta", sizeDelta);
        dataObj.Add("Opacity", opacity);
    }

    public override void JSONToVariables(JSONObject data)
    {
        Debug.Log("Json to variable runs");
        position = data["Position"];
        sizeDelta = data["Size Delta"];
        opacity = data["Opacity"];
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
    public Color GetColor()
    {
        if (opacity == new Color(0, 0, 0, 0))
        {
            OnStart();
            if (opacity == new Color(0, 0, 0, 0))
            {
                //This extra check is when there are no saved files. If this wasnt here than it would just return nothing do some crazy number like 1.23423e23 in the file.
                opacity = image.color;
            }
            return opacity;
        }
        return opacity;
    }

    void OnDestroy()
    {
        RunWhenDestroyed();
    }

    public void ChangeSizeDelta(Vector2 val)
    {
        sizeDelta = val;
    }
    public void ChangePosition(Vector2 val)
    {
        // gameObject.transform.position = val;
        position = val;
    }
    public void ChangeOpacity(Color color)
    {
        opacity = color;
    }
}
