using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingLineBar : MonoBehaviour,IQTEable
{
    [SerializeField] QTEHandler _qteHandler;

    [SerializeField] TextMeshProUGUI _indexText;
    List<CharacterStateMachine> _waitingCharactersList;
    private DjUsher _djUsher;
    private PriestCalculator _priestCalculator;

    Vector3 Direction => Vector3.down;
    Vector3 Offset => Direction * 2.5f;

    public int NbCharactersWaiting { get => _waitingCharactersList.Count; }
    public bool IsInPause = true;

    private void Awake()
    {
        _djUsher = FindObjectOfType<DjUsher>();
        _priestCalculator = FindObjectOfType<PriestCalculator>();
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
         Debug.Log("DRINK FAIL");
         _qteHandler.DeleteCurrentCoroutine();
         _indexText.text = _qteHandler.GetQTEString();
         GetNextCharacter();
     }

    public void GetNextCharacter()
    {
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
                _waitingCharactersList[i].CharacterMove.MoveTo(transform.position + Direction * (i + 1) + Offset);
            }
            UpdatePositions();
            _waitingCharactersList[0].ChangeState(_waitingCharactersList[0].BarManAtBar);
        }
        _indexText.text = _qteHandler.GetQTEString();
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

    void IQTEable.OnQTEStarted()
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

    public void OnQTEWrongInput()
    {
    }
}
