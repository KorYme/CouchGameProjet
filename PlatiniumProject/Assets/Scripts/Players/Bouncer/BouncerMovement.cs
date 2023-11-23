using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BouncerMovement : PlayerMovement, IQTEable
{
    public enum BOUNCER_STATE
    {
        MOVING,
        CHECKING,
        IDLE
    }

    [Space, Header("Bouncer Parameters")]
    [SerializeField] private AreaManager _areaManager;

    private BOUNCER_STATE _currentState = BOUNCER_STATE.MOVING;

    private SlotInformation _currentSlot;

    protected override PlayerRole PlayerRole => PlayerRole.Bouncer;
    private BouncerQTEController _qteController;

    private void Awake()
    {
        _qteController = GetComponent<BouncerQTEController>();
    }

    protected override IEnumerator Start()
    {
        yield return base.Start();
        Players.PlayersController[(int)PlayerRole].RB.OnInputChange += () =>
        {
            Debug.Log(Players.PlayersController[(int)PlayerRole].LT.InputValue);
        };
        _currentSlot = _areaManager.BouncerBoard.Board[_areaManager.BouncerBoard.BoardDimension.x
            * Mathf.Max(1,_areaManager.BouncerBoard.BoardDimension.y / 2 + _areaManager.BouncerBoard.BoardDimension.y % 2) -1];
        _currentSlot.PlayerOccupant = this;
        transform.position = _currentSlot.transform.position;
        QTEHandler qteHandler;
        TryGetComponent(out qteHandler);
        if (qteHandler != null)
        {
            qteHandler.RegisterQTEable(this);
        }
        Debug.Log("Bouncer Initialis�");
    }

    protected override void OnInputMove(Vector2 vector)
    {
        if (_currentState == BOUNCER_STATE.MOVING)
        {
            Move((int)GetClosestDirectionFromVector(vector));
        }
    }

    public void CheckMode(CharacterStateMachine chara)
    {
        _currentState = BOUNCER_STATE.CHECKING;
        StartCoroutine(TestCheck(chara.transform.position));
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BOUNCER).StartFocus(transform);
    }
    
    public void Move(int index)
    {
        if (_currentSlot.Neighbours[index] == null)
            return;

        if (MoveTo(_currentSlot.Neighbours[index].transform.position))
        {
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
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }
        else
        {
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
        while (true)
        {
            if (_playerController.Action1.InputValue) //ACCEPT
            {
                LetCharacterEnterBox();
                _qteController?.CloseBubble();
                yield break;
            }
            if (_playerController.Action3.InputValue)//REFUSE + evil character
            {
                if (_currentSlot.Occupant.TypeData.Evilness == Evilness.EVIL)
                {
                    _qteController?.StartQTE(_currentSlot.Occupant.TypeData);
                } else
                {
                    RefuseCharacterEnterBox();
                    _qteController?.CloseBubble();
                }
                yield break;
            }
            yield return null;
        }
    }

    public void LetCharacterEnterBox()
    {
        CharacterCheckByBouncerState chara = _currentSlot.Occupant.BouncerCheckState as CharacterCheckByBouncerState;
        chara.BouncerAction(true);
        _currentState = BOUNCER_STATE.MOVING;
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BOUNCER).StopFocus();
        transform.position = _currentSlot.transform.position;
    }

    public void OnQTEStarted(){}

    public void OnQTEComplete()
    {
        RefuseCharacterEnterBox();
    }

    private void RefuseCharacterEnterBox()
    {
        CharacterCheckByBouncerState chara = _currentSlot.Occupant.CurrentState as CharacterCheckByBouncerState;
        chara.BouncerAction(false);
        _currentState = BOUNCER_STATE.MOVING;
        Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BOUNCER).StopFocus();
        transform.position = _currentSlot.transform.position;
    }

    public void OnQTECorrectInput() {}

    public void OnQTEWrongInput()
    {
        LetCharacterEnterBox();
    }
}
