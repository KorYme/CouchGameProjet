using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    [SerializeField] bool _inputsAreOnBeat = true;

    PlayerInputController _playerController;
    ITimingable _timingable;
    QTEHandlerEvents _events = new();

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
        _timingable = Globals.BeatManager;
        _checkInputThisBeat = new CheckHasInputThisBeat(_timingable);
        yield return new WaitUntil(() => Players.PlayersController[(int)_role] != null);
        _playerController = Players.PlayersController[(int)_role];
        _inputsQTE = new List<InputClass>()
        {
            _playerController.Action1,
            _playerController.Action2,
            _playerController.Action3,
            _playerController.Action4,
            _playerController.LT,
            _playerController.RT,
        };
    }

    #region QTEable
    public void RegisterQTEable(IQTEable QTEable)
    {
        _events?.RegisterQTEable(QTEable);
    }

    public void UnregisterQTEable(IQTEable QTEable)
    {
        _events?.UnregisterQTEable(QTEable);
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
        _indexInListSequences = 0;
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
        _currentListSequences.SetUpList();
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
        _currentQTESequence = _currentListSequences.GetSequence(_indexOfSequence);
        _inputsSucceeded = new bool[_currentQTESequence.ListSubHandlers.Count];
        _events?.CallOnQTEStarted();

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
        _currentListSequences.Clear();
    }
    #endregion
    public string GetQTEString()
    {
        if (_currentListSequences != null && _currentListSequences.Length > 0)
        {
            return _currentListSequences.ToString(_indexOfSequence, _indexInSequence);
        }
        return String.Empty;
    }

    public string GetCurrentInputString()
    {
        if (_currentListSequences != null && _currentListSequences.Length > 0)
        {
            return _currentListSequences.GetInputString(_indexOfSequence, _indexInSequence);
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
                            _events?.CallOnCorrectInput();
                        } else
                        {
                            _indexInSequence++;
                            _indexInListSequences++;
                            _events?.CallOnWrongInput();
                        }
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
                            _currentListSequences.SetInputSucceeded(i, _inputsSucceeded[i]);
                            if (_inputsSucceeded[i])// TO DO : CHANGE
                            {
                                _events?.CallOnCorrectInput(); 
                            }
                        }
                    } else if (_inputsSucceeded[i] != inputs[i].IsPerformed)
                    {
                        _inputsSucceeded[i] = inputs[i].IsPerformed;
                        _currentListSequences.SetInputSucceeded(i, _inputsSucceeded[i]);
                        if (_inputsSucceeded[i]) // TO DO : CHANGE
                        {
                            _events?.CallOnCorrectInput();
                        }
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

        if (_indexOfSequence < _currentListSequences.Length) // There is a next sequence
        {
            StartSequenceDependingOntype();
        }
        else // End of the list of sequences
        {
            _currentListSequences.Clear();
            _events?.CallOnQTEComplete();
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
