using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OnomatopoeiaHandler))]
public class OnomatopoeiaHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        OnomatopoeiaHandler ono = (OnomatopoeiaHandler)target;
        if (GUILayout.Button("Play"))
        {
            ono.TestRoutine();
        }
        if (GUILayout.Button("Play2"))
        {
            ono.TestRoutine2();
        }
    }

}
