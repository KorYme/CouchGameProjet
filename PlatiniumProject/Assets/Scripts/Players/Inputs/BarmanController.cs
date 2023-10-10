using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class BarmanController : MonoBehaviour
{
    PlayerInput _playerInput;
    InputAction[] _inputBindings;
    int[] _currentInputs;
    [SerializeField] TextMeshProUGUI[] _texts;
    [SerializeField] TextMeshProUGUI _indexText;
    int _index;

    public void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _index = 0;
    }
    private void Start()
    {
        _inputBindings = _playerInput.actions.actionMaps[0].actions.ToArray();
        _currentInputs = new int[4];
        GetRandomInputsInQTEList();
        DisplayInputs();
    }

    public void GetRandomInputsInQTEList()
    {
        for (int i = 0; i < _currentInputs.Length; i++)
        {
            int index = Random.Range(0, _inputBindings[0].controls.Count);
            _currentInputs[i] = index;
        }
    }

    public void DisplayInputs()
    {
        for (int i = 0; i < _texts.Length; i++)
        {
            if (i < _currentInputs.Length)
            {
                _texts[i].text = _inputBindings[0].controls[_currentInputs[i]].displayName;
            }
        }
    }

    public bool ComparePlayerInputToExpectedInput(string playerInput) {
        return playerInput == _inputBindings[0].controls[_currentInputs[_index]].name;
    }
    public void HandleQTE(CallbackContext context) {
        if (context.performed && _index < _currentInputs.Length)
        {
            if (ComparePlayerInputToExpectedInput(context.control.name))
            {
                _index++;
            }
            else
            {
                _index = 0;
            }
            _indexText.text = _index + "/4";
        }
    }
}
