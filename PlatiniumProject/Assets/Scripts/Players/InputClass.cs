using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputClass
{
    public int ActionID { get; protected set; }
    public float InputDuration => (float)(_firstInputTime - DateTime.Now).TotalSeconds;
    public abstract bool IsPerformed { get; }
    public Action OnInputStart { get; set; }
    public Action OnInputEnd { get; set; }
    public Action OnInputChange { get; set; }

    protected DateTime _firstInputTime;

    public InputClass(int actionID)
    {
        ActionID = actionID;
        OnInputStart += () => _firstInputTime = DateTime.Now;
    }

    public abstract void InputCallback(InputActionEventData data);
}