using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QTELoader))]
public class QteLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        QTELoader qteLoader = (QTELoader)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("LoadQte"))
        {
            qteLoader.LoadQTE();
        }
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
