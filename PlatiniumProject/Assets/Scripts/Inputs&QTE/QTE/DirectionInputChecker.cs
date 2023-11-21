using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionInputChecker : MonoBehaviour
{
    InputVector2 _joystickDirection;
    float _inputThreshold;
    
    public DirectionInputChecker(InputVector2 joystickDirection,float threshold)
    {
        _joystickDirection = joystickDirection;
        _inputThreshold = threshold;
    }

    public void GetDirection()
    {

    }
}
