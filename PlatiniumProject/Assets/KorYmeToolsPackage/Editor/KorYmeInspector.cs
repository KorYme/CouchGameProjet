using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace KorYmeLibrary
{
    //[CanEditMultipleObjects]
    //[CustomEditor(typeof(MonoBehaviour), true)]
    //public class KorYmeInspector : UnityEditor.Editor
    //{
    //    IEnumerable<MethodInfo> _allMethods;

    //    protected virtual void OnEnable()
    //    {
    //        _allMethods = ReflectionUtility.GetAllMethods(target, m => m.GetCustomAttributes(typeof(MethodAttribute), true).Length > 0);
    //    }

    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();
    //        EditorGUILayout.Space(15f);
    //        DrawButtons();
    //    }

    //    protected void DrawButtons()
    //    {
    //        foreach (MethodInfo method in _allMethods)
    //        {
    //            KorYmeEditorGUI.Button(serializedObject.targetObject, method);
    //        }
    //    }
    //}
}
