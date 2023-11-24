using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBool : InputClass
{
    public InputBool(int actionID) : base(actionID)
    {
        OnInputStart += OnInputChange;
        OnInputEnd += OnInputChange;
    }

    public bool InputValue { get; private set; }
    public bool IsJustUnpressed { get; private set; }

    public override bool IsPerformed => InputValue;

    public override void InputCallback(InputActionEventData data)
    {
        InputDuration = data.GetButtonTimePressed();
        bool tmp = data.GetButton();
        IsJustPressed = false;
        IsJustUnpressed = false;
        if (!IsPerformed && tmp)
        {
            InputValue = tmp;
            OnInputStart?.Invoke();
            IsJustPressed = true;
        }
        else if (IsPerformed && !tmp)
        {
            InputValue = tmp;
            OnInputEnd?.Invoke();
            IsJustPressed = false;
        }
    }
}
