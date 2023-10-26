using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QTELoader))]
public class QteLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        QTELoader qteLoader = (QTELoader)target;
        DrawDefaultInspector();
        if (GUILayout.Button("LoadQte"))
        {
            qteLoader.LoadQTE();
        }
    }
}
