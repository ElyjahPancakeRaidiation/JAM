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
    [SerializeField] private AllKeys[] keys;
    private GameObject[] playerButtons;
    private List<UnityEvent<int>> fn = new List<UnityEvent<int>>();
    [SerializeField] int idx = 0;
    [SerializeField] int amountPressed = 0;

    private void Start()
    {
        if (isMobile)
        {
            FindButtons();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(testEnum());
        }
    }

    private bool testBool()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Running twice in the function");
            idx++;
            return true;
        }

        return false;
    }

    private IEnumerator testEnum()
    {
        yield return new WaitUntil(() => testBool());
        StartCoroutine(testEnum());
    }

    public void StartTrackingKeys()
    {
        StartCoroutine(TrackKeys());
    }

    public IEnumerator TrackKeys()
    {
        if (idx == keys.Length)
        {
            completed = true;
            yield break;
        }

        Debug.Log("bEING URN");
        if (isMobile)
        {
            AddToButtons();
        }
        yield return new WaitUntil(() => PickedKey());
        // StartCoroutine(TrackKeys());
        Debug.Log("Still running");

    }

    private bool PickedKey()
    {
        int currentIdx = idx;
        if (!isMobile)
        {
            if (Input.GetKeyDown(keys[currentIdx].GetKey()))
            {
                keys[currentIdx].clicked = true;
                keys[currentIdx].correctClick = true;
            }
            else if (AnyKeyExceptMouse())
            {
                keys[currentIdx].clicked = true;
                keys[currentIdx].correctClick = false;
            }
        }

        if (keys[currentIdx].clicked)
        {
            if (keys[currentIdx].correctClick)
            {
                amountPressed++;
            }
            else
            {
                idx = 0;
                amountPressed = 0;
                keys[currentIdx].clicked = false;
                keys[currentIdx].correctClick = false;
                return true;
            }


            if (amountPressed == keys[currentIdx].amountOfPresses)
            {
                Debug.Log("Did i get pressed>>??");
                if (isMobile)
                {
                    RemoveFromButtons();
                }
                idx++;
                amountPressed = 0;
                keys[currentIdx].clicked = false;
                keys[currentIdx].correctClick = false;
                StartCoroutine(TrackKeys());
                return true;
            }
            
        }
        return false;
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
            UnityEvent<int> fns = new UnityEvent<int>();
            fns.AddListener(keys[idx].CheckIfClicked);
            button.GetComponent<Button>().onClick.AddListener(() => fns.Invoke(button.GetInstanceID()));
            fn.Add(fns);
        }
    }
    private void RemoveFromButtons()
    {
        for (int i = 0; i < playerButtons.Length; i++)
        {
            playerButtons[i].GetComponent<Button>().onClick.RemoveListener(() => fn[i].Invoke(playerButtons[i].gameObject.GetInstanceID()));
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
                return PlayerManager.playerManager.playerAbilityKey;
        }
        return KeyCode.None;
    }

    public KeyCode GetKey()
    {
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
