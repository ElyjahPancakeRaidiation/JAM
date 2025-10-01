using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor
{
    SerializedProperty colliderMask;
    SerializedProperty colliderSize, colliderOffsetSize;

    SerializedProperty moveToTarget;
    SerializedProperty changeCameraSize;
    SerializedProperty changeCameraOffset;

    //Settings for moving to a location
    SerializedProperty camTarget;
    SerializedProperty newSpeed;
    SerializedProperty returningPercentageSpeed;
    SerializedProperty keepSpeedSettings;

    //Settings for camera zooms in and out
    SerializedProperty newCameraSize, cameraSizeSpeed;
    SerializedProperty returningSize, returningSizeSpeed, returnCurSpeedDistance;
    SerializedProperty keepSizeSettings;

    //Settings for changing the offset
    SerializedProperty newCameraOffset, cameraOffsetSpeed;
    SerializedProperty returningCameraOffset, returningCameraOffsetSpeed;
    SerializedProperty keepOffsetSettings;

    SerializedProperty shakeCamera;
    SerializedProperty shakeDuration;
    SerializedProperty shakeStrength;
    SerializedProperty delayShakeSec;
    SerializedProperty repeat;

    SerializedProperty camWaitTime;
    SerializedProperty useOrigSize;


    void OnEnable()
    {
        colliderMask = serializedObject.FindProperty(nameof(colliderMask));
        colliderSize = serializedObject.FindProperty(nameof(colliderSize));
        colliderOffsetSize = serializedObject.FindProperty(nameof(colliderOffsetSize));

        moveToTarget = serializedObject.FindProperty(nameof(moveToTarget));
        changeCameraSize = serializedObject.FindProperty(nameof(changeCameraSize));
        changeCameraOffset = serializedObject.FindProperty(nameof(changeCameraOffset));
        shakeCamera = serializedObject.FindProperty(nameof(shakeCamera));

        shakeDuration = serializedObject.FindProperty(nameof(shakeDuration));
        shakeStrength = serializedObject.FindProperty(nameof(shakeStrength));
        delayShakeSec = serializedObject.FindProperty(nameof(delayShakeSec));
        repeat = serializedObject.FindProperty(nameof(repeat));

        camTarget = serializedObject.FindProperty(nameof(camTarget));
        newSpeed = serializedObject.FindProperty(nameof(newSpeed));
        cameraSizeSpeed = serializedObject.FindProperty(nameof(cameraSizeSpeed));
        cameraOffsetSpeed = serializedObject.FindProperty(nameof(cameraOffsetSpeed));

        newCameraSize = serializedObject.FindProperty(nameof(newCameraSize));
        newCameraOffset = serializedObject.FindProperty(nameof(newCameraOffset));

        returningPercentageSpeed = serializedObject.FindProperty(nameof(returningPercentageSpeed));
        returningSize = serializedObject.FindProperty(nameof(returningSize));
        returningSizeSpeed = serializedObject.FindProperty(nameof(returningSizeSpeed));
        returnCurSpeedDistance = serializedObject.FindProperty(nameof(returnCurSpeedDistance));
        returningCameraOffset = serializedObject.FindProperty(nameof(returningCameraOffset));
        returningCameraOffsetSpeed = serializedObject.FindProperty(nameof(returningCameraOffsetSpeed));

        keepSpeedSettings = serializedObject.FindProperty(nameof(keepSpeedSettings));
        keepSizeSettings = serializedObject.FindProperty(nameof(keepSizeSettings));
        keepOffsetSettings = serializedObject.FindProperty(nameof(keepOffsetSettings));

        camWaitTime = serializedObject.FindProperty(nameof(camWaitTime));
        useOrigSize = serializedObject.FindProperty(nameof(useOrigSize));

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        VariableField(colliderMask);
        VariableField(colliderSize);
        VariableField(colliderOffsetSize);


        VariableField(moveToTarget);
        VariableField(changeCameraSize);
        VariableField(changeCameraOffset);
        VariableField(shakeCamera);

        if (moveToTarget.boolValue)
        {
            VariableField(camTarget);
            VariableField(newSpeed);
            if (!keepSpeedSettings.boolValue)
            {
                VariableField(returningPercentageSpeed);
                VariableField(returnCurSpeedDistance);
            }
            VariableField(keepSpeedSettings);
        }

        if (changeCameraSize.boolValue)
        {
            VariableField(newCameraSize);
            VariableField(cameraSizeSpeed);
            if (!keepSizeSettings.boolValue)
            {
                if(!useOrigSize.boolValue){VariableField(returningSize);}
                VariableField(returningSizeSpeed);
            }
            VariableField(useOrigSize);
            VariableField(keepSizeSettings);

        }

        if (changeCameraOffset.boolValue)
        {
            VariableField(newCameraOffset);
            VariableField(cameraOffsetSpeed);
            if (!keepOffsetSettings.boolValue)
            {
                VariableField(returningCameraOffset);
                VariableField(returningCameraOffsetSpeed);
            }
            VariableField(keepOffsetSettings);

        }

        if (shakeCamera.boolValue)
        {
            VariableField(shakeDuration);
            VariableField(shakeStrength);
            VariableField(delayShakeSec);
            VariableField(repeat);
        }

        VariableField(camWaitTime);

        serializedObject.ApplyModifiedProperties();


    }

    private void VariableField(SerializedProperty variable) {
        EditorGUILayout.PropertyField(variable, new GUIContent(variable.name));
    }
}
