using System;
using UnityEngine;

public interface IFloatInputable
{
    public float InputValue { get; }
    public bool IsInputPerformed => InputValue != 0f;
    public float InputDuration { get; }
    public Action OnInputStart { get; }
    public Action OnInputEnd { get; }
}


public interface IBoolInputable
{
    public bool InputValue { get; }
    public float InputDuration { get; }
    public Action OnInputStart { get; }
    public Action OnInputEnd { get; }
}