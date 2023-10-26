using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class WaitingLineBar : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    int _currentDrink = -1;
    int _index;
    InputAction[] _inputBindings;

    [SerializeField] TextMeshProUGUI _indexText;
    [SerializeField] DrinkList _drinkList;
    List<CharacterStateMachine> _waitingCharactersList;

    public int NbCharactersWaiting { get => _waitingCharactersList.Count; } 

    public int CurrentDrink { get => _currentDrink;}

    public void Awake()
    {
        _index = 0;
    }
    private void Start()
    {
        _waitingCharactersList = new List<CharacterStateMachine>();
       _inputBindings = _playerInput.actions.ToArray();
        if (_currentDrink >= 0)
        {
            _indexText.text = _index + "/4 " + (Drink)_currentDrink;
        } else
        {
            _indexText.text = _index + "/4 ";
        }
    }

    void GetRandomDrink()
    {
        _currentDrink = Random.Range(0, 3);
    }

     void OnDrinkComplete()
    {
        CharacterStateMachine stateMachine = _waitingCharactersList[0];
        if (stateMachine != null)
        {
            Debug.Log(("ssdsd"));
            stateMachine.CurrentSlot = stateMachine.AreaManager.DjBoard.GetRandomAvailableSlot();
            stateMachine.MoveToLocation = stateMachine.CurrentSlot.transform.position;
            
            Debug.Log(stateMachine.MoveToLocation);
            stateMachine.NextState = stateMachine.DancingState;
            stateMachine.ChangeState(stateMachine.MoveToState);

        }
        _waitingCharactersList.RemoveAt(0);
        if (_waitingCharactersList.Count > 0)
        {
            GetRandomDrink();
            for (int i = 0;i < _waitingCharactersList.Count; i++)
            {
                _waitingCharactersList[i].CharacterMove.MoveToPosition(transform.position + Vector3.left * (i + 1));
            }
            _waitingCharactersList[0].ChangeState(_waitingCharactersList[0].BarManAtBar);
        } else
        {
            _currentDrink = -1;
        }
        if (_currentDrink >= 0)
        {
            _indexText.text = _index + "/4 " + (Drink)_currentDrink;
        }
        else
        {
            _indexText.text = _index + "/4 ";
        }
        _index = 0;
    }

     public void OnFailDrink()
     {
         _waitingCharactersList.RemoveAt(0);
         if (_waitingCharactersList.Count > 0)
         {
             GetRandomDrink();
             for (int i = 0;i < _waitingCharactersList.Count; i++)
             {
                 _waitingCharactersList[i].CharacterMove.MoveToPosition(transform.position + Vector3.left * (i + 1));
             }
             _waitingCharactersList[0].ChangeState(_waitingCharactersList[0].BarManAtBar);
         } else
         {
             _currentDrink = -1;
         }
         if (_currentDrink >= 0)
         {
             _indexText.text = _index + "/4 " + (Drink)_currentDrink;
         }
         else
         {
             _indexText.text = _index + "/4 ";
         }
         _index = 0;
     }
    public bool ComparePlayerInputToExpectedInput(string playerInput)
    {
        return playerInput == _inputBindings[0].controls[_drinkList.DrinksRecipe[_currentDrink][_index]].name;
    }

    public void AddToWaitingLine(CharacterStateMachine character)
    {
        character.CharacterMove.MoveToPosition(transform.position + Vector3.left * (_waitingCharactersList.Count + 1));
        if (_waitingCharactersList.Count == 0)
        {
            GetRandomDrink();
            character.ChangeState(character.BarManAtBar);
        }
        _waitingCharactersList.Add(character);
        if (_currentDrink >= 0)
        {
            _indexText.text = _index + "/4 " + (Drink)_currentDrink;
        }
        else
        {
            _indexText.text = _index + "/4 ";
        }
    }

    public void CheckInputFromLine(string control)
    {
        if (_currentDrink >= 0)
        {
            if (_index < _drinkList.DrinksRecipe[_currentDrink].Length)
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
                if (_currentDrink >= 0)
                {
                    _indexText.text = _index + "/4 " + (Drink)_currentDrink;
                }
                else
                {
                    _indexText.text = _index + "/4 ";
                }
            }
        }
    }
}
