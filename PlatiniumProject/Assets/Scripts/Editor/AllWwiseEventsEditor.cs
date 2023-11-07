using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AllWwiseEvents))]
public class AllWwiseEventsEditor : Editor
{
    #region PROPERTIES
    string FolderPath
    {
        get => Path.Combine(Application.dataPath, "Scripts", "Enums&Consts");
    }

    string FileName => "WwiseEventEnum.cs";

    string FilePath => Path.Combine(FolderPath, FileName);
     
    string ClassCodeStart(string enumName) =>
            $"public enum {enumName}" + "\n" +
            "{" + "\n";
    #endregion

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Constant Class"))
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
                AssetDatabase.Refresh();
                Debug.LogWarning("Generating folder, wait a moment before retrying");
                return;
            }
            AllWwiseEvents allWwiseEvents = target as AllWwiseEvents;
            string fileContent = ClassCodeStart("WwiseEventEnumMusic");
            if (allWwiseEvents != null)
            {
                foreach (AK.Wwise.Event item in allWwiseEvents.AllMusicEvents)
                {
                    if (item?.Name == "") continue;
                    fileContent += $"    {item.Name}," + "\n";
                }
                fileContent += "}" + "\n\n" + ClassCodeStart("WwiseEventEnumSFX");
                foreach (AK.Wwise.Event item in allWwiseEvents.AllSFXEvents)
                {
                    if (item.Name == "") continue;
                    fileContent += $"    {item.Name}," + "\n";
                }
                fileContent += "}";
            }
            File.WriteAllText(FilePath, fileContent);
            AssetDatabase.Refresh();
        }
        EditorUtility.SetDirty(target);
    }
}
