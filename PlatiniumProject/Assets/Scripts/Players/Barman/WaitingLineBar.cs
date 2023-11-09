using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingLineBar : MonoBehaviour,IQTEable
{
    [SerializeField] QTEHandler _qteHandler;
    List<CharacterStateMachine> _waitingCharactersList; // Convert to queue
    private DjUsher _djUsher;
    private PriestCalculator _priestCalculator;
    BarmanQTEController _barmanController;

    Vector3 Direction => Vector3.down;
    Vector3 Offset => Direction * 2.5f;

    public int NbCharactersWaiting { get => _waitingCharactersList.Count; }
    public bool IsInPause = true;

    private void Awake()
    {
        _djUsher = FindObjectOfType<DjUsher>();
        _priestCalculator = FindObjectOfType<PriestCalculator>();
        _barmanController = FindObjectOfType<BarmanQTEController>();
        _waitingCharactersList = new List<CharacterStateMachine>();
    }

    private void Start()
    {
        if (_qteHandler != null)
        {
            _qteHandler.RegisterQTEable(this);
        }
        _djUsher.SetNextSlot();
    }

    private void OnDestroy()
    {
        if (_qteHandler != null)
        {
            _qteHandler.UnregisterQTEable(this);
        }
    }
    private void OnInputChange()
    {
        if (!IsInPause)
        {
            _barmanController.ModifyQTE(_qteHandler.GetQTEString());
        }

    }

     void OnDrinkComplete()
    {
        CharacterStateMachine stateMachine = _waitingCharactersList[0];
        if (stateMachine != null)
        {
            stateMachine.CurrentSlot = _djUsher.NextSlot;
            stateMachine.MoveToLocation = stateMachine.CurrentSlot.transform.position;
            stateMachine.NextState = stateMachine.DancingState;
            stateMachine.ChangeState(stateMachine.MoveToState);

            if (stateMachine.TypeData.Evilness == Evilness.EVIL)
            {
                _priestCalculator.PriestOnDanceFloor(stateMachine);
            }
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
            _barmanController.EndQTE(_qteHandler.GetQTEString());
        }
    }

    void UpdatePositions()
    {
        for (int i = 0; i < _waitingCharactersList.Count; i++)
        {
            _waitingCharactersList[i].CharacterMove.MoveTo(transform.position + Direction * (i + 1) + Offset);
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
    }

    void IQTEable.OnQTEStarted()
    {
        _barmanController.StartQTE(_qteHandler.GetQTEString());
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
        }
    }

    public void OnQTEWrongInput()
    {
        OnInputChange();
    }
}
