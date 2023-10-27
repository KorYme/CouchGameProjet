using Rewired;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class QTEHandler : MonoBehaviour
{
    [SerializeField] PlayerRole _role;
    PlayerInputController _playerController;
    int _indexInSequence = 0;
    QTESequence _currentQTESequence;
    [SerializeField] BeatManager _beatManager;
    private Coroutine _coroutineQTE;
    IQTEable _QTEable;
    bool _isPlaying = true;

    private void Update()
    {
        if (_playerController == null)
        {
            SetupController();
        }
    }
    private void SetupController()
    {
        _playerController = Players.PlayersController[(int)_role];
    }
    public void RegisterQTEable(IQTEable QTEable)
    {
        _QTEable = QTEable;
    }

    public void StartRandomQTE()
    {
        if (_coroutineQTE != null)
        {
            StopCoroutine();
        }
        _currentQTESequence = QTELoader.Instance.GetRandomQTE(_role);
        _indexInSequence = 0;
        if (_QTEable != null)
        {
            _QTEable.OnQTEStarted(_currentQTESequence);
        }
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
        _isPlaying = !value;
    }

    public void StopCoroutine()
    {
        StopCoroutine(_coroutineQTE);
        _coroutineQTE = null;
        
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
                isInputCorrect = _playerController.GetInput(input);
                break;
            case InputStatus.HOLD:
                isInputCorrect = _playerController.GetInputHold(input);
                break;
        }
        return isInputCorrect;
    }

    IEnumerator StartRoutineSequence()
    {
        UnitInput input = _currentQTESequence.ListSubHandlers[_indexInSequence];
        while (_indexInSequence < _currentQTESequence.ListSubHandlers.Count)
        {
            yield return new WaitUntil(() => _isPlaying);
            if (_beatManager.IsInsideBeat)
            {
                if (CheckInput(input))
                {
                    _indexInSequence++;

                    if (_indexInSequence < _currentQTESequence.ListSubHandlers.Count) //Sequence finished
                    {
                        input = _currentQTESequence.ListSubHandlers[_indexInSequence];
                        if (_QTEable != null)
                        {
                            _QTEable.OnQTECorrectInput();
                        }
                    }
                }
            }
            yield return null;
        }
        if (_QTEable != null)
        {
            _QTEable.OnQTEComplete();
        }
    }
}
