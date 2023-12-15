using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    [CustomEditor(typeof(DataSaveManager<>), true)]
    public class DataSaveManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("Destroy Old Data"))
            {
                (target as MonoBehaviour).SendMessage("DestroyOldData");
            }
            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
