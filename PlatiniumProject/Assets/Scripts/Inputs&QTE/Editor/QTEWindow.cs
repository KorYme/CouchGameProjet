using DG.DOTweenEditor.UI;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class QTEWindow : EditorWindow
{
    List<QTESequence> _listQTE;
    QTESequence _selectedQTE = null;
    bool _isATemporaryQTE = false;
    int _indexNewSequence = 0;//Used to have different names for file sequences
    bool _showListInputs = true;
    Vector2 _scrollQTEListPosition;
    Vector2 _scrollInputsPosition;
    InputManager _rewiredInputManager;

    #region ListOptionsKeys
    //Used for the display of the rewired keys
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
        _rewiredInputManager = FindObjectOfType<InputManager>();
        if (_rewiredInputManager != null ) 
        { 
            _rewiredInputManager.runInEditMode = true; 
        }
        _styleButtonAddQTE = new GUIStyle()
        {
            fontStyle = FontStyle.Bold,
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(3,5,1,1),
            margin = new RectOffset(3,5,1,1),
            normal = new GUIStyleState() { background = Texture2D.whiteTexture}
        };
    }

    private void OrganizeAndRenameQTE()
    {
        for (int i = 0; i < _listQTE.Count; i++)
        {
            AssetDatabase.RenameAsset($"Assets/ScriptableObjects/QTE/QTE{_listQTE[i].Index}.asset", $"QTE{i}.asset");
            for (int j = 0; j < _listQTE[i].ListSubHandlers.Count; j++)
            {
                AssetDatabase.RenameAsset($"Assets/ScriptableObjects/QTE/QTEInput{_listQTE[i].Index}_{_listQTE[i].ListSubHandlers[j].Index}.asset", $"QTEInput{i}_{j}.asset");
                _listQTE[i].ListSubHandlers[j].Index = j;
            }
            _listQTE[i].Index = i;
        }
    }
    private void OnDestroy()
    {
        if (_rewiredInputManager != null)
            _rewiredInputManager.runInEditMode = false;
    }
    void LoadQTE()
    {
        if (_listQTE == null)
        {
            _listQTE = new List<QTESequence>();
        }
        else
        {
            _listQTE.Clear();
        }
        string[] fileGuids = AssetDatabase.FindAssets("t:" + typeof(QTESequence));
        int maxIndex = -1;
        if (fileGuids.Length > 0)
        {
            for (int i = 0; i < fileGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(fileGuids[i]);
                QTESequence sequence = AssetDatabase.LoadAssetAtPath<QTESequence>(assetPath);
                maxIndex = Mathf.Max(maxIndex, sequence.Index);
                _listQTE.Add(sequence);
            }
        }
        _listQTE.OrderBy(sequence => sequence.Index);
        _indexNewSequence = maxIndex + 1;
    }

    void DrawSideBar()
    {
        if (_listQTE != null)
        {
            for (int i = 0; i < _listQTE.Count; i++)
            {
                if (_selectedQTE != null && _listQTE.IndexOf(_selectedQTE) == i)
                {
                    GUI.backgroundColor = Color.green;
                } else
                {
                    GUI.backgroundColor = Color.white;
                }
                string name = Enum.GetName(typeof(PlayerRole), _listQTE[i].PlayerRole);
                if (GUILayout.Button($"QTE {i + 1} {name}", GUILayout.MinHeight(30)))
                {
                    _selectedQTE = _listQTE[i];
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
    void RemoveUnitAtIndex(int indexUnit, bool moveElements)
    {
        if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects") && AssetDatabase.IsValidFolder("Assets/ScriptableObjects/QTE"))
        {
            if (AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/QTE/QTEInput{_selectedQTE.Index}_{indexUnit}.asset"))
            {
                Debug.Log($"File of unit {_selectedQTE.Index}_{indexUnit} has been deleted.");
                if (moveElements)
                    RenameUnits(indexUnit);
            }
            else
            {
                Debug.Log("File not found");
            }
        }
        AssetDatabase.SaveAssets();
    }

    void RenameUnits(int indexUnit)
    {
        //TO DO change naming convention to guid
        for (int i = indexUnit + 1; i < _selectedQTE.ListSubHandlers.Count; i++)
        {
            Debug.Log($"Rename INPUT {_selectedQTE.Index}_{i} into {_selectedQTE.Index}_{i - 1}");
            AssetDatabase.RenameAsset($"Assets/ScriptableObjects/QTE/QTEInput{_selectedQTE.Index}_{i}.asset", $"QTEInput{_selectedQTE.Index}_{i-1}.asset");
            _selectedQTE.ListSubHandlers[i].Index--; 
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void RenameSequences(int indexSequence)
    {
        for (int i = indexSequence + 1; i < _listQTE.Count; i++)
        {
            Debug.Log($"Rename QTE {i} into {i - 1} with {_listQTE[i].ListSubHandlers.Count} {_listQTE.Count}");
            AssetDatabase.RenameAsset($"Assets/ScriptableObjects/QTE/QTE{i}.asset", $"QTE{i - 1}.asset");
            _listQTE[i].Index--;
            for (int j = 0; j < _listQTE[i].ListSubHandlers.Count; j++)
            {
                Debug.Log($"Rename INPUT {i}_{_listQTE[i].ListSubHandlers[j].Index} into {i - 1}_{_listQTE[i].ListSubHandlers[j].Index} TOTAL {_listQTE[i].ListSubHandlers.Count}");
                Debug.Log(AssetDatabase.RenameAsset($"Assets/ScriptableObjects/QTE/QTEInput{i}_{_listQTE[i].ListSubHandlers[j].Index}.asset", $"QTEInput{i - 1}_{_listQTE[i].ListSubHandlers[j].Index}.asset"));
            }
            //AssetDatabase.Refresh();
            Debug.Log($"{i} {_listQTE[i].Index}");
            EditorUtility.SetDirty(_listQTE[i]);
        }
    }

    void RemoveSelectedSequence()
    {
        if (EditorUtility.DisplayDialog("Delete QTE",
                "Are you sure you want to delete this QTE ?", "Yes", "No"))
        {
            for (int i = _selectedQTE.ListSubHandlers.Count - 1; i >= 0; i--)
            {
                RemoveUnitAtIndex(i,false);
            }
            if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects") && AssetDatabase.IsValidFolder("Assets/ScriptableObjects/QTE"))
            {
                int index = _selectedQTE.Index;
                Debug.Log($"Delete QTE {_selectedQTE.Index}");
                if (AssetDatabase.DeleteAsset($"Assets/ScriptableObjects/QTE/QTE{_selectedQTE.Index}.asset"))
                {
                    Debug.Log("File of QTE has been deleted.");
                    RenameSequences(index);
                }
                else
                {
                    Debug.Log("File of QTE not found");
                }
            } else
            {
                Debug.Log("File of QTE not found");
            }
            AssetDatabase.SaveAssets();
            _listQTE.Remove(_selectedQTE);
            _indexNewSequence = _listQTE.Last().Index;
            _selectedQTE = null;
        }
    }
    #endregion
    private void OnGUI()
    {
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
        if (_selectedQTE != null)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.Space();
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
            if (_selectedQTE != null)
            {
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Status", GUILayout.Width(50));
                    _selectedQTE.Status = (InputStatus)EditorGUILayout.EnumPopup(_selectedQTE.Status);
                    if (_selectedQTE.Status == InputStatus.LONG)
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
                _selectedQTE.Evilness = (Evilness)EditorGUILayout.EnumPopup("Evilness level", _selectedQTE.Evilness);

                if (_selectedQTE.PlayerRole == PlayerRole.DJ)
                {
                    _selectedQTE.ClientType = (CharacterColor)EditorGUILayout.EnumPopup("Client type", _selectedQTE.ClientType);
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
            } else
            {
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        else // No QTE selected
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Select a QTE or add a new one");
            GUILayout.EndVertical();
        }

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
            //Display ActionID in a list like in editor
            _serializedObject.Update();
            EditorGUILayout.PropertyField(_propertyName, true); // draw property with its children
            _serializedObject.ApplyModifiedProperties();
            if (_rewiredInputManager != null)
            {
                InputActionType rewiredInputType = ReInput.mapping.GetAction(input.ActionIndex).type;
                if (rewiredInputType == InputActionType.Axis && _selectedQTE.Status == InputStatus.SHORT)
                {
                    input.PositiveValue = EditorGUILayout.Toggle("Is a positive value",input.PositiveValue);
                }
            } else
            {
                EditorGUILayout.LabelField("Rewired inputs not loaded. Please run Rewired Input Manager in edit mode.", new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red } });

            }
        }

        GUI.backgroundColor = Color.red;
        GUILayout.EndVertical();
        if (GUILayout.Button("X",GUILayout.Width(30), GUILayout.Height(30)) && EditorUtility.DisplayDialog("Delete input",
                "Are you sure you want to delete this input ?", "Yes", "No"))
        {
            RemoveUnitAtIndex(input.Index,true);
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
