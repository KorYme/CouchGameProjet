using Rewired;
using System;
using System.Globalization;
using UnityEngine;

public class InputFloat : InputClass, IFloatInputable
{
    public float InputValue { get; private set; }

    public override void InputCallback(InputActionEventData data)
    {

    }
}
