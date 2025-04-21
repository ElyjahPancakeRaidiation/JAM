using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "Cutscene", menuName = "New Cutscene/Cutscene")]
public class CutSceneScriptable : ScriptableObject
{
    public CutSceneInfo[] cutSceneInfo;


    [Serializable]
    public class CutSceneInfo{
        public enum ActionType{MoveObj, AddForce, TurnObjectOn, TurnObjectOff, Wait};
        public ActionType actionType;

        [Tooltip("MoveObj key: speed(number for amount of speed)-(number for amount of easing applied when the object stops) | AddForce key: add(x for force added in the x axis, y for force added in the y axis, z is for the power)")]
        public string information;

        //This connects to the cutscenemanagers actors array which ever index you put it will corosponde with the event 
        [Tooltip("This connects to the corosponding actor index in Cut Scene Manager")]
        public int actorIndex;

        [Tooltip("This connects to the corosponding endposition index in Cut Scene Manager")]
        public int endPositionIndex;
        
        //The amount of time to wait before moving on to the next action
        [Tooltip("The amount of time to wait before moving on to the next action")]
        public float waitTime;

        public bool infinite;

        [Tooltip("Entering a number will clamp the x and y to that number. The minimum will be the negative of this number")]
        public Vector2 clampVelocity;
    }
}
