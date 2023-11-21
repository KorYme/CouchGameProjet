using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterAnimationObject)),CanEditMultipleObjects]
public class CharacterAnimationObjectEditor : Editor
{
    private bool isPreviewing;
    private int animIndex;

    private double _timer;
    private double _timeBetweenframe;
    CharacterAnimationObject anim ;

    private void OnEnable()
    {
        anim = (CharacterAnimationObject)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUILayout.Label($"Custom Animatooor", new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            font = (Font)AssetDatabase.LoadAssetAtPath("Assets/Font/Confetti Stream.ttf", typeof(Font)),
            fontSize = 50,
            margin = new RectOffset(10,10,15,15),
        });
        //anim.ClearAnim();
        if (GUILayout.Button("ClearAnim"))
        {
            anim.ClearAnim();
        }
        GUILayout.BeginHorizontal();
        {
            anim.selectedAnimationType = (ANIMATION_TYPE) EditorGUILayout.EnumPopup(anim.selectedAnimationType);
            if (GUILayout.Button("Add Animation"))
            {
                anim.AddAnimation();
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        {
            anim.animBpm = Mathf.Max(90,EditorGUILayout.IntField("BPM", anim.animBpm));
            if (GUILayout.Button("Preview Anim"))
            {
                _timeBetweenframe = 60f / anim.animBpm;
                EditorApplication.update += AnimRoutine;
                isPreviewing = true;
                _timer = EditorApplication.timeSinceStartup + _timeBetweenframe;
                animIndex = 0;
            }

            if (GUILayout.Button("Stop Preview"))
            {
                EditorApplication.update -= AnimRoutine;
                animIndex = 0;
                isPreviewing = false;
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginVertical();
        {
            for (int i = 0; i < anim.animationsList.Count; ++i)
            {
                GUILayout.Label($"{anim.animationsList[i].AnimationType} animation", new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    font = (Font)AssetDatabase.LoadAssetAtPath("Assets/Font/Confetti Stream.ttf", typeof(Font)),
                    fontSize = 25,
                    margin = new RectOffset(10,10,30,30),
                });
                
                if (anim.animationsList[i].animationSprites.Count > 0)
                {
                    //Debug.Log(animIndex % anim.animationsList[i].animationSprites.Count);
                    if (anim.animationsList[i]
                            .animationSprites[animIndex % anim.animationsList[i].animationSprites.Count] != null)
                    {
                        Texture t = anim.animationsList[i]
                            .animationSprites[animIndex % anim.animationsList[i].animationSprites.Count].texture;
                        GUILayout.Label(t , new GUIStyle(GUI.skin.label)
                        {
                            fixedHeight = 100,
                            fixedWidth = 100,
                            margin = new RectOffset(Screen.width / 2 - 50 ,0,0,0)
                        });
                    }
                }
                
                
                if (GUILayout.Button("Add Frame"))
                {
                    anim.animationsList[i].animationSprites.Add(null);
                }
                for (int j = 0; j < anim.animationsList[i].animationSprites.Count; j++)
                {
                    GUILayout.BeginHorizontal();
                    {
                        anim.animationsList[i].animationSprites[j] = EditorGUILayout.ObjectField(anim.animationsList[i].animationSprites[j], typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
                        if (GUILayout.Button("Remove frame"))
                        {
                            anim.animationsList[i].animationSprites.Remove(anim.animationsList[i].animationSprites[j]);
                        }
                    }
                    GUILayout.EndHorizontal();
                    //EditorGUILayout.ObjectField(new GUIContent("",""), null, typeof(Sprite));
                }
                if (GUILayout.Button("Delete Animation"))
                {
                    anim.RemoveAnimation(anim.animationsList[i]);
                }
            }
        }
        GUILayout.EndVertical();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(anim);
            AssetDatabase.SaveAssets();
        }
    }
    
    private void AnimRoutine()
    {
        if (isPreviewing && EditorApplication.timeSinceStartup >= _timer)
        {
            _timer = EditorApplication.timeSinceStartup + _timeBetweenframe;
            animIndex++;
            Repaint();
        }        
    }
}   
