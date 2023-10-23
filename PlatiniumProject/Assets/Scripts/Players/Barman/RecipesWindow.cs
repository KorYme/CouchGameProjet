using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RecipesWindow : EditorWindow
{
    List<QTESequence> _drinks;
    QTESequence _selectedDrink = null;
    QTESequence SOObject;
    int index = 0;
    #region ListOptions
    static readonly string[] _buttonInputOptions = { "A","B", "X", "Y"};
    static readonly string[] _inputTypeOptions = { "Press", "Hold"};
    static readonly string[] _inputSerieOptions = { "Simultaneous", "Sequential"};
    #endregion

    #region Selected
    int _selectedIndexInputButton = 0;
    int _selectedIndexInputType = 0;
    #endregion

    //bool showAngleRotation = false;
    float angleRotation = 0f;
    int durationHold = 2;

    [MenuItem("Tools/RecipesWindow")]
    static void InitWindow()
    {
        RecipesWindow window = GetWindow<RecipesWindow>();
        window.titleContent = new GUIContent("Tool recipes inputs");
        window.Show();
    }
    private void Awake()
    {
        LoadDrinks();
    }
    void LoadDrinks()
    {
        //EditorGUILayout.PropertyField
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
                if (GUILayout.Button($"Drink {i+1}", GUILayout.Width(200)))
                {
                    _selectedDrink = _drinks[i];
                }
            }
        }
        
    }

    void CreateDrinkFile()
    {

    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical("Drinks", GUILayout.ExpandWidth(false));
        DrawSideBar();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        _selectedIndexInputButton = EditorGUILayout.Popup("Button input", _selectedIndexInputButton, _buttonInputOptions);
        _selectedIndexInputType = EditorGUILayout.Popup("Input type", _selectedIndexInputType, _inputTypeOptions);
        //EditorGUILayout.Toggle("Show Button", showAngleRotation);
        if (_inputTypeOptions[_selectedIndexInputType] == "Hold")
        {
            durationHold = EditorGUILayout.IntField("Duration hold", durationHold);
        }
        angleRotation = EditorGUILayout.Slider("Angle rotation",angleRotation, 0, 360);
        if (GUILayout.Button("Save Drink")){
            SOObject = CreateInstance<QTESequence>();//new DrinkHandler();
            SOObject._sequenceType = InputsSequence.SEQUENCE;
            UnitInput unit = CreateInstance<UnitInput>(); //new UnitInput();
            unit.status = (InputStatus)_selectedIndexInputType;
            unit.actionIndex = _selectedIndexInputButton;
            SOObject._listSubHandlers.Add(unit);
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects"))
            {
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            }
            if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Drinks"))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Drinks");
            }
            AssetDatabase.CreateAsset(unit, $"Assets/ScriptableObjects/Drinks/DrinkIngredient{index}.asset");
            AssetDatabase.CreateAsset(SOObject, $"Assets/ScriptableObjects/Drinks/Drink{index}.asset");
            AssetDatabase.SaveAssets();
            index++;
            LoadDrinks();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}
