using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovement : EntityMovement
{
    [Header("Player Movements Parameters")]
    [SerializeField, Range(0f, 1f)] protected float _inputDeadZone = .5f;

    protected abstract PlayerRole PlayerRole { get; }
    protected PlayerInputController _playerController;
    protected bool _isInputReset = true;
    protected bool _hasAlreadyMovedThisBeat;

    protected CharacterAnimation _animation;
    protected SpriteRenderer _sp;

    protected virtual IEnumerator Start()
    {
        _sp = GetComponentInChildren<SpriteRenderer>();
        _animation = GetComponent<CharacterAnimation>();
        OnMove += AnimationSetter;
        yield return new WaitUntil(() => Players.PlayersController[(int)PlayerRole] != null);
        _playerController = Players.PlayersController[(int)PlayerRole];
        _playerController.LeftJoystick.OnInputChange += CheckJoystickValue;
        _timingable.OnBeatStartEvent.AddListener(AllowNewMovement);
        
    }

    protected virtual void OnDestroy()
    {
        OnMove -= AnimationSetter;
        if (_playerController != null)
        {
            _playerController.LeftJoystick.OnInputChange -= CheckJoystickValue;
            _timingable.OnBeatStartEvent.RemoveListener(AllowNewMovement);
        }
    }

    protected abstract void OnInputMove(Vector2 vector);

    protected void AllowNewMovement() => _hasAlreadyMovedThisBeat = false;

    public bool MoveTo(Vector3 position)
    {
        if (_hasAlreadyMovedThisBeat || !_timingable.IsInsideBeatWindow) return false;
        if (MoveToPosition(position, _animation.CharacterAnimationObject.Animations[ANIMATION_TYPE.MOVE].AnimationLenght))
        {
            _hasAlreadyMovedThisBeat = true;
            return true;
        }
        return false;
    }
    
    private void AnimationSetter()
    {
        _sp.sprite = _animation.GetAnimationSprite(ANIMATION_TYPE.MOVE);
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

    protected Vector2 GetClosestUnitVectorFromVector(Vector2 vector)
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
}
