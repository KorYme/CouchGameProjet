using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovement : EntityMovement
{
    [Header("Player Movements Parameters")]
    [SerializeField, Range(0f, 1f)] protected float _inputDeadZone;

    protected abstract PlayerRole _playerRole { get; }
    protected bool _isInputReset = true;
    protected PlayerInputController _playerController;

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => Players.PlayersController[(int)_playerRole] != null);
        _playerController = Players.PlayersController[(int)_playerRole];
        _playerController.LeftJoystick.OnInputChange += CheckJoystickValue;
    }

    protected virtual void CheckJoystickValue()
    {
        Vector2 vector = GetClosestUnitVectorFromVector(_playerController.LeftJoystick.InputValue);
        if (!_isInputReset && vector == Vector2.zero)
        {
            _isInputReset = true;
        }
        else if (_isInputReset && vector != Vector2.zero)
        {
            OnInputMove(vector);
            _isInputReset = false;
        }
    }

    protected abstract void OnInputMove(Vector2 vector);

    private Vector2 GetClosestUnitVectorFromVector(Vector2 vector)
    {
        if (vector.magnitude < _inputDeadZone) return Vector2.zero;
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            return new Vector2(Mathf.Sign(vector.x), 0f);
        }
        else
        {
            return new Vector2(0f, Mathf.Sign(vector.y));
        }
    }

    public override bool MoveToPosition(Vector3 position)
    {
        return base.MoveToPosition(position);
        // A MODIFIER
    }
}
