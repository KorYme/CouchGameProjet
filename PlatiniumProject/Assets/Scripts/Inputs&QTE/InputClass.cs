using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputClass
{
    public int ActionID { get; protected set; }
    public virtual double InputDuration { get; protected set; }
    public abstract bool IsPerformed { get; }
    public virtual bool IsJustPressed { get; protected set; }
    public Action OnInputStart { get; set; }
    public Action OnInputEnd { get; set; }
    public Action OnInputChange { get; set; }

    public InputClass(int actionID)
    {
        ActionID = actionID;
        InputDuration = 0f;
    }

    public abstract void InputCallback(InputActionEventData data);
}