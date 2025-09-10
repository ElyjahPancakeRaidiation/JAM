using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    [Serializable]
    class MobileButton
    {
        public string tag;
        public GameObject mainButton; //the original button in the mobile layout ui
        public GameObject editable; //the button that shows up in the editor mode
        public Button buttonComponent;
        public Vector2 RelativePosition { get; set; }
        public Vector2 DefaultPostion { get; set; }
        public Vector2 DefaultSize { get; set; }
        public MobileButton(GameObject mainButton, string tag, Button buttonComponent)
        {
            this.mainButton = mainButton;
            this.tag = tag;
            this.buttonComponent = buttonComponent;
            this.DefaultPostion = mainButton.GetComponent<RectTransform>().anchoredPosition;
        }
        public void UpdateButton()
        {
            var mainButtonRect = mainButton.GetComponent<RectTransform>();
            var editableRect = editable.GetComponent<RectTransform>();
            mainButtonRect.anchoredPosition = editableRect.anchoredPosition;
            mainButtonRect.sizeDelta = editableRect.sizeDelta;
        }
        public void ResetToDefault()
        {
            RelativePosition = DefaultPostion;
            var mainButtonRect = mainButton.GetComponent<RectTransform>();
            var editableRect = editable.GetComponent<RectTransform>();
            mainButtonRect.anchoredPosition = DefaultPostion;
            editableRect.anchoredPosition = DefaultPostion;
            mainButtonRect.sizeDelta = DefaultSize;
            editableRect.sizeDelta = DefaultSize;

        }
    }
    private GameObject pauseCanvas, blackBarCanvas, thoughtBubbleObj, TransitionCanvas;
    private GameObject reorganizeUI;
    private bool isThoughtBubbleFollow;

    private GameObject player;
    public Animator _transitionsAnim { get; set; }

    [SerializeField] public AnimationClip sceneTransitionEndClip;
    [SerializeField] public AnimationClip mainMenuTransitionClip;
    [SerializeField] private List<MobileButton> buttons;
    [SerializeField] private List<MobileButton> editButtons;
    [SerializeField] private Slider buttonScale;
    private RectTransform currentDraggedObject;
    private bool isEditMode;


    // Start is called before the first frame update
    void Start()
    {
        //Finds all of the objects according to their NAME(Except thought bubble).
        pauseCanvas = GameObject.Find("PauseCanvas") ?? null;
        blackBarCanvas = GameObject.Find("BlackBarCanvas") ?? null;
        TransitionCanvas = GameObject.Find("Transition") ?? null;
        _transitionsAnim = TransitionCanvas.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (pauseCanvas != null) { pauseCanvas.SetActive(false); }

        GameManager.current.pauseEvent += setActivePauseCanvas;
        GameManager.current.unPauseEvent += deactivateActivePauseCanvas;

        foreach (MobileButton button in buttons)
        {
            var buttonRect = button.mainButton.GetComponent<RectTransform>();
            button.DefaultPostion = buttonRect.anchoredPosition;
            button.DefaultSize = buttonRect.sizeDelta;
        }
        if (buttonScale) //if the slider is correctly passed to the script
        {
            buttonScale.onValueChanged.AddListener((v) =>
            {
                Debug.Log("hehehe");
                UpdateButtonScale(v);
            });
        }
        else
        {
            Debug.Log("missing slider reference in reorganize ui");
        }
    }

    private void UpdateButtonScale(float v)
    {
        foreach (MobileButton button in buttons)
        {
            button.editable.GetComponent<RectTransform>().sizeDelta = button.DefaultSize * v;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isThoughtBubbleFollow)
        {
            thoughtBubbleObj.transform.position = player.transform.position;
        }
        if (isEditMode)
        {
            if (Input.GetMouseButtonDown(0) && !currentDraggedObject)
            {
                CheckDraggable();
            }
            if (Input.GetMouseButtonDown(0) && currentDraggedObject)
            {
                StartCoroutine(DragButton());
            }
            if (Input.GetMouseButtonUp(0) && currentDraggedObject != null)
            {
                currentDraggedObject = null;
            }
        }
    }

    private IEnumerator DragButton()
    {
        while (currentDraggedObject)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
            reorganizeUI.GetComponent<RectTransform>(),
            Input.mousePosition,
            reorganizeUI.gameObject.GetComponentInParent<Canvas>().worldCamera,
            out pos);
            currentDraggedObject.anchoredPosition = pos;
            yield return null;
        }
    }

    private void CheckDraggable()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0)
        {
            GameObject hitObject = results[0].gameObject;
            int index = buttons.FindIndex(b => b.editable == hitObject);
            if (index != -1)
            {
                Debug.Log("we found smth");
                currentDraggedObject = hitObject.GetComponent<RectTransform>();
            }
            else
            {
                Debug.Log(hitObject);
            }
        }

    }

    private void setActivePauseCanvas()
    {
        if (pauseCanvas != null) { pauseCanvas.SetActive(true); }
    }
    private void deactivateActivePauseCanvas()
    {
        if (pauseCanvas != null) { pauseCanvas.SetActive(false); }
    }

    private void SetActiveThoughtBubble(bool isActive)
    {
        thoughtBubbleObj.SetActive(isActive);
        isThoughtBubbleFollow = isActive;
    }


    private void OnDestroy()
    {
        GameManager.current.pauseEvent -= setActivePauseCanvas;
        GameManager.current.unPauseEvent -= setActivePauseCanvas;
    }
    public void LoadButtonForOrganization()
    {
        isEditMode = true;
        Debug.Log(reorganizeUI);
        reorganizeUI = GameObject.FindGameObjectWithTag("Reorganize");
        if (reorganizeUI)
        {
            foreach (MobileButton button in buttons)
            {
                if (!button.editable) button.editable = Instantiate(button.mainButton, reorganizeUI.transform);
                button.editable.SetActive(true);
            }
        }
        else
        {
            Debug.Log("missing reorganize UI, go get it");
        }
    }
    public void ExitReorganize()
    {
        isEditMode = false;
        foreach (MobileButton button in buttons)
        {
            if (button.editable)
            {
                button.UpdateButton();
                button.editable.SetActive(false);
            }
            else
            {
                Debug.Log("remember to turn off the reorganize ui before starting");
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter);
    }
    public void ResetAllButtons()
    {
        buttonScale.value = 1;
        foreach (MobileButton button in buttons)
        {
            button.ResetToDefault();
        }
    }
}
