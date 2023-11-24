using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RumbleTest)), CanEditMultipleObjects]
public class RumbleEditor : Editor
{
    RumbleTest _rumble;
    string _assetName;

    private void OnEnable()
    {
        _rumble = target as RumbleTest;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space(3f);
        _assetName = EditorGUILayout.TextField(_assetName);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Single Press Rumble"))
        {
            RumbleValues asset = CreateInstance<RumbleValues>();
            asset.isHolding = false;
            asset.rumbleCurve = _rumble.SinglePressRumble.rumbleCurve;
            asset.rumbleName = _rumble.SinglePressRumble.rumbleName;
            AssetDatabase.CreateAsset(asset, $"Assets/ScriptableObjects/RumbleValues/SinglePressRumble_{_assetName}.asset");
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("Hold Rumble"))
        {
            RumbleValues asset = CreateInstance<RumbleValues>();
            asset.isHolding = true;
            asset.rumbleCurve = _rumble.HoldRumble.rumbleCurve;
            asset.rumbleName = _rumble.HoldRumble.rumbleName;
            AssetDatabase.CreateAsset(asset, $"Assets/ScriptableObjects/RumbleValues/HoldRumble_{_assetName}.asset");
            AssetDatabase.SaveAssets();
        }
        EditorGUILayout.EndHorizontal();
    }
}
