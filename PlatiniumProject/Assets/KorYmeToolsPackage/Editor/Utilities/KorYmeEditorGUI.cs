using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace KorYmeLibrary
{
    public static class KorYmeEditorGUI
    {
        #region METHODS
        public static void Button(UnityEngine.Object target, MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().All(p => p.IsOptional))
            {
                if (GUILayout.Button(ObjectNames.NicifyVariableName(methodInfo.Name)))
                {
                    object[] defaultParams = methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();
                    IEnumerator methodResult = methodInfo.Invoke(target, defaultParams) as IEnumerator;

                    if (!Application.isPlaying)
                    {
                        // Set target object and scene dirty to serialize changes to disk
                        EditorUtility.SetDirty(target);

                        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                        if (stage != null)
                        {
                            // Prefab mode
                            EditorSceneManager.MarkSceneDirty(stage.scene);
                        }
                        else
                        {
                            // Normal scene
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }
                    else if (methodResult != null && target is MonoBehaviour behaviour)
                    {
                        behaviour.StartCoroutine(methodResult);
                    }
                }

                EditorGUI.EndDisabledGroup();
            }
            else
            {
                string warning = methodInfo.Name + " works only on methods with no parameters";
            }
        }
        #endregion
    }
}
