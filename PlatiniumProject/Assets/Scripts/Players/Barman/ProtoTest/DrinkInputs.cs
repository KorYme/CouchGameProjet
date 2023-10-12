using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

[CreateAssetMenu(menuName = "Bar/Create new drink inputs", fileName = "Drink")]
public class DrinkInputs : ScriptableObject
{
    [SerializeField] InputControl[] _inputs;
    [SerializeField] InputActionAsset _inputActions;

    public ReadOnlyArray<InputControl> Inputs;

    public InputActionAsset InputActions { get => _inputActions;}

    private void OnValidate()
    {
        Inputs = new ReadOnlyArray<InputControl>(_inputs);
    }
}
