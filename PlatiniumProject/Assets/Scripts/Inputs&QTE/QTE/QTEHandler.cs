using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QTE_STATE
{
    NEED_PRESS,
    NEED_RELEASE,
    IS_PRESSED
}

public class QTEHandler : MonoBehaviour
{
    [SerializeField] float thresholdDirectionJoystick = 0.9f;
    [SerializeField] PlayerRole _role;
    [SerializeField] bool _inputsAreOnBeat = true;
    [SerializeField] bool _includeLeftJoystick = true;
    [SerializeField] bool _includeRightJoystick = true;

    protected PlayerInputController _playerController;
    ITimingable _timingable;
    protected QTEHandlerEvents _events = new();

    protected Coroutine _coroutineQTE;
    protected QTEListSequences _currentListSequences;
    protected QTESequence _currentQTESequence;
    protected QTE_STATE[] _inputsSucceeded;

    protected int _indexOfSequence = 0;
    protected int _indexInSequence = 0;
    protected int _indexInListSequences = 0;
    protected bool _isSequenceComplete = false;
    protected int _durationHold = 0;
    CheckHasInputThisBeat _checkInputThisBeat;
    List<InputClass> _inputsQTE;


    public int LengthInputs { get; private set; }
    private void Awake()
    {
        _currentListSequences = new QTEListSequences();
    }

    private IEnumerator Start()
    {
        _timingable = Globals.BeatManager;
        _checkInputThisBeat = new CheckHasInputThisBeat(_timingable);
        yield return new WaitUntil(() => Players.PlayersController[(int)_role] != null);
        _playerController = Players.PlayersController[(int)_role];        _inputsQTE = new List<InputClass>();    }
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
    public void StartQTE()    {        if (_currentQTESequence != null)        {            _indexOfSequence = 0;            StartSequenceDependingOntype();        }     }
    public void StartNewQTE(CharacterTypeData[] characters)
    {
        _indexOfSequence = 0;        _indexInListSequences = 0;        StoreNewQTE(characters);        StartSequenceDependingOntype();    }

    public void StartNewQTE(CharacterTypeData character)
    {
        _indexOfSequence = 0;        _indexInListSequences = 0;        StoreNewQTE(character);        StartSequenceDependingOntype();    }

