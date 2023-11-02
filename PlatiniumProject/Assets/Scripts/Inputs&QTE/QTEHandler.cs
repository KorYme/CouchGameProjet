using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    PlayerInputController _playerController;
    int _indexInSequence = 0;
    QTESequence _currentQTESequence;
    private Coroutine _coroutineQTE;
    List<IQTEable> _QTEables = new List<IQTEable>();
    [SerializeField] float _holdDuration = .5f;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Players.PlayersController[(int)_role] != null);
        _playerController = Players.PlayersController[(int)_role];
    }

    public void RegisterQTEable(IQTEable QTEable)
    {
        _QTEables.Add(QTEable);
    }

    public void UnregisterQTEable(IQTEable QTEable)
    {
        _QTEables.Remove(QTEable);
    }

    public void StartQTE()
    {
        if (_currentQTESequence != null)
        {
            _indexInSequence = 0;
            StartSequenceDependingOntype();
        }
        else
        {
            StartNewQTE();
        }
    }
    public void StartNewQTE()
    {
        StoreNewQTE();
        _indexInSequence = 0;
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEStarted(_currentQTESequence);
        }
        StartSequenceDependingOntype();
    }
    public void StoreNewQTE()
    {
        if (_coroutineQTE != null)
        {
            DeleteCurrentCoroutine();
        }
        _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role);
    }
    private void StartSequenceDependingOntype()
    {
        switch (_currentQTESequence.SequenceType)
        {
            case InputsSequence.SEQUENCE:
                _coroutineQTE = StartCoroutine(StartRoutineSequence());
                break;
            case InputsSequence.SIMULTANEOUS:
                //TO DO
                break;
        }
    }
    public void PauseQTE(bool value)
    {
        if (value)
        {
            StopCurrentCoroutine();
        } else
        {
            StartQTE();
        }
    }

    public void StopCurrentCoroutine()
    {
        if (_coroutineQTE != null)
        {
            StopCoroutine(_coroutineQTE);
            _coroutineQTE = null;
        }
    }

    public void DeleteCurrentCoroutine()
    {
        StopCurrentCoroutine();
        _currentQTESequence = null;
    }

    public string GetQTEString()
    {
        if (_currentQTESequence != null)
        {
            StringBuilder str = new StringBuilder();
            foreach (UnitInput input in _currentQTESequence.ListSubHandlers)
            {
                InputAction action = ReInput.mapping.GetAction(input.ActionIndex);
                if (action != null)
                {
                    str.Append(action.descriptiveName);
                    str.Append(" ");
                }
            }
            str.Append(_indexInSequence.ToString());
            str.Append(" ");
            str.Append(_currentQTESequence.ListSubHandlers.Count);
            return str.ToString();
        }
        return String.Empty;
    }

    bool CheckInput(UnitInput input)
    {
        if (_playerController == null) 
        {
            return false;
        }
        bool isInputCorrect = false;
        switch (input.Status)
        {
            case InputStatus.PRESS:
                InputBool inputBool = _playerController.GetInputClassWithID(input.ActionIndex) as InputBool;
                if (inputBool != null)
                {
                    isInputCorrect = inputBool.IsJustPressed;
                }
                break;
            case InputStatus.HOLD:
                InputClass inputClass = _playerController.GetInputClassWithID(input.ActionIndex);
                isInputCorrect = inputClass.IsPerformed && inputClass.InputDuration > _holdDuration;
                break;
        }
        return isInputCorrect;
    }

    IEnumerator StartRoutineSequence()
    {
        UnitInput input = _currentQTESequence.ListSubHandlers[_indexInSequence];
        while (_indexInSequence < _currentQTESequence.ListSubHandlers.Count)
        {
            if ((Globals.BeatTimer?.IsInsideBeat ?? true) || true) //A MODIFIER
            {
                if (CheckInput(input))
                {
                    _indexInSequence++;

                    if (_indexInSequence < _currentQTESequence.ListSubHandlers.Count) //Sequence finished
                    {
                        input = _currentQTESequence.ListSubHandlers[_indexInSequence];
                        foreach (IQTEable reciever in _QTEables)
                        {
                            reciever.OnQTECorrectInput();
                        }
                    }
                }
            }
            yield return null;
        }
        
        _currentQTESequence = null;
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEComplete();
        }
    }
}
