using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpotLightRotation))]
public class SpotLightRotationEditor : Editor
{
    SpotLightRotation _spotLightScript;

    private void OnEnable()
    {
        _spotLightScript = target as SpotLightRotation;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        {
            if (GUILayout.Button("PlaceAtFirstRotation"))
            {
                _spotLightScript.transform.eulerAngles = new(0, 0, _spotLightScript.firstLocationRotation);
            }
            if (GUILayout.Button("PlaceAtSecondRotation"))
            {
                _spotLightScript.transform.eulerAngles = new(0, 0, _spotLightScript.secondLocationRotation);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("SetFirstRotation"))
            {
                _spotLightScript.firstLocationRotation = _spotLightScript.transform.eulerAngles.z;
            }
            if (GUILayout.Button("SetSecondRotation"))
            {
                _spotLightScript.secondLocationRotation = _spotLightScript.transform.eulerAngles.z;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(target);
    }
}

