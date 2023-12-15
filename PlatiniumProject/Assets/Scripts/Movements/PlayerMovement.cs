using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovement : EntityMovement,IIsControllable
{
    [Header("Player Movements Parameters")]
    [SerializeField, Range(0f, 1f)] protected float _inputDeadZone = .5f;

    protected abstract PlayerRole PlayerRole { get; }
    protected PlayerInputController _playerController;
    protected bool _isInputReset = true;
    protected bool _hasAlreadyMovedThisBeat;

    protected CharacterAnimation _animation;
    protected SpriteRenderer _sp;
    protected DropManager _dropManager;

    protected virtual IEnumerator Start()
    {
        Globals.BeatManager.OnBeatEvent.AddListener(OnBeat);
        _sp = GetComponentInChildren<SpriteRenderer>();
        _animation = GetComponent<CharacterAnimation>();
        _dropManager = Globals.DropManager;
        SetUpEventsDrop();
        yield return new WaitUntil(() => Players.PlayersController[(int)PlayerRole] != null);
        _playerController = Players.PlayersController[(int)PlayerRole];
        Players.AddListenerPlayerController(this);
        _playerController.LeftJoystick.OnInputChange += CheckJoystickValue;
        _timingable.OnBeatStartEvent.AddListener(AllowNewMovement);
        
    }
    
    protected virtual void OnBeat()
    {
        SetAnimation();
    }

    protected virtual void SetAnimation()
    {
        if (!IsMoving)
        {
            _animation.SetAnim(ANIMATION_TYPE.IDLE);
        }
    }
    
    protected virtual void OnDestroy()
    {
        if (_playerController != null)
        {
            _playerController.LeftJoystick.OnInputChange -= CheckJoystickValue;
            _timingable.OnBeatStartEvent.RemoveListener(AllowNewMovement);
        }
        Globals.BeatManager.OnBeatEvent.RemoveListener(OnBeat);
        Players.RemoveListenerPlayerController(this);
        _dropManager.OnBeginBuildUp -= OnBeginDrop;
        _dropManager.OnDropSuccess -= OnDropEnd;
        _dropManager.OnDropFail -= OnDropEnd;
    }

    protected abstract void OnInputMove(Vector2 vector);

    protected void AllowNewMovement() => _hasAlreadyMovedThisBeat = false;

    public bool MoveTo(Vector3 position, ANIMATION_TYPE moveAnim = ANIMATION_TYPE.MOVE)
    {
        if (_hasAlreadyMovedThisBeat || !_timingable.IsInsideBeatWindow) {
            return false;
        }

        if (MoveToPosition(position, false,moveAnim))
        {
            _hasAlreadyMovedThisBeat = true;
            return true;
        }
        return false;
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

    public void ChangeController()
    {
        if (_playerController != null)
            _playerController.LeftJoystick.OnInputChange -= CheckJoystickValue;
        _playerController = Players.PlayersController[(int)PlayerRole];
        if (_playerController != null)
            _playerController.LeftJoystick.OnInputChange += CheckJoystickValue;
    }

    private void SetUpEventsDrop()
    {
        _dropManager.OnBeginBuildUp += OnBeginDrop;
        _dropManager.OnDropSuccess += OnDropEnd;
        _dropManager.OnDropFail += OnDropEnd;
    }

    protected abstract void OnBeginDrop();
    protected abstract void OnDropEnd();

}
