using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileSkipButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] SkipButtonManager assignedButton;
    
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        assignedButton.isTouchingButton = true;
    }
}
