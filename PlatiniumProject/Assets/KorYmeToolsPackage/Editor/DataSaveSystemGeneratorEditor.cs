using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KorYmeLibrary.SaveSystem
{
    [CustomEditor(typeof(DataSaveSystemGenerator)), CanEditMultipleObjects]
    public class DataSaveSystemGeneratorEditor : Editor
    {
        DataSaveSystemGenerator _dataSaveSystem;

        private void OnEnable()
        {
            _dataSaveSystem = target as DataSaveSystemGenerator;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            if (GUILayout.Button("Generate Save System Folder"))
            {
                _dataSaveSystem.GenerateSaveSystemFolder();
            }
            if (GUILayout.Button("Generate Game Data Class"))
            {
                _dataSaveSystem.GenerateGameDataClass();
            }
            if (GUILayout.Button("Attach Data Save Manager"))
            {
                _dataSaveSystem.AttachDataSaveManager();
            }

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
