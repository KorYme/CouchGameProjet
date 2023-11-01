using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        _inputsSucceeded = new bool[_currentQTESequence.ListSubHandlers.Count];
        switch (_currentQTESequence.SequenceType)
        {
            case InputsSequence.SEQUENCE:
                _coroutineQTE = StartCoroutine(StartRoutineSequence());
                break;
            case InputsSequence.SIMULTANEOUS:
                _coroutineQTE = StartCoroutine(StartRoutineSimultaneous());
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
                    if (_inputsSucceeded != null && _inputsSucceeded[input.Index])
                    {
                        str.Append("<color=\"green\">");
                    } else
                    {
                        str.Append("<color=\"red\">");
                    }
                    str.Append(action.descriptiveName);
                    str.Append("</color> ");
                } else
                {
                    str.Append("(Not found) ");
                }
                if (_currentQTESequence.SequenceType == InputsSequence.SIMULTANEOUS && input.Index != _currentQTESequence.ListSubHandlers.Count - 1)
                {
                    str.Append("+ ");
                }
            }
            
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
    #region Routines
    IEnumerator StartRoutineSequence()
    {
        yield return new WaitUntil(() => _playerController != null);
        UnitInput input = _currentQTESequence.ListSubHandlers[_indexInSequence];
        while (_indexInSequence < _currentQTESequence.ListSubHandlers.Count)
        {
            if ((Globals.BeatTimer?.IsInsideBeat ?? true) || true) //A MODIFIER
            {
                if (CheckInput(input))
                {
                    _inputsSucceeded[_indexInSequence] = true;
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
        _inputsSucceeded = null;
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEComplete();
        }
    }

    IEnumerator StartRoutineSimultaneous()
    {
        yield return new WaitUntil(() => _playerController != null);
        _isSequenceComplete = false;
        while (!_isSequenceComplete)
        {
            foreach (UnitInput input in _currentQTESequence.ListSubHandlers)
            {
                InputClass inputClass = _playerController.GetInputClassWithID(input.ActionIndex);
                if (_inputsSucceeded[input.Index] != inputClass.IsPerformed) //Press
                {
                    _inputsSucceeded[input.Index] = inputClass.IsPerformed;
                    ChangeStateInputClass();
                }
            }
            yield return null;
            _isSequenceComplete = CheckSequence();
        }
        
        _currentQTESequence = null;
        _inputsSucceeded = null;
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEComplete();
        }
    }

    void ChangeStateInputClass()
    {
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTECorrectInput();
        }
    }

    private bool CheckSequence()
    {
        bool res = true;
        int i = 0;
        while (res && i < _inputsSucceeded.Length) 
        {
            res = _inputsSucceeded[i];
            i++;
        }
        
        return res;
    }
    #endregion
}
