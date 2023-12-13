using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BouncerMovement : PlayerMovement, IQTEable
{
    public enum BOUNCER_STATE
    {
        MOVING,
        CHECKING,
        IDLE
    }
    public enum CHECKING_STATE
    {
        CHECKING,
        QTE,
        NONE
    }

    [Space, Header("Bouncer Parameters")]
    [SerializeField] private AreaManager _areaManager;

    private BOUNCER_STATE _currentState = BOUNCER_STATE.MOVING;
    private CharacterCheckByBouncerState _currentClient;
    public CharacterCheckByBouncerState CurrentClient => _currentClient;

    private SlotInformation _currentSlot;

    protected override PlayerRole PlayerRole => PlayerRole.Bouncer;
    private BouncerQTEController _qteController;
    private bool _isInDrop = false;
    private CHECKING_STATE _checkingState = CHECKING_STATE.NONE;
    private ANIMATION_TYPE _moveAnim;
    
    [SerializeField] UnityEvent _onAccept;
    [SerializeField] UnityEvent _onRefuse;
    [SerializeField] UnityEvent _onCheck;
    [SerializeField] UnityEvent _onMovement;
    private void Awake()
    {
        _qteController = GetComponent<BouncerQTEController>();
    }

    protected override void SetAnimation()
    {
        switch (_currentState)
        {
            case BOUNCER_STATE.CHECKING:
                _sp.flipX = true;
                _animation.SetAnim(ANIMATION_TYPE.FIGHT_IDLE);
                
                break;
            case BOUNCER_STATE.MOVING:
                _sp.flipX = false;
                _animation.SetAnim(ANIMATION_TYPE.IDLE, false);
                break;
        }
    }
    protected override IEnumerator Start()
    {
        yield return base.Start();
        _currentSlot = _areaManager.BouncerBoard.Board[_areaManager.BouncerBoard.BoardDimension.x
            * Mathf.Max(1,_areaManager.BouncerBoard.BoardDimension.y / 2 + _areaManager.BouncerBoard.BoardDimension.y % 2) -1];
        _currentSlot.PlayerOccupant = this;
        transform.position = _currentSlot.transform.position;
        QTEHandler qteHandler;
        TryGetComponent(out qteHandler);
        if (qteHandler != null)
        {
            qteHandler.RegisterListener(this);
        }
        Debug.Log("Bouncer Initialis�");
    }

    protected override void OnInputMove(Vector2 vector)
    {
        if (_currentState == BOUNCER_STATE.MOVING && !_isInDrop)
        {
            Move((int)GetClosestDirectionFromVector(vector));
        }
    }

    public void CheckMode(CharacterStateMachine chara, CharacterCheckByBouncerState state)
    {
        _onCheck?.Invoke();
        _currentClient = state;
        _currentState = BOUNCER_STATE.CHECKING;
        StartCoroutine(TestCheck(chara.transform.position));
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BOUNCER).StartFocus(transform);
    }
    
    public void Move(int index)
    {
        if (_currentSlot.Neighbours[index] == null)
            return;

        if (MoveTo(_currentSlot.Neighbours[index].transform.position, _moveAnim))
        {
            _onMovement?.Invoke();
            _currentSlot.PlayerOccupant = null;
            _currentSlot = _currentSlot.Neighbours[index];
            _currentSlot.PlayerOccupant = this;
        }
    }

    private Direction GetClosestDirectionFromVector(Vector2 vector)
    {
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            if (vector.x > 0)
            {
                _moveAnim = ANIMATION_TYPE.MOVE_RIGHT;
                return Direction.Right;
            }
            else
            {
                _moveAnim = ANIMATION_TYPE.MOVE_LEFT;
                return Direction.Left;
            }
        }
        else
        {
            _moveAnim = _moveAnim == ANIMATION_TYPE.MOVE_RIGHT ? ANIMATION_TYPE.MOVE_LEFT : ANIMATION_TYPE.MOVE_RIGHT;
            if (vector.y > 0)
            {
                return Direction.Up;
            }
            else
            {
                return Direction.Down;
            }
        }
    }


    IEnumerator TestCheck(Vector3 pos)
    {
        CorrectDestination(pos + new Vector3(_areaManager.BouncerBoard.HorizontalSpacing , 0, 0));
        _qteController?.OpenBubble();
        _checkingState = CHECKING_STATE.CHECKING;
        while (true)
        {
            if (!_isInDrop && _playerController.Action1.InputValue) //ACCEPT
            {
                if ((_currentClient.StateMachine.CharacterDataObject.isTutorialNpc &&
                     _currentClient.StateMachine.TypeData.Evilness == Evilness.GOOD) ||
                    !_currentClient.StateMachine.CharacterDataObject.isTutorialNpc)
                {
                    _onAccept?.Invoke();
                    _animation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.YEAH);
                    LetCharacterEnterBox();
                    _qteController?.CloseBubble();
                    _animation.SetLatency(2);
                    _animation.SetAnim(ANIMATION_TYPE.ACCEPT, false);
                    yield break;
                }
                
            }
            if (!_isInDrop && _playerController.Action3.InputValue)//REFUSE + evil character
            {
                if ((_currentClient.StateMachine.CharacterDataObject.isTutorialNpc &&
                     _currentClient.StateMachine.TypeData.Evilness == Evilness.EVIL) ||
                    !_currentClient.StateMachine.CharacterDataObject.isTutorialNpc)
                {
                    _onRefuse?.Invoke();
                    _animation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.NO);
                    _currentClient.StateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.CHOC);
                    _currentClient.StateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.EXCLAMATION);
                    if (_currentClient.StateMachine.TypeData.Evilness == Evilness.EVIL)
                    {
                        _checkingState = CHECKING_STATE.QTE;
                        _qteController?.StartQTE(_currentClient.StateMachine.TypeData);
                    } else
                    {
                        _animation.SetLatency(2);
                        _animation.SetAnim(ANIMATION_TYPE.REFUSE, false);
                        RefuseCharacterEnterBox();
                        _qteController?.CloseBubble();
                    }
                    yield break;
                }
            }
            yield return null;
        }
    }

    public void LetCharacterEnterBox()
    {
        _checkingState = CHECKING_STATE.NONE;
        _currentClient.BouncerAction(true);
        _currentState = BOUNCER_STATE.MOVING;
        //_animation.SetAnim(ANIMATION_TYPE.IDLE);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BOUNCER).StopFocus();
        transform.position = _currentSlot.transform.position;
        _currentClient = null;
    }

    public void OnQTEStarted(){}

    public void OnQTEComplete()
    {
        RefuseCharacterEnterBox();
    }

    private void RefuseCharacterEnterBox()
    {
        _checkingState = CHECKING_STATE.NONE;
        if (_currentClient.StateMachine.CharacterDataObject.isTutorialNpc)
        {
            Globals.TutorialManager.HandledTutoCharacter++;
        }
        _currentClient.BouncerAction(false);
        _currentState = BOUNCER_STATE.MOVING;
        //_animation.SetAnim(ANIMATION_TYPE.IDLE);
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BOUNCER).StopFocus();
        transform.position = _currentSlot.transform.position;
        _currentClient = null;
    }

    public void OnQTECorrectInput()
    {
        _currentClient.StateMachine.CharacterAnimation.VfxHandeler.PlayVfx( Random.Range(0,2) == 1 ? VfxHandeler.VFX_TYPE.BLUE_IMPACT : VfxHandeler.VFX_TYPE.RED_IMPACT);
    }

    public void OnQTEWrongInput()
    {
        if (!_currentClient.StateMachine.CharacterDataObject.isTutorialNpc)
        {
            LetCharacterEnterBox();
        }
    }
    public void OnQTEMissedInput()
    {

    }

    protected override void OnBeginDrop()
    {
        _isInDrop = true;
        _qteController.OnBeginDrop();
    }

    protected override void OnDropEnd()
    {
        _isInDrop = false;
        
        _qteController.OnDropEnd(_checkingState);
    }
}
