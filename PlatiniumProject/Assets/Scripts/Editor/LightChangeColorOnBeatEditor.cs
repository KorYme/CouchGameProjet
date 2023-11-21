using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightChangeColorOnBeat))]
public class LightChangeColorOnBeatEditor : Editor
{
    LightChangeColorOnBeat _lightChangeColorOnBeat;
    private void OnEnable()
    {
        _lightChangeColorOnBeat = target as LightChangeColorOnBeat;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("SetUpLights"))
        {
            _lightChangeColorOnBeat.SetUpLights();
        }
        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
