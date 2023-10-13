using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaitingLineBar : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    int _currentDrink;
    int _index;
    InputAction[] _inputBindings;

    [SerializeField] TextMeshProUGUI _indexText;
    [SerializeField] DrinkList _drinkList;
    List<GameObject> _waitingCharactersList;
    public int NbCharactersWaiting { get => _waitingCharactersList.Count; } 

    public int CurrentDrink { get => _currentDrink;}

    public void Awake()
    {
        _index = 0;
    }
    private void Start()
    {
        _waitingCharactersList = new List<GameObject>();
       _inputBindings = _playerInput.actions.ToArray();
        _indexText.text = _index + "/4 " + (Drink)_currentDrink;
    }

    void GetRandomDrink()
    {
        _currentDrink = Random.Range(0, 3);
    }

    void OnDrinkComplete()
    {
        _waitingCharactersList.RemoveAt(0);
        GetRandomDrink();
        _index = 0;
    }
    public bool ComparePlayerInputToExpectedInput(string playerInput)
    {
        return playerInput == _inputBindings[0].controls[_drinkList.DrinksRecipe[_currentDrink][_index]].name;
    }

    public void AddToWaitingLine(GameObject character)
    {
        //character.GetComponent<CharacterStateMachine>
        _waitingCharactersList.Add(character);
    }

    public void CheckInputFromLine(string control)
    {
        if(_index < _drinkList.DrinksRecipe[_currentDrink].Length)
        {
            if (ComparePlayerInputToExpectedInput(control))
            {
                _index++;
                if (_index > 3)
                {
                    OnDrinkComplete();
                }
            }
            else
            {
                _index = 0;
            }
            _indexText.text = _index + "/4 "+ (Drink)_currentDrink;
        }
    }
}
