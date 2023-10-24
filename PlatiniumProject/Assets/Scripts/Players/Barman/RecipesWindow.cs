using Rewired;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RecipesWindow : EditorWindow
{
    List<QTESequence> _drinks;
    QTESequence _selectedQTE = null;
    bool _temporaryQTE = false;
    SerializedObject _serializedObject;
    SerializedProperty _propertyName;
    int index = 0;
    bool _showList = true;
    #region ListOptions
    private string[] _buttonInputOptions = null;
    const string NAME_ACTION = nameof(UnitInput.ActionIndex);
    #endregion
    #region Selected
    int _selectedIndexInputButton = 0;
    int _selectedIndexInputType = 0;
    #endregion
    GUIStyle style;

    [MenuItem("Tools/QTEWindow")]
    static void InitWindow()
    {
        RecipesWindow window = GetWindow<RecipesWindow>();
        window.titleContent = new GUIContent("Tool QTE inputs");
        window.Show();
    }
    private void Awake()
    {
        LoadQTE();
        style = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState() { background = Texture2D.whiteTexture }
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
        index = _drinks.Count;
    }

    void DrawSideBar()
    {
        if (_drinks != null)
        {
            for (int i = 0; i < _drinks.Count; i++)
            {
                if (GUILayout.Button($"QTE {i + 1}"))
                {
                    _selectedQTE = _drinks[i];
                    _temporaryQTE = false;
                }
            }
        }
        if (GUILayout.Button("Add QTE", style))
        {
            _selectedQTE = CreateInstance<QTESequence>();
            _temporaryQTE = true;
        }
    }

    void SaveQTEFile()
    {
        _selectedQTE.Index = index;
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
        index++;
        LoadQTE();
    }

    void SaveQTEUnitInputFile(int indexQTE ,int indexUnit)
    {
        AssetDatabase.CreateAsset(_selectedQTE.ListSubHandlers[indexUnit], $"Assets/ScriptableObjects/QTE/QTEInput{indexQTE}_{indexUnit}.asset");
        AssetDatabase.SaveAssets();
    }

    void RemoveUnitAtIndex( int indexUnit)
    {
        if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects") && AssetDatabase.IsValidFolder("Assets/ScriptableObjects/QTE"))
        {
            Debug.Log($"Assets/ScriptableObjects/QTE/QTE{_drinks.Count}_{indexUnit}.asset");
            AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/QTE/QTE{_drinks.Count}_{indexUnit}.asset");
        }
        
    }
    private void OnGUI()
    {
        if (_buttonInputOptions == null && ReInput.isReady)
        {
            _buttonInputOptions = new string[ReInput.mapping.Actions.Count];
            for (int i = 0; i < _buttonInputOptions.Length; i++)
            {
                _buttonInputOptions[ReInput.mapping.Actions[i].id] = ReInput.mapping.Actions[i].name;
            }
        }
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        DrawSideBar();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();

        if (_selectedQTE != null)
        {
            GUILayout.BeginHorizontal();
            _selectedQTE.Difficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty ", _selectedQTE.Difficulty);
            _selectedQTE.SequenceType = (InputsSequence) EditorGUILayout.EnumPopup("Sequence ", _selectedQTE.SequenceType);
            _selectedQTE.PlayerRole = (PlayerRole) EditorGUILayout.EnumPopup("Role ", _selectedQTE.PlayerRole);
            GUILayout.EndHorizontal();

            DrawListInputs();
            if (_temporaryQTE && GUILayout.Button("Save QTE"))
            {
                SaveQTEFile();
                _temporaryQTE = false;
            }
        }
        else
        {
            GUILayout.Label("Select a QTE or add a new one");
            GUILayout.Label("PLEASE FOR THE INCOMING BUILD CHOOSE SEQUENCE AND PRESS");
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
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
            input.IsInputPositive = EditorGUILayout.Toggle("Input has positive value",input.IsInputPositive);
        }
        input.Status = (InputStatus) EditorGUILayout.EnumPopup("Input type", input.Status);
        if (input.Status == InputStatus.HOLD)
        {
            input.NbBeatHoldDuration = EditorGUILayout.IntField("Duration hold", input.NbBeatHoldDuration);
        }

        GUILayout.EndVertical();
        if (GUILayout.Button("Delete"))
        {
            RemoveUnitAtIndex(input.Index);
            _selectedQTE.ListSubHandlers.Remove(input);
        }
        GUILayout.EndHorizontal();
    }

    private void DrawListInputs()
    {
        if (_selectedQTE.ListSubHandlers.Count > 0)
        {
            _showList = EditorGUILayout.Foldout(_showList, "List of inputs");
            if (_showList)
            {
                for (int i = 0; i < _selectedQTE.ListSubHandlers.Count; i++)
                {
                    _serializedObject = new SerializedObject(_selectedQTE.ListSubHandlers[i]);
                    _propertyName = _serializedObject.FindProperty(NAME_ACTION);
                    DrawInput(_selectedQTE.ListSubHandlers[i]);
                    //Debug.Log($"i {i} index {_selectedQTE.Index} {_selectedQTE.ListSubHandlers[i].Index}");
                }
            }
        }
        
        if (GUILayout.Button("Add an input"))
        {
            
            UnitInput unit = CreateInstance<UnitInput>();
            unit.Index = _selectedQTE.ListSubHandlers.Count;
            _selectedQTE.ListSubHandlers.Add(unit);
            _serializedObject = new SerializedObject(unit);
            _propertyName = _serializedObject.FindProperty(NAME_ACTION);
            if (!_temporaryQTE)
            {
                SaveQTEUnitInputFile(_selectedQTE.Index, unit.Index);
            }
            
        }
    }
}
