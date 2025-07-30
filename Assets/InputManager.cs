using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private float horizontalInput;

    public bool canControl=true;//Might have to change this later KEEP IN MIND 

    public class MainTouch
    {
        public float fingerID;
        public Vector2 origin;
        public Vector2 touchPos;
        public void setOrigin(Vector2 origin)
        {
            this.origin = origin;
        }
        public void setFingerID(float fingerID)
        {
            this.fingerID = fingerID;
        }
        public float getXDistance()
        {
            return touchPos.x - origin.x;
        }
    }
    public MainTouch mainTouch;
    #region Mobile Settings
    [Header("Mobile Settings")]
    public Vector2 screenSize;
    [SerializeField] public float inputRange; //i think this is in pixels idk bru, how far player needs to drag
    [SerializeField] public float inputDetectionPercentX; //this is percentage of screen that can be used for player input
    [SerializeField] private bool visualizeTouchArea;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        canControl = true;
        screenSize = new Vector2(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        if(canControl){mobileInput();}
#else
        if (canControl) { horizontalInput = Input.GetAxisRaw("Horizontal"); }
#endif

        if (!canControl) { horizontalInput = 0; }
    }
    
    private void mobileInput()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {    //if maintouch not initialized yet
                if (touch.phase == TouchPhase.Began && mainTouch == null && touch.position.x < screenSize.x * inputDetectionPercentX)
                {
                    mainTouch = new MainTouch();
                    mainTouch.setOrigin(touch.position);
                    mainTouch.setFingerID(touch.fingerId);
                    Debug.Log("Touch started: " + touch.fingerId);
                }
            }   //if maintouch is initialized, update its position
            if (mainTouch != null)
            {
                updateMainTouch();
            }
            else
            {
                horizontalInput = 0;
            }
        }
        else
        {
            horizontalInput = 0;
        }
    }
    private void updateMainTouch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == mainTouch.fingerID)
            {
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    mainTouch = null;
                }
                else
                {
                    mainTouch.touchPos = touch.position;
                    horizontalInput = Mathf.Clamp(mainTouch.getXDistance() / inputRange, -1, 1); //screenSize.x * inputRange is the max distance the player can move their finger to get the max input of 1
                }
            }
        }
    }
    private void OnGUI()
    {
        if (visualizeTouchArea)
        {
            GUI.color = new Color(0, 0, 0, 0.1f);
            GUI.DrawTexture(new Rect(0, 0, screenSize.x * inputDetectionPercentX, screenSize.y), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }

    public float getHorizontalInput() { return horizontalInput; }
    

}
