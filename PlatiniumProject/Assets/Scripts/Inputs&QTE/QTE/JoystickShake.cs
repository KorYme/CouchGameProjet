using UnityEngine;

public class JoystickShake : IPerformed
{
    DirectionInputChecker _directionChecker;
    private int _nextDirection;
    public int NbShakeLeft { get; private set; } = 5;
    public bool IsGoodDirectionPerformed { get; private set; }
    public bool IsInputPerformed => IsGoodDirectionPerformed;

    public JoystickShake(InputFloat input)
    {
        _nextDirection = 1;
        _directionChecker = new DirectionInputChecker(input, 0.05f);
        _directionChecker.OnInputStartedInDirection += OnEnterCheckDirection;
        _directionChecker.OnInputEndedInDirection += OnExitCheckDirection;
    }

    private void OnEnterCheckDirection(int direction)
    {
        if (direction == _nextDirection)
        {
            IsGoodDirectionPerformed = true;
        }
    }

    private void OnExitCheckDirection(int direction)
    {
        if (direction == _nextDirection)
        {
            IsGoodDirectionPerformed = false;
        }
    }

    public void NextDirection()
    {
        _nextDirection = -_nextDirection;
        NbShakeLeft--;
    }

    public void ReloadMaxShake(int maxShake)
    {
        NbShakeLeft = maxShake;
    }
}
