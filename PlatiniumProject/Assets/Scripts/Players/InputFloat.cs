using Rewired;
using System;
using System.Globalization;
using UnityEngine;

public class InputFloat : InputClass
{
    public InputFloat(int actionID) : base(actionID)
    {
    }

    public override bool IsPerformed => InputValue != 0f;

    float _inputValue;
    public float InputValue
    {
        get => _inputValue;
        private set
        {
            if (value != _inputValue)
            {
                value = _inputValue;
                OnInputChange?.Invoke();
            }
        }
    }

    public override void InputCallback(InputActionEventData data)
    {
        InputDuration = data.GetAxisTimeActive();
        switch (data.eventType)
        {
            case InputActionEventType.AxisActive:
                if (InputValue == 0f)
                {
                    OnInputStart?.Invoke();
                }
                InputValue = data.GetAxis();
                break;
            case InputActionEventType.AxisInactive:
                if (InputValue != 0f)
                {
                    OnInputEnd?.Invoke();
                    InputValue = 0f;
                }
                break;
            default:
                break;
        }
    }
}
