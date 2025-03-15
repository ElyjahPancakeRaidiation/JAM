using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileJoystick : MonoBehaviour
{
    public static MobileJoystick instance;
    private RectTransform joystickRect;
    private Vector2 originalPos;
    [SerializeField]private float maxDistance;
    private float input;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null){
            instance = GetComponent<MobileJoystick>();
        }
        joystickRect = GetComponent<RectTransform>();
        originalPos = joystickRect.position;
    }

    // Update is called once per frame
    void Update()
    {
        movingJoyStick();
    }

    private void movingJoyStick(){
        if(Input.touchCount > 0){
            float posX;//Contains the players x direction of their finger on the screen
            Touch touch = Input.GetTouch(0);//Stores the first finger touching the screen
            if(touch.position.x < 750){
                switch(touch.phase){
                    case TouchPhase.Began://The initial touch
                        posX = touch.position.x;
                        joystickRect.position = new Vector2(Mathf.Clamp(posX, originalPos.x - maxDistance, originalPos.x + maxDistance), originalPos.y);
                        input = touch.position.x - joystickRect.position.x;
                        break;
                    case TouchPhase.Moved:
                        posX = touch.position.x;
                        joystickRect.position = new Vector2(Mathf.Clamp(posX, originalPos.x - maxDistance, originalPos.x + maxDistance), originalPos.y);
                        input = touch.position.x - joystickRect.position.x;
                        break;
                    case TouchPhase.Ended:
                        joystickRect.position = originalPos;
                        input = 0;
                        break;
                }
            }


        }
    }

    public int mobileInput(){//The main function that gives the player the horizontal input
        if(input == 0){return 0;}
        return (input > 0) ? 1 : -1;
    }
}
