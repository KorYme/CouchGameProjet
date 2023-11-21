using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTESequenceShake : QTEHandler
{
    /*IEnumerator CheckInputsShakeSequence(QTESequence sequence)
    {
        yield return new WaitUntil(() => _playerController != null);

        _isSequenceComplete = false;
        InputClass[] inputs = new InputClass[_currentQTESequence.ListSubHandlers.Count];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = _playerController.GetInputClassWithID(_currentQTESequence.ListSubHandlers[i].ActionIndex, true);
        }
        _durationHold = 0;

        while ((!_isSequenceComplete && _currentQTESequence.Status == InputStatus.SHORT) || _durationHold < (_currentQTESequence.DurationHold * _timingable.BeatDurationInMilliseconds))
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
                            _inputsSucceeded[i] = vectAxis.IsMoving;
                            _currentListSequences.SetInputSucceeded(i, _inputsSucceeded[i]);

                            if (_inputsSucceeded[i])
                            {
                                _events?.CallOnCorrectInput();
                            }
                            else
                            {
                                _events?.CallOnWrongInput();
                            }
                        }
                    }
                    else if (_inputsSucceeded[i] != inputs[i].IsPerformed)
                    {
                        _inputsSucceeded[i] = inputs[i].IsPerformed;
                        _currentListSequences.SetInputSucceeded(i, _inputsSucceeded[i]);

                        if (_inputsSucceeded[i])
                        {
                            _events?.CallOnCorrectInput();
                        }
                        else
                        {
                            _events?.CallOnWrongInput();
                        }
                    }
                }
            }
            yield return null;
            _isSequenceComplete = CheckSequence();

            if (_isSequenceComplete && _currentQTESequence.Status == InputStatus.LONG)
            {
                _durationHold += (int)(Time.deltaTime * 1000);
            }
        }
    }*/
}
