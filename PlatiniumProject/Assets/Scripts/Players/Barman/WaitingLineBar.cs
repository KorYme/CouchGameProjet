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
    //[SerializeField] PlayerInput _playerInput;
    [SerializeField] QTEHandler _qteHandler;
    int _currentDrink = -1;

    [SerializeField] TextMeshProUGUI _indexText;
    //[SerializeField] DrinkList _drinkList;
    List<CharacterStateMachine> _waitingCharactersList;

    public int NbCharactersWaiting { get => _waitingCharactersList.Count; } 

    public int CurrentDrink { get => _currentDrink;}


    private void Start()
    {
        _waitingCharactersList = new List<CharacterStateMachine>();
        if (_qteHandler != null)
        {
            _qteHandler.OnSequenceComplete += OnDrinkComplete;
            _qteHandler.OnInputCorrect += OnInputCorrect;
        }
        _indexText.text = _qteHandler.DisplayQTE();
    }

    private void OnInputCorrect()
    {
        _indexText.text = _qteHandler.DisplayQTE();
    }

    void GetRandomDrink()
    {
        _qteHandler.GetRandomQTE();
    }

     void OnDrinkComplete()
    {
        _indexText.text = _qteHandler.DisplayQTE();
        CharacterStateMachine stateMachine = _waitingCharactersList[0];
        if (stateMachine != null)
        {
            stateMachine.CurrentSlot = stateMachine.AreaManager.DjBoard.GetRandomAvailableSlot();
            EditorGUIUtility.PingObject(stateMachine.CurrentSlot.gameObject);
            stateMachine.MoveToLocation = stateMachine.CurrentSlot.transform.position;
            
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
    }

     public void OnFailDrink()
     {
        _indexText.text = _qteHandler.DisplayQTE();
        _qteHandler.StopCoroutine();
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
     }

    public void AddToWaitingLine(CharacterStateMachine character)
    {
        _indexText.text = _qteHandler.DisplayQTE();
        character.CharacterMove.MoveToPosition(transform.position + Vector3.left * (_waitingCharactersList.Count + 1));
        if (_waitingCharactersList.Count == 0)
        {
            GetRandomDrink();
            character.ChangeState(character.BarManAtBar);
        }
        _waitingCharactersList.Add(character);
        
    }
}
