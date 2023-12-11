using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerInputCheck
{
    public enum TRIGGER_STATE
    {
        RELEASED,
        PRESSED_ON_TIME,
        NEED_TO_BE_RELEASED,
    }

    float _inputDeadZone;
    InputFloat _inputTrigger;
    public event Action<bool> OnTriggerPerformed;
    public event Action<TRIGGER_STATE> OnTriggerStateChange;
    TRIGGER_STATE _triggerState;

    public TRIGGER_STATE TriggerState
    {
        get => _triggerState;
        set
        {
            if (value == TRIGGER_STATE.NEED_TO_BE_RELEASED && !_inputTrigger.IsPerformed)
            {
                _triggerState = TRIGGER_STATE.RELEASED;
            }
            else
            {
                _triggerState = value;
            }
            OnTriggerStateChange?.Invoke(value);
        }
    }


    public TriggerInputCheck(InputFloat inputTrigger, float inputDeadZone)
    {
        _inputTrigger = inputTrigger;
        _inputDeadZone = inputDeadZone;
        TriggerState = TRIGGER_STATE.RELEASED;
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
            if (TriggerState != TRIGGER_STATE.RELEASED)
            {
                TriggerState = TRIGGER_STATE.RELEASED;
                OnTriggerPerformed?.Invoke(false);
            }
        }
        else
        {
            if (TriggerState == TRIGGER_STATE.RELEASED)
            {
                TriggerState = TRIGGER_STATE.PRESSED_ON_TIME;
                OnTriggerPerformed?.Invoke(true);
            }
        }
    }
}
