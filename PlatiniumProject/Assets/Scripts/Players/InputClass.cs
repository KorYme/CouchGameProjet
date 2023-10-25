using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputClass
{
    protected DateTime _firstInputTime = DateTime.Now;
    public float InputDuration => (float)(_firstInputTime - DateTime.Now).TotalSeconds;
    public Action OnInputStart { get; set; }
    public Action OnInputEnd { get; set; }

    public abstract void InputCallback(InputActionEventData data);
}