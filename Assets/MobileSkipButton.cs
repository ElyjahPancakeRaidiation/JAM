using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileSkipButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] SkipButtonManager assignedButton;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (GameManager.current.GetBuildVer() == GameManager.Build.Mobile) {
            assignedButton.isTouchingButton = true;
        }
    }
}
