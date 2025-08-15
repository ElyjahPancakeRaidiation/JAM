using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkipButtonManager : MonoBehaviour
{
    // [SerializeField]private Animation anim;
    // private bool isVisible;//Checks if the sprite is visible

    // // Start is called before the first frame update
    // void Start()
    // {
    //     anim = GetComponent<Animation>();
    // }

    // private void OnGUI() {
    //     Event e = Event.current;
    //     if (e.isKey && !isVisible)
    //     {
    //         anim.Play();
    //         isVisible = true;
    //     }
    // }

    [SerializeField] private Slider buttonSlider;
    [SerializeField] private float maxSkipTime;
    [SerializeField] private float skipTimeMultiplierInc;
    [SerializeField] private float skipTimeMultiplierDec;
    [SerializeField] private UnityEvent skippedButtonEvents;
    [SerializeField] private KeyCode skipButtonKey;

    private Animation anim;
    private Coroutine increaseSliderCoroutine;
    private Coroutine decreaseSliderCoroutine;
    public bool isTouchingButton{ get; set; }

    private void Start()
    {
        if (buttonSlider != null)
        {
            buttonSlider.maxValue = maxSkipTime;
            buttonSlider.value = 0;
        }
        else
        {
            Debug.LogError("The variable Button Slider is not assigned.");
        }

        anim = GetComponent<Animation>();
        if (SaveSystem.current.GetCompletedGame())
        {
            anim.Play();
        }
    }

    private void Update()
    {
        //Although it isn't good to have these both in the update method I didn't want to go through the hassal of  gettng the build version
        //The previous version Felix used for the input manager wasn't working for me for some reason.
        //Plus(Another excuse for bad programming...)The method will always stop(For mobile) before getting to the heavy portions because of the bool isTouchingButton
        if (SaveSystem.current.GetCompletedGame())
        {
            MobileSkip();
            PCSkip();
        }
    }

    private void MobileSkip()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (isTouchingButton)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        StartIncreasingSlider();
                        break;
                    case TouchPhase.Ended:
                        StartDecreasingSlider();
                        break;
                }
            }
        }
    }

    private void PCSkip()
    {
        if (Input.GetKey(skipButtonKey))
        {
            StartIncreasingSlider();
        }

        if (Input.GetKeyUp(skipButtonKey))
        {
            StartDecreasingSlider();
        }
    }


    public void StartIncreasingSlider()
    {
        if (decreaseSliderCoroutine != null)
        {
            StopCoroutine(decreaseSliderCoroutine);
            decreaseSliderCoroutine = null;
        }
        if (increaseSliderCoroutine == null)
        {
            increaseSliderCoroutine = StartCoroutine(IncreaseSlider());
        }
    }
    public void StartDecreasingSlider()
    {
        if (increaseSliderCoroutine != null)
        {
            StopCoroutine(increaseSliderCoroutine);
            increaseSliderCoroutine = null;
        }
        if (decreaseSliderCoroutine == null)
        {
            decreaseSliderCoroutine = StartCoroutine(DecreaseSlider());
        }
    }

    private IEnumerator IncreaseSlider()
    {

        while (buttonSlider.value < maxSkipTime)
        {
            buttonSlider.value += Time.deltaTime * skipTimeMultiplierInc;
            yield return null;
        }

        if (buttonSlider.value >= maxSkipTime)
        {
            skippedButtonEvents.Invoke();
        }
    }

    private IEnumerator DecreaseSlider()
    {
        while (buttonSlider.value > 0)
        {
            buttonSlider.value -= Time.deltaTime * skipTimeMultiplierDec;
            yield return null;
        }

        if (buttonSlider.value <= 0)
        {
            buttonSlider.value = 0;
        }
    }


}
