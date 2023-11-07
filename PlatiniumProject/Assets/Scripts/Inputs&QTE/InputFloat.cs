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
                _inputValue = value;
                OnInputChange?.Invoke();
            }
        }
    }

    public override void InputCallback(InputActionEventData data)
    {
        InputDuration = data.GetAxisTimeActive();
        float tmp = data.GetAxis();
        if (!IsPerformed && tmp != 0f)
        {
            InputValue = tmp;
            OnInputStart?.Invoke();
        } else if (IsPerformed && tmp == 0f)
        {
            InputValue = tmp;
            OnInputEnd?.Invoke();
        } else
        {
            InputValue = tmp;
        }
    }

    public void InputCallbackForced(Player player)
    {
        InputDuration = player.GetAxisTimeActive(ActionID);
        float tmp = player.GetAxis(ActionID);
        if (!IsPerformed && tmp != 0f)
        {
            _inputValue = tmp;
            OnInputStart?.Invoke();
        }
        else if (IsPerformed && tmp == 0f)
        {
            _inputValue = tmp;
            OnInputEnd?.Invoke();
        }
        else
        {
            _inputValue = tmp;
        }
    }
}
