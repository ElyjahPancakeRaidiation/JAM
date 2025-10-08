using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;

public class ConsoleScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField consoleImage;
    private GameObject consoleObj;
    public static ConsoleScript consoleScript;
    public delegate void ConsoleCommand(List<ConsoleCommands> cmd);
    public static ConsoleCommand AddCmds;
    public delegate void CmdFunction(string[] key);

    [SerializeField] private List<ConsoleCommands> cmds = new List<ConsoleCommands>();

    void Awake()
    {
        consoleScript = this.GetComponent<ConsoleScript>();
        if (consoleImage == null)
        {
            Debug.LogError("Missing console image component, remeber to add that before I explode and do evil things like set your house on fire");
        }
    }

    void Start()
    {
        consoleObj = consoleImage.gameObject;
        consoleObj.SetActive(false);
    }

    void Update()
    {

    }

    public void TURNONCONSOLE()
    {
        ChangeConsoleState();
    }

    public void SendCommand(string command)
    {
        string[] commandSplit = SeperateString(command);
        if (command != "" && commandSplit.Length >= 2)
        {
            ConsoleCommands cmd = SearchForCatergory(commandSplit[0], commandSplit[1]);
            if (cmd.category != "null")
            {
                cmd.GetCmdFunc().Invoke(commandSplit);
                StartCoroutine(FlashConsole(Color.green));
            }
            else
            {
                StartCoroutine(FlashConsole(Color.red));
            }
        }
    }

    public void AddCommand(string category, string objName, CmdFunction f)
    {
        ConsoleCommands command = new ConsoleCommands(category, objName, new ConsoleCommands.CmdFunction(f));
        cmds.Add(command);
    }

    private void ChangeConsoleState()
    {
        consoleObj.SetActive(!consoleObj.activeSelf);
        consoleImage.text = "";
    }

    #region  future stuff
    public string[] SeperateString(string command)
    {
        return command.Split(" ");
    }

    public string BinarySearch(string[] arr, string key)
    {

        int i = arr.Length - 1;
        int j = 0;
        int middle = i - j;
        while (middle > i)
        {
            if (String.Compare(arr[middle], key) > 0)
            {

            }
            else if (String.Compare(arr[middle], key) < 0)
            {

            }
        }

        return "";
    }
    public ConsoleCommands SearchForCatergory(string category, string objName)
    {
        foreach (ConsoleCommands item in cmds)
        {
            if (item.category == category)
            {
                if (item.GetObjName() == objName)
                {
                    return item;
                }
            }
        }

        Debug.Log("Error: Could not find category or objName in commands list");
        return new ConsoleCommands("null", "null", null);//Make it where it returns the function and sends out an error message in the console
    }
    public string[] MergeSort()
    {
        return new string[3];
    }

    #endregion

    private IEnumerator FlashConsole(Color color)
    {
        var oriColor = consoleImage.textComponent.color;
        consoleImage.textComponent.color = color;
        yield return new WaitForSeconds(0.5f);
        consoleImage.textComponent.color = oriColor;
        yield return new WaitForSeconds(0.2f);
        consoleImage.text = "";

    }
}

[Serializable]
public class ConsoleCommands
{
    public string category;
    public delegate void CmdFunction(string[] key);
    public class Command
    {
        public string objName;
        public CmdFunction cmdFunction;
        public Command(string objName, CmdFunction cmdFunction)
        {
            this.objName = objName;
            this.cmdFunction = cmdFunction;
        }
    }
    private Command cmd;
    public ConsoleCommands(string category, string objName, CmdFunction func)
    {
        this.category = category;
        cmd = new Command(objName, func);
    }
    public string GetObjName() { return cmd.objName; }
    public CmdFunction GetCmdFunc(){ return cmd.cmdFunction; }
}
