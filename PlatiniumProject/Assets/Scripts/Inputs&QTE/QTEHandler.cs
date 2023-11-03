using DG.Tweening;
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
    int _indexOfSequence = 0;
    QTESequence _currentQTESequence;
    QTEListSequences _listSequences;
    private Coroutine _coroutineQTE;
    List<IQTEable> _QTEables = new List<IQTEable>();
    [SerializeField] float _holdDuration = .5f;
    bool[] _inputsSucceeded;
    bool _isSequenceComplete = false;

    public int LengthInputs { get; private set; }

    private IEnumerator Start()
    {
        _listSequences = new QTEListSequences();
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
            _indexOfSequence = 0;
            StartSequenceDependingOntype();
        }
        else
        {
            StartNewQTE();
        }
    }
    public void StartNewQTE(CharacterData[] characters = null)
    {
        _indexOfSequence = 0;
        _listSequences.Clear();
        StoreNewQTE(characters);
        StartSequenceDependingOntype();
    }

    public void StoreNewQTE(CharacterData[] characters = null)
    {
        if (_coroutineQTE != null)
        {
            DeleteCurrentCoroutine();
        }
        if (characters == null)
        {
            _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role);
            _listSequences.AddSequence(_currentQTESequence);
        }
        else
        {
            int[] charactersCount = new int[Enum.GetNames(typeof(CLIENT_TYPE)).Length];
            foreach (CharacterData character in characters)
            {
                charactersCount[(int)character.clienType] += 1;
                //Debug.Log($"{character.clienType} {_role} {charactersCount[(int)character.clienType]}");
                _currentQTESequence = QTELoader.Instance.GetRandomQTE(character.clienType, charactersCount[(int)character.clienType], _role);
                _listSequences.AddSequence(_currentQTESequence);
            }
        }
        LengthInputs = _listSequences.TotalLengthInputs;
    }
    private void StartSequenceDependingOntype()
    {
        _indexInSequence = 0;
        _currentQTESequence = _listSequences.GetSequence(_indexOfSequence);
        _inputsSucceeded = new bool[_currentQTESequence.ListSubHandlers.Count];
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEStarted(_currentQTESequence);
        }
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
                    }
                    CallOnCorrectInput();
                }
            }
            yield return null;
        }

        ClearRoutine();
    }

    IEnumerator StartRoutineSimultaneous()
    {
        yield return new WaitUntil(() => _playerController != null);
        _isSequenceComplete = false;
        InputClass[] inputs = new InputClass[_currentQTESequence.ListSubHandlers.Count];
        for (int i = 0; i < inputs.Length;i++)
        {
            inputs[i] = _playerController.GetInputClassWithID(_currentQTESequence.ListSubHandlers[i].ActionIndex);
        }
        while (!_isSequenceComplete)
        {
            for (int i = 0; i < _currentQTESequence.ListSubHandlers.Count;i++)
            {
                if (_inputsSucceeded[i] != inputs[i].IsPerformed) //Press
                {
                    _inputsSucceeded[i] = inputs[i].IsPerformed;
                    CallOnCorrectInput();
                }
            }
            yield return null;
            _isSequenceComplete = CheckSequence();
        }

        ClearRoutine();
        
    }

    void ClearRoutine()
    {
        _indexOfSequence++;
        _currentQTESequence = null;
        _inputsSucceeded = null;
        
        if (_indexOfSequence < _listSequences.Length)
        {
            Debug.LogWarning("START NEXT SEQUENCE");
            StartSequenceDependingOntype();
        } else
        {
            foreach (IQTEable reciever in _QTEables)
            {
                reciever.OnQTEComplete();
            }
        }
    }
    void CallOnCorrectInput()
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
