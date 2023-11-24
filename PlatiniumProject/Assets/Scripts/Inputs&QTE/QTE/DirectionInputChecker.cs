using Rewired.Data.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionInputChecker
{
    InputFloat _joystickDirection;
    float _inputThreshold = 0.9f;
    bool[] _directionEventsSent;

    public event Action<int> OnInputStartedInDirection;
    public event Action<int> OnInputEndedInDirection;

    public DirectionInputChecker(InputFloat joystickDirection,float threshold)
    {
        _joystickDirection = joystickDirection;
        _inputThreshold = threshold;
        _directionEventsSent = new bool[2];
        _joystickDirection.OnInputChange += () => ChangeValue();
    }

    ~DirectionInputChecker()
    {
        _joystickDirection.OnInputChange -= () => ChangeValue();
    }

    public void ChangeValue()
    {
        UpdateEvents();
    }

    void UpdateEvents()
    {
        ChangeValueDirection(1, _joystickDirection.InputValue > _inputThreshold);
        ChangeValueDirection(-1, _joystickDirection.InputValue < -_inputThreshold);
    }

    //1 : positive, -1 : negative of axis
    void ChangeValueDirection(int valueAxis, bool value)
    {
        if (_directionEventsSent[(valueAxis - 1)/-2] != value)
        {
            _directionEventsSent[(valueAxis - 1) / -2] = value;
            if (value) //Start event
            {
                OnInputStartedInDirection?.Invoke(valueAxis);
            } else //End event
            {
                OnInputEndedInDirection?.Invoke(valueAxis);
            }
        }
    }
}
