using DG.DOTweenEditor.UI;
using Rewired;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QTEWindow : EditorWindow
{
    List<QTESequence> _drinks;
    QTESequence _selectedQTE = null;
    bool _isATemporaryQTE = false;
    int _indexNewSequence = 0;//Used to have different names for file sequences
    bool _showListInputs = true;
    Vector2 _scrollQTEListPosition;
    Vector2 _scrollInputsPosition;

    #region ListOptionsKeys
    //Used for the display of the rewired keys
    private string[] _buttonInputOptions = null;
    const string NAME_ACTION = nameof(UnitInput.ActionIndex);

    SerializedObject _serializedObject;
    SerializedProperty _propertyName;
    #endregion

    #region Styles
    GUIStyle _styleButtonAddQTE;
    #endregion

    [MenuItem("Tools/QTEWindow")]
    static void InitWindow()
    {
        QTEWindow window = GetWindow<QTEWindow>();
        window.titleContent = new GUIContent("Tool QTE inputs");
        window.Show();
    }
    private void Awake()
    {
        LoadQTE();
        _styleButtonAddQTE = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(3,5,1,1),
            margin = new RectOffset(3,5,1,1),
            normal = new GUIStyleState() { background = Texture2D.whiteTexture}
        };
    }
    void LoadQTE()
    {
        if (_drinks == null)
        {
            _drinks = new List<QTESequence>();
        }
        else
        {
            _drinks.Clear();
        }
        string[] fileGuids = AssetDatabase.FindAssets("t:" + typeof(QTESequence));
        if (fileGuids.Length > 0)
        {
            for (int i = 0; i < fileGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(fileGuids[i]);
                _drinks.Add(AssetDatabase.LoadAssetAtPath<QTESequence>(assetPath));
            }
        }
        _indexNewSequence = _drinks.Count;
    }

    void DrawSideBar()
    {
        if (_drinks != null)
        {
            for (int i = 0; i < _drinks.Count; i++)
            {
                if (_selectedQTE != null && _selectedQTE.Index == i)
                {
                    GUI.backgroundColor = Color.green;
                } else
                {
                    GUI.backgroundColor = Color.white;
                }
                string name = Enum.GetName(typeof(PlayerRole), _drinks[i].PlayerRole);
                if (GUILayout.Button($"QTE {name}", GUILayout.MinHeight(30)))
                {
                    _selectedQTE = _drinks[i];
                    _isATemporaryQTE = false;
                }
            }
        }
        GUI.backgroundColor = Color.white;
    }

    #region Save
    void SaveQTEFile()
    {
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/QTE"))
        {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "QTE");
        }
        for (int i = 0; i < _selectedQTE.ListSubHandlers.Count; i++)
        {
            AssetDatabase.CreateAsset(_selectedQTE.ListSubHandlers[i], $"Assets/ScriptableObjects/QTE/QTEInput{_selectedQTE.Index}_{i}.asset");
        }
        AssetDatabase.CreateAsset(_selectedQTE, $"Assets/ScriptableObjects/QTE/QTE{_selectedQTE.Index}.asset");

        AssetDatabase.SaveAssets();
        _indexNewSequence++;
        LoadQTE();
    }

    void SaveQTEUnitInputFile(int indexQTE ,int indexUnit)
    {
        AssetDatabase.CreateAsset(_selectedQTE.ListSubHandlers[indexUnit], $"Assets/ScriptableObjects/QTE/QTEInput{indexQTE}_{indexUnit}.asset");
        AssetDatabase.SaveAssets();
    }
    #endregion

    #region Delete
    void RemoveUnitAtIndex(int indexUnit)
    {
        if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects") && AssetDatabase.IsValidFolder("Assets/ScriptableObjects/QTE"))
        {
            if (AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/QTE/QTEInput{_selectedQTE.Index}_{indexUnit}.asset"))
            {
                Debug.Log("File of unit has been deleted.");
            }else
            {
                Debug.Log("File not found");
            }
        }
        AssetDatabase.SaveAssets();
    }

    void RemoveSelectedSequence()
    {
        if (EditorUtility.DisplayDialog("Delete QTE",
                "Are you sure you want to delete this QTE ?", "Yes", "No"))
        {
            for (int i = _selectedQTE.ListSubHandlers.Count - 1; i >= 0; i--)
            {
                RemoveUnitAtIndex(i);
            }
            if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects") && AssetDatabase.IsValidFolder("Assets/ScriptableObjects/QTE"))
            {
                if (AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/QTE/QTEInput{_selectedQTE.Index}.asset"))
                {
                    Debug.Log("File of QTE has been deleted.");
                }
                else
                {
                    Debug.Log("File of QTE not found");
                }
            }
            AssetDatabase.SaveAssets();
            _selectedQTE = null;
        }
    }
    #endregion
    private void OnGUI()
    {
        //EditorGUIUtility.labelWidth = 100;
        if (_buttonInputOptions == null && ReInput.isReady)
        {
            _buttonInputOptions = new string[ReInput.mapping.Actions.Count];
            for (int i = 0; i < _buttonInputOptions.Length; i++)
            {
                _buttonInputOptions[ReInput.mapping.Actions[i].id] = ReInput.mapping.Actions[i].name;
            }
        }
        GUILayout.BeginHorizontal();
        DisplayViewLeft();
        EditorGUILayout.Space(2);
        DisplayMainView();
        EditorGUILayout.Space(2);
        GUILayout.EndHorizontal();
        SaveChangements();
    }

    private void DisplayViewLeft()
    {
        GUILayout.BeginVertical(GUILayout.Width(150));
        _scrollQTEListPosition = GUILayout.BeginScrollView(_scrollQTEListPosition);
        DrawSideBar();
        GUILayout.EndScrollView();

        if (GUILayout.Button("Add QTE", _styleButtonAddQTE, GUILayout.MinHeight(30)))
        {
            _selectedQTE = CreateInstance<QTESequence>();
            _selectedQTE.Index = _indexNewSequence;
            _isATemporaryQTE = true;
        }
        GUILayout.EndVertical();
    }
    private void DisplayMainView()
    {
        GUILayout.BeginVertical("GroupBox");
        EditorGUILayout.Space();
        if (_selectedQTE != null)
        {
            //GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sequence", GUILayout.Width(70));
            _selectedQTE.SequenceType = (InputsSequence)EditorGUILayout.EnumPopup(_selectedQTE.SequenceType);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete QTE", GUILayout.MaxWidth(100), GUILayout.MinHeight(30)))
            {
                RemoveSelectedSequence();
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status", GUILayout.Width(50));
            _selectedQTE.Status = (InputStatus)EditorGUILayout.EnumPopup(_selectedQTE.Status);
            if (_selectedQTE.Status == InputStatus.HOLD)
            {
                EditorGUILayout.Space(20, true);
                EditorGUILayout.LabelField("Duration", GUILayout.Width(70));
                _selectedQTE.DurationHold = EditorGUILayout.IntField(_selectedQTE.DurationHold);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Role", GUILayout.Width(50));
            _selectedQTE.PlayerRole = (PlayerRole)EditorGUILayout.EnumPopup(_selectedQTE.PlayerRole);
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (_selectedQTE.PlayerRole == PlayerRole.DJ)
            {
                _selectedQTE.ClientType = (CharacterColor)EditorGUILayout.EnumPopup("Client type", _selectedQTE.ClientType);
                _selectedQTE.Evilness = (Evilness)EditorGUILayout.EnumPopup("Evilness level", _selectedQTE.Evilness);
                _selectedQTE.QTELevel = EditorGUILayout.IntField("Level", _selectedQTE.QTELevel);
            }

            GUILayout.EndVertical();
            EditorGUILayout.Space();
            DrawListInputs();
            if (_isATemporaryQTE && GUILayout.Button("Save QTE"))
            {
                SaveQTEFile();
                _isATemporaryQTE = false;
            }
        }
        else // No QTE selected
        {
            GUILayout.Label("Select a QTE or add a new one");
        }
        GUILayout.EndVertical();
    }
    private void SaveChangements()
    {
        if (GUI.changed)
        {
            if (_selectedQTE != null)
            {
                _selectedQTE.DurationHold = Mathf.Max(0, _selectedQTE.DurationHold);
                _selectedQTE.QTELevel = Mathf.Max(1, _selectedQTE.QTELevel);
                EditorUtility.SetDirty(_selectedQTE);
                for (int i = 0; i < _selectedQTE.ListSubHandlers.Count; i++)
                {
                    EditorUtility.SetDirty(_selectedQTE.ListSubHandlers[i]);
                }
            }
            AssetDatabase.SaveAssets();
        }
    }
    private void DrawInput(UnitInput input)
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(EditorStyles.helpBox);

        if (_propertyName != null)
        {
            _serializedObject.Update();
            EditorGUILayout.PropertyField(_propertyName, true); // draw property with its children
            _serializedObject.ApplyModifiedProperties();
            bool isAxisInput = input.ActionIndex == RewiredConsts.Action.AXISX || 
                input.ActionIndex == RewiredConsts.Action.AXISY;
            if (isAxisInput)
            {
                input.UseRotation = EditorGUILayout.Toggle("Use rotation",input.UseRotation);
            }
            if (input.UseRotation)
            {
                input.NbTurns = EditorGUILayout.IntField("Number of turns", input.NbTurns);
            }
        }

        GUI.backgroundColor = Color.red;
        GUILayout.EndVertical();
        if (GUILayout.Button("X",GUILayout.Width(30), GUILayout.Height(30)) && EditorUtility.DisplayDialog("Delete input",
                "Are you sure you want to delete this input ?", "Yes", "No"))
        {
            RemoveUnitAtIndex(input.Index);
            _selectedQTE.ListSubHandlers.Remove(input);
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
    }

    private void DrawListInputs()
    {
        if (GUILayout.Button("Add an input", GUILayout.MinHeight(30)))
        {
            UnitInput unit = CreateInstance<UnitInput>();
            unit.Index = _selectedQTE.ListSubHandlers.Count;
            _selectedQTE.ListSubHandlers.Add(unit);
            if (!_isATemporaryQTE)
            {
                SaveQTEUnitInputFile(_selectedQTE.Index, unit.Index);
            }

        }
        if (_selectedQTE.ListSubHandlers.Count > 0)
        {
            _showListInputs = EditorGUILayout.Foldout(_showListInputs, "List of inputs");
            _scrollInputsPosition = GUILayout.BeginScrollView(_scrollInputsPosition);
            if (_showListInputs)
            {
                for (int i = 0; i < _selectedQTE.ListSubHandlers.Count; i++)
                {
                    _serializedObject = new SerializedObject(_selectedQTE.ListSubHandlers[i]);
                    _propertyName = _serializedObject.FindProperty(NAME_ACTION);
                    DrawInput(_selectedQTE.ListSubHandlers[i]);
                }
            }
            GUILayout.EndScrollView();
        }
    }
}
