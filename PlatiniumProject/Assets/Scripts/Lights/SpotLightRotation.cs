using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering.Universal;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpotLightRotation : MonoBehaviour
{
    [SerializeField] Light2D _light2D;
    [SerializeField, Range(0f, 10f)] float _speed = 1;
    [SerializeField] List<Color> _allPossibleColors;
    [HideInInspector] public float firstLocationRotation, secondLocationRotation;

    public float FirstLocationRotation => firstLocationRotation + 90;
    public float SecondLocationRotation => secondLocationRotation + 90;

    private void Reset()
    {
        _light2D = GetComponent<Light2D>();
    }

    private IEnumerator Start()
    {
        Globals.BeatManager.OnBeatEvent.AddListener(ChangeToRandomColor);
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime * _speed;
            transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(firstLocationRotation, secondLocationRotation, (Mathf.Cos(timer) + 1) / 2));
            yield return null;
        }
    }

    private void OnDestroy()
    {
        Globals.BeatManager.OnBeatEvent.RemoveListener(ChangeToRandomColor);
    }

    private void ChangeToRandomColor()
    {
        if (_allPossibleColors == null || _allPossibleColors.Count == 0) return;
        _light2D.color = _allPossibleColors[Random.Range(0, _allPossibleColors.Count - 1)];
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(FirstLocationRotation* Mathf.Deg2Rad), Mathf.Sin(FirstLocationRotation * Mathf.Deg2Rad),0) * 10f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(SecondLocationRotation * Mathf.Deg2Rad), Mathf.Sin(SecondLocationRotation * Mathf.Deg2Rad),0) * 10f);
    }
}


#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(SpotLightRotation))]
public class SpotLightRotationEditor : Editor
{
    SpotLightRotation _spotLightScript;

    private void OnEnable()
    {
        _spotLightScript = target as SpotLightRotation;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        {
            if (GUILayout.Button("PlaceAtFirstRotation"))
            {
                _spotLightScript.transform.eulerAngles = new(0, 0, _spotLightScript.firstLocationRotation);
            }
            if (GUILayout.Button("PlaceAtSecondRotation"))
            {
                _spotLightScript.transform.eulerAngles = new(0, 0, _spotLightScript.secondLocationRotation);
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5f);

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("SetFirstRotation"))
            {
                _spotLightScript.firstLocationRotation = _spotLightScript.transform.eulerAngles.z;
            }
            if (GUILayout.Button("SetSecondRotation"))
            {
                _spotLightScript.secondLocationRotation = _spotLightScript.transform.eulerAngles.z;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(target);
    }
}

#endif