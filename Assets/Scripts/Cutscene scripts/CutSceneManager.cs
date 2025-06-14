using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TerrainUtils;

public class CutSceneManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;

    //A scriptable object containing all of the data for the scene
    [SerializeField] private CutSceneScriptable cutSceneToPlay;

    //the objects that will be moved through out the scene.
    [Tooltip("The objects that will be moved through out the scene.")]
    [SerializeField] private GameObject[] actorObjects;

    //The end position the actors will have at the end of their actions
    [Tooltip("The end position the actors will have at the end of their actions.")]
    [SerializeField] private GameObject[] endPositions;

    [SerializeField] private UnityEvent[] inGameEvents;

    [SerializeField] private bool stopWhenSceneStarts;
    public bool playOnStart;
    private bool canPlayCutScene = false;
    private bool isPlaying = false;
    //Ensures the current scene is finished before moving on to the next
    private bool canMoveOn = false;
    private bool isFinished = false;
    private int sceneCounter = 0;

    private void Start()
    {

        if (playOnStart)
        {
            canPlayCutScene = true;
        }
    }

    void FixedUpdate()
    {
        if (canPlayCutScene)
        {
            if (!isPlaying)
            {
                sceneCounter = 0;
                StartCutscene();
            }
        }
    }

    void StartCutscene()
    {
        isPlaying = true;
        isFinished = false;
        if(playerMovement != null){playerMovement.setCanControl(false);}
        if(stopWhenSceneStarts){ StartCoroutine(easeObj(50)); }
        StartCoroutine(RunCutScene(cutSceneToPlay));
    }

    private IEnumerator RunCutScene(CutSceneScriptable scene)
    {
        Debug.Log("I'm still running Cutscene");
        //Base case to stop the loop when theres no more scenes
        if (sceneCounter == scene.cutSceneInfo.Length)
        {
            isPlaying = false;
            canPlayCutScene = false;
            if(playerMovement != null){ playerMovement.setCanControl(true); }
            CameraOperator playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOperator>();
            playerCamera.setFollowPlayer(true);
            isFinished = true;
            yield break;
        }

        //Loop through each of the scenes in the array cutscene info
        if (!scene.cutSceneInfo[sceneCounter].infinite)
        {
            startAction(scene.cutSceneInfo[sceneCounter].actionType, scene);
        }
        else
        {
            while (scene.cutSceneInfo[sceneCounter].infinite)
            {
                startAction(scene.cutSceneInfo[sceneCounter].actionType, scene);
                yield return new WaitForSecondsRealtime(scene.cutSceneInfo[sceneCounter].waitTime);
            }
        }
        yield return new WaitUntil(() => canMoveOn);
        yield return new WaitForSecondsRealtime(scene.cutSceneInfo[sceneCounter].waitTime);
        sceneCounter++;
        canMoveOn = false;//Resets the value for the new instance.
        //Do the actions it requires.
        StartCoroutine(RunCutScene(scene));

    }

    private void startAction(CutSceneScriptable.CutSceneInfo.ActionType e, CutSceneScriptable c){
        switch (e)
        {
            case CutSceneScriptable.CutSceneInfo.ActionType.MoveObj:
                StartCoroutine(MoveObj(c));
                break;
            case CutSceneScriptable.CutSceneInfo.ActionType.AddForce:
                AddForceToObject(c);
                break;
            case CutSceneScriptable.CutSceneInfo.ActionType.TurnObjectOff:
                TurnOffObject(c);
                break;
            case CutSceneScriptable.CutSceneInfo.ActionType.TurnObjectOn:
                TurnOnObject(c);
                break;
            case CutSceneScriptable.CutSceneInfo.ActionType.Wait:
                canMoveOn = true;
                break;
            case CutSceneScriptable.CutSceneInfo.ActionType.Event:
                inGameEvents[c.cutSceneInfo[sceneCounter].eventIndex].Invoke();
                canMoveOn = true;
                break;
        }
    }

    #region Objects
    private void AddForceToObject(CutSceneScriptable c)
    {
        Vector3 amount = InfoToVector2(c.cutSceneInfo[sceneCounter].information);

        actorObjects[c.cutSceneInfo[sceneCounter].actorIndex].GetComponent<Rigidbody2D>().AddForce(new Vector2(amount.x * amount.z, amount.y * amount.z), ForceMode2D.Impulse);
        Vector2 clampedVel = clampVelocity(actorObjects[c.cutSceneInfo[sceneCounter].actorIndex].GetComponent<Rigidbody2D>().velocity, c.cutSceneInfo[sceneCounter].clampVelocity);
        actorObjects[c.cutSceneInfo[sceneCounter].actorIndex].GetComponent<Rigidbody2D>().velocity = clampedVel;
        //If it is not playing infinitely then move on to the next scene
        if (!c.cutSceneInfo[sceneCounter].infinite) { canMoveOn = true; }
    }
    private IEnumerator MoveObj(CutSceneScriptable c)
    {
        GameObject actor = actorObjects[c.cutSceneInfo[sceneCounter].actorIndex];
        GameObject endPos = endPositions[c.cutSceneInfo[sceneCounter].endPositionIndex];
        Rigidbody2D _actorRb = actor.GetComponent<Rigidbody2D>();
        float speed = InfoToFloat(c.cutSceneInfo[sceneCounter].information);
        //make sure it is between 1 and -1
        float dirFuck = Mathf.Sign(endPos.transform.position.x - actor.transform.position.x);


        while (!canMoveOn)
        {
            //Checks if the actor object is close to the endposition
            if (Vector2.Distance(actor.transform.position, endPos.transform.position) > 0.6f)
            {
                if (dirFuck < 0)
                {
                    //Constantly adds a force to the object and clamping it between the amount chosen to be clamped. Left
                    _actorRb.AddForce(Vector2.left * speed);
                    Vector2 vel = clampVelocity(_actorRb.velocity, c.cutSceneInfo[sceneCounter].clampVelocity);
                    _actorRb.velocity = vel;
                    yield return new WaitForSecondsRealtime(0.2f);
                }
                else if (dirFuck > 0)
                {
                    //Constantly adds a force to the object and clamping it between the amount chosen to be clamped. Right
                    _actorRb.AddForce(Vector2.right * speed);
                    Vector2 vel = clampVelocity(_actorRb.velocity, c.cutSceneInfo[sceneCounter].clampVelocity);
                    _actorRb.velocity = vel;
                    yield return new WaitForSecondsRealtime(0.2f);
                }
            }
            else
            {
                float easingAmount = 0;
                if (c.cutSceneInfo[sceneCounter].information.IndexOf('-') != -1)
                {
                    string sub = c.cutSceneInfo[sceneCounter].information.Substring(c.cutSceneInfo[sceneCounter].information.IndexOf('-') + 1);
                    easingAmount = InfoToFloat(sub);
                }
                StartCoroutine(easeObj(actor, easingAmount));
                yield break;
            }
        }
    }
    private void TurnOffObject(CutSceneScriptable c)
    {
        actorObjects[c.cutSceneInfo[sceneCounter].actorIndex].SetActive(false);
        canMoveOn = true;
    }
    private void TurnOnObject(CutSceneScriptable c)
    {
        actorObjects[c.cutSceneInfo[sceneCounter].actorIndex].SetActive(true);
        canMoveOn = true;
    }
    #endregion

    private IEnumerator easeObj(GameObject obj, float easingAmount = 0)//This function is for any object
    {
        if (easingAmount == 0) { easingAmount = 30; }
        Vector2 velocity = obj.GetComponent<Rigidbody2D>().velocity;
        if (Vector2.Distance(velocity, Vector2.zero) < 0.1f)
        {
            canMoveOn = true;
            yield break;
        }
        // if(Vector2.Distance(velocity, new Vector2(0.5f, 0.5f)) < 0.2f){yield break;}
        velocity.x = Mathf.Lerp(velocity.x, 0, easingAmount * Time.deltaTime);
        velocity.y = Mathf.Lerp(velocity.y, 0, easingAmount * Time.deltaTime);
        obj.GetComponent<Rigidbody2D>().angularVelocity = Mathf.Lerp(obj.GetComponent<Rigidbody2D>().angularVelocity, 0, 30 * Time.deltaTime);
        obj.GetComponent<Rigidbody2D>().velocity = velocity;
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(easeObj(obj));
    }

    private IEnumerator easeObj(float easingAmount = 0)//This is for the player
    {
        if (easingAmount == 0) { easingAmount = 30; }
        Vector2 velocity = playerMovement.getCurVelocity();
        if (Vector2.Distance(velocity, Vector2.zero) < 0.1f){yield break;}
        // if(Vector2.Distance(velocity, new Vector2(0.5f, 0.5f)) < 0.2f){yield break;}
        velocity.x = Mathf.Lerp(velocity.x, 0, easingAmount * Time.deltaTime);
        velocity.y = Mathf.Lerp(velocity.y, 0, easingAmount * Time.deltaTime);
        playerMovement.GetComponent<Rigidbody2D>().angularVelocity = Mathf.Lerp(playerMovement.GetComponent<Rigidbody2D>().angularVelocity, 0, 30 * Time.deltaTime);
        playerMovement.GetComponent<Rigidbody2D>().velocity = velocity;
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(easeObj());
    }

    private Vector3 InfoToVector2(string information)
    {//Turns the string info in cut scene scriptable to a vector
        string x = "", y = "", z = "";
        //Only using one bool to find the comma since we are only dealing with a vector2
        int passedComma = 0;
        //This gets a sub string from the ( char all the way to the end: EX: add(0, 3) -> (0, 3, 2) sub string
        information = information.Substring(information.IndexOf('(') + 1);
        //The for loop goes through the substring and gets every character before the comma. X
        //--exluding the paranthesis--
        //and every character after the comma. Y
        for (int i = 0; i < information.Length; i++)
        {
            //Ends the loop once we hit the last paranthesis so we dont get any extra characters
            if (information[i] == ')') { break; }
            if (information[i] == ',')
            {
                passedComma++;
                i++;//This ensures that we don't get the comma when adding it to the y and z.
            }
            switch (passedComma)
            {
                case 0:
                    x += information[i];
                    break;
                case 1:
                    y += information[i];
                    break;
                case 2:
                    z += information[i];
                    break;
            }
        }

        //Turns the string x, y, and z into floats
        float xF = float.Parse(x);
        float yF = float.Parse(y);
        float zF = float.Parse(z);
        return new Vector3(xF, yF, zF);
    }
    private float InfoToFloat(string information)
    {//Turns the string info in cut scene scriptable to a vector
        string speed = "";
        //This gets a sub string from the ( char all the way to the end: EX: add(0, 3) -> (0, 3, 2) sub string
        information = information.Substring(information.IndexOf('(') + 1);
        //The for loop goes through the substring and gets every character before the end paranthesis
        for (int i = 0; i < information.Length; i++)
        {
            //Ends the loop once we hit the last paranthesis so we dont get any extra characters
            if (information[i] == ')') { break; }
            speed += information[i];
        }

        //Turns the string x, y, and z into floats
        float xF = float.Parse(speed);
        return xF;
    }
    private Vector2 clampVelocity(Vector2 val, Vector2 amountToClamp){
        if (amountToClamp != Vector2.zero)
        {
            val.x = Mathf.Clamp(val.x, -amountToClamp.x, amountToClamp.x);
            val.y = Mathf.Clamp(val.y, -amountToClamp.y, amountToClamp.y);
            return new Vector2(val.x, val.y);
        }

        return val;
    }
    public void playCutScene() { canPlayCutScene = true; }
    public bool getIsPlaying() { return isPlaying; }
    public bool getIsFinished() { return isFinished; }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isFinished)
            {
                canPlayCutScene = true;
            }
        }
    }
}



//Linear drag
//Lowering friction at a lower point when going back up on a hill
//Godwalker movement joshes git hub
//if untiy editor x64
