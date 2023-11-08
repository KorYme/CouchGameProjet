using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerInputCheck
{
    float _inputDeadZone;
    InputFloat _inputTrigger;

    public event Action<bool> OnTriggerStateChange;

    bool _needToBeReleased;
    public bool NeedToBeReleased
    {
        get => _needToBeReleased;
        set => _needToBeReleased = IsPressed && value;
    }
    public bool IsPressed { get; private set; }
    public bool IsPressedOnBeat => IsPressed && !NeedToBeReleased;

    public TriggerInputCheck(InputFloat inputTrigger, float inputDeadZone)
    {
        _inputTrigger = inputTrigger;
        _inputDeadZone = inputDeadZone;
        _needToBeReleased = false;
        _inputTrigger.OnInputChange += () => GetTriggerValue();
    }

    ~TriggerInputCheck()
    {
        _inputTrigger.OnInputChange -= () => GetTriggerValue();
    }

    private void GetTriggerValue()
    {
        if (_inputTrigger.InputValue < _inputDeadZone)
        {
            if (IsPressed)
            {
                IsPressed = false;
                _needToBeReleased = false;
                OnTriggerStateChange?.Invoke(false);
            }
        }
        else
        {
            if (!IsPressed)
            {
                IsPressed = true;
                OnTriggerStateChange?.Invoke(true);
            }
        }
    }
}
