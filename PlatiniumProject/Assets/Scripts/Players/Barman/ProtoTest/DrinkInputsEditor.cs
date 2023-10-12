using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR

[CustomEditor(typeof(DrinkInputs))]
public class DrinkInputsEditor : Editor
{
    private DrinkInputs _drinkInputsRef;
    private int selected = 0;
    private string[] options;
    InputControl[] controls;
    private void OnEnable()
    {
        _drinkInputsRef = (DrinkInputs) target;
        controls = _drinkInputsRef.InputActions.actionMaps[0].actions[0].controls.ToArray();
        options = new string[controls.Length];
    }

    private void OnValidate()
    {
        if (controls != null)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                options[i] = controls[i].name;
            }
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        selected = EditorGUILayout.Popup("Label", selected, options);
    }
}
#endif