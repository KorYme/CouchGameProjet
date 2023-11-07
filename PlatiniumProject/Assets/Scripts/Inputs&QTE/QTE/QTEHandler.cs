using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField, Range(0f, 1f)] float _inputDistanceRotationQTE = .4f;

    PlayerInputController _playerController;
    ITimingable _timingable;
    //List of listener on QTEHandler
    List<IQTEable> _QTEables = new List<IQTEable>();

    private Coroutine _coroutineQTE;
    QTEListSequences _currentListSequences;
    QTESequence _currentQTESequence;
    bool[] _inputsSucceeded;
    
    int _indexOfSequence = 0;
    int _indexInSequence = 0;
    int _indexInListSequences = 0;
    bool _isSequenceComplete = false;
    int _durationHold = 0;
    CheckHasInputThisBeat _checkInputThisBeat;
    List<InputClass> _inputsQTE;

    public int LengthInputs { get; private set; }

    private IEnumerator Start()
    {
        _currentListSequences = new QTEListSequences();
        _timingable = Globals.BeatTimer;
        _checkInputThisBeat = new CheckHasInputThisBeat(_timingable);
        yield return new WaitUntil(() => Players.PlayersController[(int)_role] != null);
        _playerController = Players.PlayersController[(int)_role];
        _inputsQTE = new List<InputClass>()
        {
            _playerController.Action1,
            _playerController.Action2,
            _playerController.Action3,
            _playerController.Action4,
            _playerController.RB,
            _playerController.RT,
            _playerController.RT,
            //_playerController.RightJoystick            
        };
    }

    #region QTEable
    public void RegisterQTEable(IQTEable QTEable)
    {
        _QTEables.Add(QTEable);
    }

    public void UnregisterQTEable(IQTEable QTEable)
    {
        _QTEables.Remove(QTEable);
    }
    #endregion

    #region SetUpQTE
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
    public void StartNewQTE(CharacterTypeData[] characters = null)
    {
        _indexOfSequence = 0;
        _currentListSequences.Clear();
        StoreNewQTE(characters);
        StartSequenceDependingOntype();
    }

    public void StoreNewQTE(CharacterTypeData[] characters = null)
    {
        if (_coroutineQTE != null)
        {
            DeleteCurrentCoroutine();
        }
        if (characters == null) // Characters type not needed
        {
            _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role);
            _currentListSequences.AddSequence(_currentQTESequence);
        }
        else
        {
            int[] charactersCount = new int[Enum.GetNames(typeof(CharacterColor)).Length];
            int indexEvil = 0;
            int nbEvilCharacters = GetNbOfEvilCharacters(characters);
            foreach (CharacterTypeData character in characters)
            {
                if (character.Evilness == Evilness.GOOD)
                {
                    charactersCount[(int)character.ClientType] += 1; // SI GENTIL SINON + 0
                    _currentQTESequence = QTELoader.Instance.GetRandomQTE(character.ClientType, character.Evilness, charactersCount[(int)character.ClientType] + nbEvilCharacters, _role);

                } else
                {
                    indexEvil++;
                    _currentQTESequence = QTELoader.Instance.GetRandomQTE(character.ClientType, character.Evilness, indexEvil, _role);
                }
                _currentListSequences.AddSequence(_currentQTESequence);
            }
        }
        LengthInputs = _currentListSequences.TotalLengthInputs;
    }

    public int GetNbOfEvilCharacters(CharacterTypeData[] characters)
    {
        int total = 0;
        foreach (CharacterTypeData character in characters)
        {
            if (character.Evilness == Evilness.EVIL) total++;
        }
        return total;
    }
    private void StartSequenceDependingOntype()
    {
        _indexInSequence = 0;
        _indexInListSequences = 0;
        _currentQTESequence = _currentListSequences.GetSequence(_indexOfSequence);
        _inputsSucceeded = new bool[_currentQTESequence.ListSubHandlers.Count];
        _currentListSequences.SetUpList();
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
    #endregion
    public string GetQTEString()
    {
        if (_currentListSequences != null)
        {
            /*StringBuilder str = new StringBuilder();
            UnitInput input;
            for (int i = 0;i < _currentQTESequence.ListSubHandlers.Count ;i++)
            {
                input = _currentQTESequence.ListSubHandlers[i];

                InputAction action = ReInput.mapping.GetAction(input.ActionIndex);
                if (action != null)
                {
                    if (_inputsSucceeded != null && _inputsSucceeded[i])
                    {
                        str.Append("<color=\"green\">");
                    } else if (_indexInSequence == i && _currentQTESequence.SequenceType == InputsSequence.SEQUENCE)
                    {
                        str.Append("<color=\"orange\">");
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
            }*/

            return _currentListSequences.ToString(_indexOfSequence, _indexInSequence);
        }
        return String.Empty;
    }

    void CheckInputs(int expectedActionID)
    {
        if (!_checkInputThisBeat.HadInputThisBeat)
        {
            InputBool inputBool;
            foreach (InputClass inputRef in _inputsQTE)
            {
                if (!_checkInputThisBeat.HadInputThisBeat && inputRef != null)
                {
                    inputBool = inputRef as InputBool; //TO DO : change for IsJustPerformed or smt in inputclass
                    if (inputBool != null && inputBool.IsJustPressed)
                    {
                        _checkInputThisBeat.ChangeHadInputThisBeat();
                        if (inputRef.ActionID == expectedActionID)
                        {
                            _inputsSucceeded[_indexInSequence] = true;
                            _currentListSequences.SetInputSucceeded(_indexInListSequences, true);
                            _indexInSequence++;
                            _indexInListSequences++;
                            CallOnCorrectInput();
                        } else
                        {
                            _indexInSequence++;
                            _indexInListSequences++;
                            CallOnWrongInput();
                        }
                        //Debug.Log($"index {_indexInSequence} {expectedActionID}");
                    }
                }
            }
        }
    }
    #region Routines
    IEnumerator StartRoutineSequence()
    {
        yield return new WaitUntil(() => _playerController != null);
        UnitInput correctInput = _currentQTESequence.ListSubHandlers[_indexInSequence];
        Debug.Log($"index {_indexInSequence} {correctInput.ActionIndex}");
        while (_indexInSequence < _currentQTESequence.ListSubHandlers.Count)
        {
            if ((_timingable?.IsInsideBeatWindow ?? true) || true)
            {
                CheckInputs(_currentQTESequence.ListSubHandlers[_indexInSequence].ActionIndex);
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
            inputs[i] = _playerController.GetInputClassWithID(_currentQTESequence.ListSubHandlers[i].ActionIndex,true);
        }
        _durationHold = 0;
        while ((!_isSequenceComplete &&_currentQTESequence.Status == InputStatus.PRESS) || _durationHold < (_currentQTESequence.DurationHold * _timingable.BeatDurationInMilliseconds))
        {
            for (int i = 0; i < _currentQTESequence.ListSubHandlers.Count; i++)
            {
                if (inputs[i] != null)
                {
                    if (_currentQTESequence.ListSubHandlers[i].UseRotation)
                    {
                        InputVector2 vectAxis = inputs[i] as InputVector2;
                        if (vectAxis != null)
                        {
                            //Debug.Log($"DeltaValue {vectAxis.DeltaValue}");
                            _inputsSucceeded[i] = vectAxis.IsMoving;
                            CallOnCorrectInput();
                        }
                    } else if (_inputsSucceeded[i] != inputs[i].IsPerformed)
                    {
                        _inputsSucceeded[i] = inputs[i].IsPerformed;
                        CallOnCorrectInput();
                    }
                }
            }
            yield return null;
            _isSequenceComplete = CheckSequence();
            if (_isSequenceComplete && _currentQTESequence.Status == InputStatus.HOLD)
            {
                _durationHold += (int)(Time.deltaTime * 1000);
            }
        }
        ClearRoutine();
    }

    void ClearRoutine()
    {
        _indexOfSequence++;
        _currentQTESequence = null;
        _inputsSucceeded = null;
        
        if (_indexOfSequence < _currentListSequences.Length)
        {
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
        Debug.LogWarning("CORRECT INPUT");
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTECorrectInput();
        }
    }

    void CallOnWrongInput()
    {
        Debug.LogWarning("WRONG INPUT");
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEWrongInput();
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
