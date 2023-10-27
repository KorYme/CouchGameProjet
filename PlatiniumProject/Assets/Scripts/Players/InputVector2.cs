using Rewired;
using System;
using UnityEngine;

public class InputVector2 : InputClass
{
    InputFloat _inputValueX;
    InputFloat _inputValueY;

    public int SecondActionID { get; private set; }

    public Player Player { get; set; }

    public override bool IsPerformed => _inputValueX.IsPerformed && _inputValueY.IsPerformed;

    public Vector2 InputValue => new Vector2(_inputValueX.InputValue, _inputValueY.InputValue);

    public override double InputDuration => Math.Max(_inputValueX.InputDuration, _inputValueY.InputDuration);

    public InputVector2(int actionID, int secondActionID) : base(actionID)
    {
        SecondActionID = secondActionID;
        _inputValueX = new InputFloat(actionID);
        _inputValueY = new InputFloat(secondActionID);
        _inputValueX.OnInputChange += () =>
        {
            ForceUpdateY();
            OnInputChange?.Invoke();
        };
        _inputValueY.OnInputChange += () =>
        {
            ForceUpdateX();
            OnInputChange?.Invoke();
        };
        _inputValueX.OnInputStart += () =>
        {
            if (!_inputValueY.IsPerformed)
            {
                OnInputStart?.Invoke();
            }
        };
        _inputValueX.OnInputEnd += () =>
        {
            if (!_inputValueY.IsPerformed)
            {
                OnInputEnd?.Invoke();
            }
        };
        _inputValueY.OnInputStart += () =>
        {
            if (!_inputValueX.IsPerformed)
            {
                OnInputStart?.Invoke();
            }
        };
        _inputValueY.OnInputEnd += () =>
        {
            if (!_inputValueX.IsPerformed)
            {
                OnInputEnd?.Invoke();
            }
        };
    }

    public override void InputCallback(InputActionEventData data)
    {
        _inputValueX.InputCallback(data);
    }

    void ForceUpdateX()
    {
        _inputValueX.InputCallbackForced(Player);
    }

    public void InputCallbackSecondAction(InputActionEventData data)
    {
        _inputValueY.InputCallback(data);
    }

    void ForceUpdateY()
    {
        _inputValueY.InputCallbackForced(Player);
    }
}