using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TriggerInputCheck
{
    public enum TRIGGER_STATE
    {
        RELEASED,
        PRESSED_ON_BEAT,
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
            _triggerState = value;
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
                TriggerState = Globals.BeatManager.IsInsideBeatWindow ? TRIGGER_STATE.PRESSED_ON_BEAT : TRIGGER_STATE.NEED_TO_BE_RELEASED;
                OnTriggerPerformed?.Invoke(true);
            }
        }
    }
}