    public void StoreNewQTE(CharacterTypeData character)    {        if (_coroutineQTE != null)        {            DeleteCurrentCoroutine();        }        _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role, character.Evilness);        _currentListSequences.AddSequence(_currentQTESequence);        _currentListSequences.SetUpList();        LengthInputs = _currentListSequences.TotalLengthInputs;    }

    public void StoreNewQTE(CharacterTypeData[] characters)    {        if (_coroutineQTE != null)        {            DeleteCurrentCoroutine();        }        int[] charactersCount = new int[Enum.GetNames(typeof(CharacterColor)).Length];        int indexEvil = 0;        int nbEvilCharacters = GetNbOfEvilCharacters(characters);        foreach (CharacterTypeData character in characters)        {            if (character.Evilness == Evilness.GOOD)            {                charactersCount[(int)character.ClientType] += 1; // SI GENTIL SINON + 0                _currentQTESequence = QTELoader.Instance.GetRandomQTE(character.ClientType, character.Evilness, charactersCount[(int)character.ClientType] + nbEvilCharacters, _role);            } else            {                indexEvil++;                _currentQTESequence = QTELoader.Instance.GetRandomQTE(character.ClientType, character.Evilness, indexEvil, _role);            }            _currentListSequences.AddSequence(_currentQTESequence);        }        _currentListSequences.SetUpList();        LengthInputs = _currentListSequences.TotalLengthInputs;    }

    public int GetNbOfEvilCharacters(CharacterTypeData[] characters)    {        int total = 0;        foreach (CharacterTypeData character in characters)        {            if (character.Evilness == Evilness.EVIL) total++;        }        return total;    }
    private void StartSequenceDependingOntype()    {        _indexInSequence = 0;        _currentQTESequence = _currentListSequences.GetSequence(_indexOfSequence);        _inputsSucceeded = new QTE_STATE[_currentQTESequence.ListSubHandlers.Count];        for (int i = 0; i < _inputsSucceeded.Length; i++)
        {
            _inputsSucceeded[i] = QTE_STATE.NEED_RELEASE;
        }        _events?.CallOnQTEStarted();                switch (_currentQTESequence.SequenceType)        {            case InputsSequence.SEQUENCE:                _coroutineQTE = StartCoroutine(StartRoutineSequence());                break;
            case InputsSequence.SIMULTANEOUS:                _coroutineQTE = StartCoroutine(StartRoutineSimultaneous());                break;        }    }
    public void PauseQTE(bool value)
    {
        if (value)        {            StopCurrentCoroutine();        } else        {            StartQTE();        }
    }

    public void StopCurrentCoroutine()    {        if (_coroutineQTE != null)
        {
            StopCoroutine(_coroutineQTE);
            _coroutineQTE = null;
        }    }

    public void DeleteCurrentCoroutine()
    {
        StopCurrentCoroutine();
        _currentQTESequence = null;
        _currentListSequences.Clear();
    }
    #endregion

    public void CreateListInputsListened()
    {
        if (_includeLeftJoystick)
        {
            _inputsQTE.AddRange(new List<InputClass>()
            {
                _playerController.LeftJoystick.InputClassX,                _playerController.LeftJoystick.InputClassY
            });
        }
        if (_includeRightJoystick)
        {
            _inputsQTE.AddRange(new List<InputClass>()
            {
                _playerController.RightJoystick.InputClassX,                _playerController.RightJoystick.InputClassY
            });
        }
        _inputsQTE.AddRange(new List<InputClass>()
        {
            _playerController.Action1,            _playerController.Action2,            _playerController.Action3,            _playerController.Action4,            _playerController.LT,            _playerController.RT
        });
           }

    public string GetQTEString()
    {
        if (_currentListSequences != null && _currentListSequences.Length > 0)
        {            return _currentListSequences.ToString(_indexOfSequence, _indexInSequence);
        }
        return String.Empty;
    }

    public string GetCurrentInputString()    {        if (_currentListSequences != null && _currentListSequences.Length > 0)
        {            return _currentListSequences.GetInputString(_indexOfSequence, _indexInSequence);
        }
        return String.Empty;    }
    void CheckInputs(int expectedActionID) //SHORT
    {
        if (!_checkInputThisBeat.HadInputThisBeat)        {            InputBool inputBool;
            InputFloat inputFloat;
            foreach (InputClass inputRef in _inputsQTE)            {                if (!_checkInputThisBeat.HadInputThisBeat && inputRef != null)                {                    inputBool = inputRef as InputBool;
                    if (inputBool != null && inputBool.IsJustPressed)                    {                        ChangeInput(inputRef.ActionID, expectedActionID);                    }                    inputFloat = inputRef as InputFloat;                    if (inputFloat != null)
                    {
                        if (_currentQTESequence.ListSubHandlers[_indexInSequence].PositiveValue && inputFloat.InputValue > thresholdDirectionJoystick)
                        {
                            ChangeInput(inputRef.ActionID, expectedActionID);
                            
                        } else if (!_currentQTESequence.ListSubHandlers[_indexInSequence].PositiveValue && inputFloat.InputValue < - thresholdDirectionJoystick)
                        {
                            ChangeInput(inputRef.ActionID, expectedActionID);
                        }
                    }
                }            }
        }
    }

    void ChangeInput(int currentActionID, int expectedActionID)
    {
        _checkInputThisBeat.ChangeHadInputThisBeat();

        if (currentActionID == expectedActionID)
        {
            Debug.Log("Input ok");
            _inputsSucceeded[_indexInSequence] = QTE_STATE.IS_PRESSED;
            _currentListSequences.SetInputSucceeded(_indexInListSequences, QTE_STATE.IS_PRESSED);
            _indexInSequence++;
            _indexInListSequences++;
            _events?.CallOnCorrectInput();
        }
        else
        {
            Debug.Log("Input failed");
            _indexInSequence++;
            _indexInListSequences++;
            _events?.CallOnWrongInput();
        }
    }

    #region Routines
    IEnumerator StartRoutineSequence()
    {
        yield return new WaitUntil(() => _playerController != null);
        UnitInput correctInput = _currentQTESequence.ListSubHandlers[_indexInSequence];
        while (_indexInSequence < _currentQTESequence.ListSubHandlers.Count)
        {
            if ((_inputsAreOnBeat && (_timingable?.IsInsideBeatWindow ?? true)) || !_inputsAreOnBeat)
            {
                CheckInputs(_currentQTESequence.ListSubHandlers[_indexInSequence].ActionIndex);
            } 
            yield return null;
        }        ClearRoutine();
    }
    IEnumerator StartRoutineSimultaneous()    {        yield return new WaitUntil(() => _playerController != null);
        _isSequenceComplete = false;        InputClass[] inputs = new InputClass[_currentQTESequence.ListSubHandlers.Count];
        for (int i = 0; i < inputs.Length;i++)        {            inputs[i] = _playerController.GetInputClassWithID(_currentQTESequence.ListSubHandlers[i].ActionIndex,true);        }        _durationHold = 0;                while ((!_isSequenceComplete &&_currentQTESequence.Status == InputStatus.SHORT) || _durationHold < (_currentQTESequence.DurationHold * _timingable.BeatDurationInMilliseconds))        {            for (int i = 0; i < _currentQTESequence.ListSubHandlers.Count; i++)            {                if (inputs[i] == null) continue;                bool inputValue = _currentQTESequence.ListSubHandlers[i].UseforShake ? (inputs[i] as InputVector2)?.IsMoving ?? false : inputs[i].IsPerformed;                switch (_inputsSucceeded[i])
                {
                    case QTE_STATE.NEED_PRESS:
                        if (inputValue)
                        {
                            _inputsSucceeded[i] = QTE_STATE.IS_PRESSED;
                            _currentListSequences.SetInputSucceeded(i, QTE_STATE.IS_PRESSED);                            _events?.CallOnCorrectInput();
                        }
                        break;
                    case QTE_STATE.NEED_RELEASE:
                        if (!inputValue)
                        {
                            _inputsSucceeded[i] = QTE_STATE.NEED_PRESS;
                            _currentListSequences.SetInputSucceeded(i, QTE_STATE.NEED_PRESS);
                        }
                        break;
                    case QTE_STATE.IS_PRESSED:
                        if (!inputValue)
                        {
                            _inputsSucceeded[i] = QTE_STATE.NEED_PRESS;
                            _currentListSequences.SetInputSucceeded(i, QTE_STATE.NEED_PRESS);                            _events?.CallOnWrongInput();
                        }
                        break;
                    default:
                        break;
                }            }            yield return null;            _isSequenceComplete = _inputsSucceeded.ToList().TrueForAll(x => x == QTE_STATE.IS_PRESSED);
            if (_isSequenceComplete && _currentQTESequence.Status == InputStatus.LONG)            {                _durationHold += (int)(Time.deltaTime * 1000);            }        }        ClearRoutine();    }
    void ClearRoutine()    {        _indexOfSequence++;        _currentQTESequence = null;
        _inputsSucceeded = null;
        if (_indexOfSequence < _currentListSequences.Length) // There is a next sequence        {            StartSequenceDependingOntype();        } else // End of the list of sequences        {            _currentListSequences.Clear();            _events?.CallOnQTEComplete();        }    }    #endregion
}
