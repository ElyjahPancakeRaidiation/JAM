using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrackKeyOrder : MonoBehaviour
{
    [SerializeField] private bool OnStart;
    [SerializeField] private bool isMobile;
    public bool completed;
    [SerializeField] private GameObject[] playerButtons;
    [SerializeField] private AllKeys[] keys;
    [SerializeField] private int keysIdx = 0;
    int amountPressed = 0;
    private bool isRunning = false;

    private void Start()
    {
        keysIdx = 0;

        if (OnStart)
        {
            StartTrackingKeys();
        }
    }

    public void StartTrackingKeys()
    {
        if (!completed)
        {
            StartCoroutine(TrackCompletedKeyOrder());
        }
    }

    private IEnumerator TrackCompletedKeyOrder()
    {
        while (keysIdx < keys.Length)
        {
            isRunning = false;
            if (isMobile)
            {
                //Adds the events to check if the specfic button was clicked
                AddToButtons();
            }
            yield return StartCoroutine(TrackKeys());

        }
        completed = true;
    }

    public IEnumerator TrackKeys()
    {
        while (!isRunning)
        {
            if (!isMobile)
            {
                //For keyboard
                if (Input.GetKeyDown(keys[keysIdx].GetKey()))
                {
                    keys[keysIdx].clicked = true;
                    keys[keysIdx].correctClick = true;
                }
                else if (AnyKeyExceptMouse())
                {
                    keys[keysIdx].clicked = true;
                    keys[keysIdx].correctClick = false;
                }
            }

            //Checks if the key or button has been pressed than checks if it was the correct one. If so they can move on to next element. If not repeat on the first element.
            if (keys[keysIdx].clicked)
            {
                if (keys[keysIdx].correctClick)
                {
                    amountPressed++;
                    isRunning = true;
                }
                else
                {
                    keys[keysIdx].clicked = false;
                    keys[keysIdx].correctClick = false;
                    keysIdx = 0;
                    amountPressed = 0;
                    isRunning = true;
                }
            }

            if (amountPressed == keys[keysIdx].amountOfPresses)
            {
                keys[keysIdx].clicked = false;
                keys[keysIdx].correctClick = false;
                keysIdx++;
                amountPressed = 0;
            }


            yield return null;
        }

        if (isMobile)
        {
            RemoveFromButtons();
        }

    }

    private bool AnyKeyExceptMouse()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2))
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public void FindButtons()
    {
        playerButtons = GameObject.FindGameObjectsWithTag("PlayerButton");
    }

    private void AddToButtons()
    {
        foreach (GameObject button in playerButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(() => keys[keysIdx].CheckIfClicked(button.GetInstanceID()));
        }
    }
    private void RemoveFromButtons()
    {

        for (int i = 0; i < playerButtons.Length; i++)
        {
            playerButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}

[Serializable]
public class AllKeys
{
    public enum PlayerKeys
    {
        None,
        Ability,
        Switch
    }
    public PlayerKeys playerKeys;
    public KeyCode keys;
    public Button playerButton;
    public bool clicked{ get; set; }
    public bool correctClick{ get; set; }
    public int amountOfPresses = 1;

    public KeyCode GetPlayerKey()
    {
        switch (playerKeys)
        {
            case PlayerKeys.Ability:
                return PlayerManager.playerManager.playerAbilityKey;
            case PlayerKeys.Switch:
                return PlayerManager.playerManager.playerSwitchFormKey;
        }
        return KeyCode.None;
    }

    public KeyCode GetKey()
    {
        if (playerKeys != PlayerKeys.None && keys != KeyCode.None)
        {
            Debug.LogError("Choose either player keys or regular keys");
            return KeyCode.None;
        }
        if (playerKeys != PlayerKeys.None)
        {
            return GetPlayerKey();
        }
        if (keys != KeyCode.None)
        {
            return keys;
        }
        Debug.LogError("NO KEY WAS SET");
        return KeyCode.None;
    }
    public void CheckIfClicked(int ID)
    {
        if (ID == playerButton.gameObject.GetInstanceID())
        {
            correctClick = true;
            clicked = true;
        }
        else
        {
            correctClick = false;
            clicked = true;
        }
    }
}
