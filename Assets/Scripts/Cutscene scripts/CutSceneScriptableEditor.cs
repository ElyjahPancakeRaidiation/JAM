using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CutSceneScriptable.CutSceneInfo))]
public class CutSceneScriptableEditor : Editor
{

    SerializedProperty _cutSceneInfo;
    SerializedProperty _actionType;
    SerializedProperty _information;
    SerializedProperty _actorIndex;
   
    private void OnEnable()
    {
        
        // _cutSceneInfo = serializedObject.FindProperty("cutSceneInfo");
        _actionType = serializedObject.FindProperty("actionType");
        _information = serializedObject.FindProperty("information");
        _actorIndex = serializedObject.FindProperty("actorIndex");
        
    }

    public override void OnInspectorGUI()
    {
        // CutSceneScriptable.CutSceneInfo tar = (CutSceneScriptable.CutSceneInfo)
        // EditorGUILayout.LabelField("uhj");
        base.OnInspectorGUI();
        // serializedObject.UpdateIfRequiredOrScript();

        // // EditorGUILayout.PropertyField(_cutSceneInfo, new GUIContent("Cut Scene Info"));
        // EditorGUILayout.PropertyField(_actionType, new GUIContent("actionType"));
        // EditorGUILayout.Space(10);
        // EditorGUILayout.PropertyField(_actorIndex, new GUIContent("actorIndex"));

        // if(_actorIndex.intValue == 1){
        //     EditorGUILayout.PropertyField(_information, new GUIContent("information"));
        //     EditorGUILayout.Space(10);

        // }
        // // base.OnInspectorGUI();
        // EditorGUILayout.Space(10);


        // serializedObject.ApplyModifiedProperties();
    }

    
}
