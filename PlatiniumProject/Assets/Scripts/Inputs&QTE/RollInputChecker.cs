using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollInputChecker
{
    /// <summary>
    /// If rotate clockwise then 1 else -1 
    /// </summary>
    int _rotationOrientation = 0;
    int _directionChecked = 0;
    Vector2 _lastDirection = Vector2.zero;

    public event Action TurnClockWise;
    public event Action TurnAntiClockWise;

    //Parameters
    float _inputDistance;
    InputVector2 _inputJoystick;

    public RollInputChecker(InputVector2 inputController, float inputDistance)
    {
        _inputJoystick = inputController;
        _inputDistance = inputDistance;
        _inputJoystick.OnInputChange += () => GetDirection();
    }

    ~RollInputChecker()
    {
        _inputJoystick.OnInputChange -= () => GetDirection();
    }

    public void GetDirection()
    {
        Vector2 closestPoint = GetClosestUnitVectorFromVector(_inputJoystick.InputValue);
        if (_lastDirection == closestPoint) return;
        if (closestPoint == Vector2.zero)
        {
            _lastDirection = closestPoint;
            _rotationOrientation = 0;
            _directionChecked = 0;
            return;
        }
        if (closestPoint == -_lastDirection)
        {
            //Cas quasi impossible mais on sait jamais, si le joueur est ultra rapide
            _lastDirection = closestPoint;
            _rotationOrientation = 0;
            _directionChecked = 1;
            return;
        }
        if (_rotationOrientation == 0)
        {
            if (_lastDirection != Vector2.zero)
            {
                _rotationOrientation = (new Vector2(_lastDirection.y, -_lastDirection.x) == closestPoint) ? 1 : -1;
            }
        }
        else if (new Vector2(_lastDirection.y, -_lastDirection.x) * _rotationOrientation != closestPoint)
        {
            _rotationOrientation *= -1;
            _directionChecked = 1;
        }
        _lastDirection = closestPoint;
        _directionChecked++;
        if (_directionChecked >= 4)
        {
            switch (_rotationOrientation)
            {
                case 1:
                    TurnClockWise?.Invoke();
                    break;
                case -1:
                    TurnAntiClockWise?.Invoke();
                    break;
                default:
                    break;
            }
            _directionChecked = 1;
            _rotationOrientation = 0;
        }
    }

    private Vector2 GetClosestUnitVectorFromVector(Vector2 vector)
    {
        if (vector.magnitude < _inputDistance) {
            return Vector2.zero;
        }
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            return new Vector2(Mathf.Sign(vector.x), 0f);
        }
        else
        {
            return new Vector2(0f, Mathf.Sign(vector.y));
        }
    }
}
