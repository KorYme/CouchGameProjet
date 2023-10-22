using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckerBoard))]
public class CheckerBoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CheckerBoard checkerBoard = (CheckerBoard)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Refresh"))
        {
            checkerBoard.UpdateData();
        }
        if (GUILayout.Button("Delete"))
        {
            checkerBoard.Delete();
        }
    }
}
