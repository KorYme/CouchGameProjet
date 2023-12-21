using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WaitingLineBar : MonoBehaviour,IQTEable
{
    [SerializeField] QTEHandler _qteHandler;
    List<CharacterStateMachine> _waitingCharactersList; // Convert to queue
    private DjUsher _djUsher;
    private PriestCalculator _priestCalculator;
    BarmanQTEController _barmanController;
    [SerializeField] int _maxPlaces = 10;
    [SerializeField] private UnityEvent _onDrinkFinished;
    Vector3 Direction => Vector3.down;
    Vector3 Offset => Direction * 2.5f;

    public int NbCharactersWaiting { get => _waitingCharactersList.Count; }
    public bool IsInPause = true;
    public bool IsFull => _waitingCharactersList == null ? true : _waitingCharactersList.Count >= _maxPlaces;
    public float DurationValue => _qteHandler == null?0:_qteHandler.DurationValue;

    private void Awake()
    {
        _djUsher = FindObjectOfType<DjUsher>();
        _priestCalculator = FindObjectOfType<PriestCalculator>();
        _barmanController = FindObjectOfType<BarmanQTEController>();
        _waitingCharactersList = new List<CharacterStateMachine>();
        if (_qteHandler != null)
        {
            _qteHandler.RegisterListener(this);
        }
    }

    private void Start()
    {
        _djUsher.SetNextSlot();
    }

    private void OnDestroy()
    {
        if (_qteHandler != null)
        {
            _qteHandler.UnregisterListener(this);
        }
    }
    private void OnInputChange()
    {
        if (!IsInPause)
        {
            _barmanController.ModifyQTE(_qteHandler.GetQTESprites(),_qteHandler.InputsSucceeded);
        }
    }

     public void OnDrinkComplete()
    {
        //if(stateMachine.TypeData.Evilness)
        if (_waitingCharactersList.Count > 0)
        {
            CharacterStateMachine stateMachine = _waitingCharactersList[0];
            if (stateMachine != null)
            {
                _onDrinkFinished?.Invoke();
                if (stateMachine.TypeData.Evilness == Evilness.GOOD)
                {
                    stateMachine.CurrentSlot = _djUsher.NextSlot;
                    _djUsher.NextSlot.Occupant = stateMachine;
                    stateMachine.MoveToLocation = stateMachine.CurrentSlot.transform.position;
                    stateMachine.NextState = stateMachine.DancingState;
                    stateMachine.UseTp = true;
                    stateMachine.ChangeState(stateMachine.MoveToState);
                    _djUsher.SetNextSlot();
                }
                else
                {
                    stateMachine.ChangeState(stateMachine.DieState);
                }
            }
        }
        GetNextCharacter();
    }

     public void PriestForceEnterance()
     {
         CharacterStateMachine stateMachine = _waitingCharactersList[0];
         if (stateMachine != null)
         {
             stateMachine.CurrentSlot = _djUsher.NextSlot;
             _djUsher.NextSlot.Occupant = stateMachine;
             stateMachine.MoveToLocation = stateMachine.CurrentSlot.transform.position;
             stateMachine.NextState = stateMachine.DancingState;
             stateMachine.UseTp = true;
             stateMachine.ChangeState(stateMachine.MoveToState);
             _priestCalculator.AddPriestOnDanceFloor(stateMachine);
         }
         GetNextCharacter();
         _djUsher.SetNextSlot();
     }

     public void OnFailDrink()
     {
         _qteHandler.DeleteCurrentCoroutine();
         GetNextCharacter();
     }

    public void GetNextCharacter()
    {
        if (_waitingCharactersList.Count > 0)
            _waitingCharactersList.RemoveAt(0);

        if (_waitingCharactersList.Count > 0)
        {
            if (IsInPause)
            {
                _qteHandler.StoreNewQTE(_waitingCharactersList[0].TypeData);
            }
            else
            {
                _qteHandler.StartNewQTE(_waitingCharactersList[0].TypeData);
            }
            for (int i = 0;i < _waitingCharactersList.Count; i++)
            {
                _waitingCharactersList[i].CharacterMove.MoveTo(transform.position + Direction * (i + 1) + Offset);
            }
            UpdatePositions();
            _waitingCharactersList[0].ChangeState(_waitingCharactersList[0].BarManAtBar);
        } else if (!IsInPause)
        {
            _barmanController.EndQTE(_qteHandler.GetQTESprites(), _qteHandler.InputsSucceeded);
        }
    }

    void UpdatePositions()
    {
        for (int i = 0; i < _waitingCharactersList.Count; i++)
        {
            if (Vector3.Distance(_waitingCharactersList[i].transform.position,
                    transform.position + Direction * (i + 1) + Offset) > 0.1f)
            {
                _waitingCharactersList[i].CharacterMove.MoveTo(transform.position + Direction * (i + 1) + Offset);
            }
        }
    }
    public void AddToWaitingLine(CharacterStateMachine character)
    {
        character.CharacterMove.MoveTo(transform.position + Offset + Direction * (_waitingCharactersList.Count + 1));
        if (_waitingCharactersList.Count == 0) //If first person in line
        {
            if (IsInPause)
            {
                _qteHandler.StoreNewQTE(character.TypeData);
            }
            else
            {
                _qteHandler.StartNewQTE(character.TypeData);
            }
            character.ChangeState(character.BarManAtBar);
        }
        _waitingCharactersList.Add(character);
        UpdatePositions();
    }

    void IQTEable.OnQTEStarted()
    {
        _barmanController.StartQTE(_qteHandler.GetQTESprites(), _qteHandler.InputsSucceeded);
    }

    void IQTEable.OnQTEComplete()
    {
        OnDrinkComplete();
    }

    void IQTEable.OnQTECorrectInput()
    {
        OnInputChange();
    }

    public void PauseQTE(bool value) {
        IsInPause = value;
        if (_waitingCharactersList.Count > 0)
        {
            _qteHandler.PauseQTE(value);
        } else
        {
            OnInputChange();
        }
    }

    public void PauseQTEForDrop(bool value)
    {
        IsInPause = value;
        _qteHandler.PauseQTE(value);
        if (value)
        {
            _barmanController.EndQTE(_qteHandler.GetQTESprites(), _qteHandler.InputsSucceeded);
        } else
        {
            _barmanController.StartQTE(_qteHandler.GetQTESprites(), _qteHandler.InputsSucceeded);
        }
    }

    public void OnQTEWrongInput()
    {
        OnInputChange();
    }
    public void OnQTEMissedInput() {}
}
