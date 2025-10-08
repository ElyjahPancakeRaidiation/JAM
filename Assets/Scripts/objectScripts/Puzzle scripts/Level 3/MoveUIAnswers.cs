using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveUIAnswers : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isClicked;
    public bool canClick = true;
    
    private void Update()
    {
        if (!canClick) isClicked = false;
        if (isClicked)
        {
            // transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (canClick)
        {
            // mouse.WarpCursorPosition(Camera.main.ScreenToWorldPoint(this.gameObject.transform.position));
            isClicked = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicked = false;
    }
}
