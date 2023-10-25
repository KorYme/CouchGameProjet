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

    public override bool IsPerformed => InputValue;

    public override void InputCallback(InputActionEventData data)
    {
        switch (data.eventType)
        {
            case InputActionEventType.ButtonJustPressed:
                InputValue = true;
                OnInputStart?.Invoke();
                break;
            case InputActionEventType.ButtonJustReleased:
                InputValue = false;
                OnInputEnd?.Invoke();
                break;
            default:
                break;
        }
    }
}
