using UnityEngine;

public class JoystickShake
{
    DirectionInputChecker _directionChecker;
    private int _nextDirection;
    public int NbShakeLeft { get; private set;} 

    public JoystickShake(InputFloat input,int maxShake)
    {
        NbShakeLeft = maxShake;
        _nextDirection = 1;
        _directionChecker = new DirectionInputChecker(input, 0.9f);
        _directionChecker.OnInputStartedInDirection += OnEnterCheckDirection;
    }

    private void OnEnterCheckDirection(int direction)
    {
        if (direction == _nextDirection)
        {
            _nextDirection = -_nextDirection;
            NbShakeLeft--;
        }
    }
}
