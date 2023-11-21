using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SetUpNeonLight))]
public class ChangeNeonColorEditor : Editor
{
    SetUpNeonLight _neonColor;

    private void OnEnable()
    {
        _neonColor = target as SetUpNeonLight;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            _neonColor.SetMaterialColor();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Set Up Vertex Z"))
        {
            _neonColor.SetUpAllVertex();
        }
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
