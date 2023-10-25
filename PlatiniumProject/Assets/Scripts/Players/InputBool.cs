using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBool : InputClass, IBoolInputable
{
    public bool InputValue { get; private set; }

    public override void InputCallback(InputActionEventData data)
    {
        switch (data.eventType)
        {
            case InputActionEventType.ButtonJustPressed:
                InputValue = true;
                _firstInputTime = DateTime.Now;
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
