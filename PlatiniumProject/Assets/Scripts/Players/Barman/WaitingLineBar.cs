using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class WaitingLineBar : MonoBehaviour,IQTEable
{
    [SerializeField] QTEHandler _qteHandler;

    [SerializeField] TextMeshProUGUI _indexText;
    List<CharacterStateMachine> _waitingCharactersList;

    public int NbCharactersWaiting { get => _waitingCharactersList.Count; }
    public bool IsInPause = true;

    private void Awake()
    {
        _waitingCharactersList = new List<CharacterStateMachine>();
    }

    private void Start()
    {
        if (_qteHandler != null)
        {
            _qteHandler.RegisterQTEable(this);
        }
    }

    private void OnDestroy()
    {
        if (_qteHandler != null)
        {
            _qteHandler.UnregisterQTEable(this);
        }
    }
    private void OnInputCorrect()
    {
        _indexText.text = _qteHandler.GetQTEString();
    }

     void OnDrinkComplete()
    {
        _indexText.text = string.Empty;
        CharacterStateMachine stateMachine = _waitingCharactersList[0];
        if (stateMachine != null)
        {
            stateMachine.CurrentSlot = stateMachine.AreaManager.DjBoard.GetRandomAvailableSlot();
            stateMachine.MoveToLocation = stateMachine.CurrentSlot.transform.position;
            
            stateMachine.NextState = stateMachine.DancingState;
            stateMachine.ChangeState(stateMachine.MoveToState);

        }
        _waitingCharactersList.RemoveAt(0);
        if (_waitingCharactersList.Count > 0)
        {
            if (IsInPause)
            {
                _qteHandler.StoreNewQTE();
            }
            else
            {
                _qteHandler.StartNewQTE();
            }
            for (int i = 0;i < _waitingCharactersList.Count; i++)
            {
                _waitingCharactersList[i].CharacterMove.MoveToPosition(transform.position + Vector3.left * (i + 1));
            }
            _waitingCharactersList[0].ChangeState(_waitingCharactersList[0].BarManAtBar);
            _indexText.text = _qteHandler.GetQTEString();
        }
    }

     public void OnFailDrink()
     {
        _indexText.text = _qteHandler.GetQTEString();
        _qteHandler.DeleteCurrentCoroutine();
         _waitingCharactersList.RemoveAt(0);
         if (_waitingCharactersList.Count > 0)
         {
            if (IsInPause)
            {
                _qteHandler.StoreNewQTE();
            } else
            {
                _qteHandler.StartNewQTE();
            }
            for (int i = 0;i < _waitingCharactersList.Count; i++)
             {
                 _waitingCharactersList[i].CharacterMove.MoveTo(transform.position + Vector3.left * (i + 1));
             }
             _waitingCharactersList[0].ChangeState(_waitingCharactersList[0].BarManAtBar);
         }
     }

    public void AddToWaitingLine(CharacterStateMachine character)
    {
        character.CharacterMove.MoveTo(transform.position + Vector3.left * (_waitingCharactersList.Count + 1));
        if (_waitingCharactersList.Count == 0) //If first person in line
        {
            if (IsInPause)
            {
                _qteHandler.StoreNewQTE();
            }
            else
            {
                _qteHandler.StartNewQTE();
            }
            character.ChangeState(character.BarManAtBar);
        }
        _waitingCharactersList.Add(character);
        _indexText.text = _qteHandler.GetQTEString();
    }

    void IQTEable.OnQTEStarted(QTESequence sequence)
    {
        _indexText.text = _qteHandler.GetQTEString();
    }

    void IQTEable.OnQTEComplete()
    {
        OnDrinkComplete();
    }

    void IQTEable.OnQTECorrectInput()
    {
        OnInputCorrect();
    }

    public void PauseQTE(bool value) {
        IsInPause = value;
        if (_waitingCharactersList.Count > 0)
        {
            _qteHandler.PauseQTE(value);
        }
    }
}
