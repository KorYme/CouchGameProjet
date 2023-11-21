using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionInputChecker : MonoBehaviour
{
    InputVector2 _joystickDirection;
    float _inputThreshold = 0.9f;
    Vector2 _joystickDirectionReference;

    public event Action OnInputChange;

    public DirectionInputChecker(InputVector2 joystickDirection,float threshold)
    {
        _joystickDirection = joystickDirection;
        _inputThreshold = threshold;
        _joystickDirection.OnInputChange += () => ChangeValue();
    }

    ~DirectionInputChecker()
    {
        _joystickDirection.OnInputChange -= () => ChangeValue();
    }

    public void ChangeValue()
    {
        //Vector2 direction = CheckDirectionFromVector(_joystickDirection.InputValue);
    }
}
