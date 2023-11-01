using Rewired;
using Rewired.Utils.Classes.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] PlayerRole _gamePlayerRole;
    private Rewired.Player player { get { return PlayerInputsAssigner.GetRewiredPlayer((int)_gamePlayerRole); } }
    public Rewired.Player newPlayer { get; private set; } 
    private bool _isRegistered = false;

    #region InputsMovement
    public Vector2 MoveVector {  get; private set; } = Vector2.zero;
    public bool OnMoveDown { get; private set; } = false;
    private bool _isMoveDownRefreshed = true;
    public Vector2 MoveDirection {  get; private set; } = Vector2.zero;
    public event Action OnAxisMoveStarted;
    #endregion

    #region InputsActions
    public bool IsAction1Pressed { get; private set; } = false;
    public float DurationAction1Down { get; private set; } = 0f;
    #endregion

    #region NewInputsActions
    //Button Inputs
    public InputBool Action1 { get; private set; } = new(RewiredConsts.Action.ACTION1);
    public InputBool Action2 { get; private set; } = new(RewiredConsts.Action.ACTION2);
    public InputBool Action3 { get; private set; } = new(RewiredConsts.Action.ACTION3);
    public InputBool Action4 { get; private set; } = new(RewiredConsts.Action.ACTION4);
    public InputFloat RT { get; private set; } = new(RewiredConsts.Action.RT);
    public InputFloat LT { get; private set; } = new(RewiredConsts.Action.LT);
    public InputBool RB { get; private set; } = new(RewiredConsts.Action.RB);
    public InputBool LB { get; private set; } = new(RewiredConsts.Action.LB);

    //Axis Inputs
    public InputVector2 LeftJoystick { get; private set; } = new(RewiredConsts.Action.MOVE_HORIZONTAL, RewiredConsts.Action.MOVE_VERTICAL);
    public InputVector2 RightJoystick { get; private set; } = new(RewiredConsts.Action.AXISX, RewiredConsts.Action.AXISY);


    List<InputClass> _allMainInputClasses => new List<InputClass>()
    {
        Action1,
        Action2,
        Action3,
        Action4,
        RT,
        LT,
        RB,
        LB,
        LeftJoystick,
        RightJoystick,
    };
    #endregion

    private void Start()
    {
        StartCoroutine(SetUpRewiredCoroutine());
    }

    IEnumerator SetUpRewiredCoroutine()
    {
        yield return new WaitUntil(() => ReInput.isReady && player != null);
        Debug.Log("Rewired ready");
        newPlayer = PlayerInputsAssigner.GetRewiredPlayer((int)_gamePlayerRole);

        if (newPlayer != null)
        {
            _isRegistered = true;
            Players.AddPlayerToList(this, (int) PlayerInputsAssigner.GetRolePlayer((int)_gamePlayerRole));
            SetUpAllInputClasses();
        }
    }

    void SetUpAllInputClasses()
    {
        _allMainInputClasses.ForEach(inputClass =>
        {
            newPlayer.AddInputEventDelegate(inputClass.InputCallback, UpdateLoopType.Update, inputClass.ActionID);
            switch (inputClass)
            {
                case InputVector2 inputVector2:
                    inputVector2.Player = newPlayer;
                    newPlayer.AddInputEventDelegate(inputVector2.InputCallbackSecondAction, UpdateLoopType.Update, inputVector2.SecondActionID);
                    break;
                default:
                    break;
            }
        });
    }

    public bool GetInput(UnitInput input)
    {
        return player == null ? false : player.GetButtonDown(input.ActionIndex);
    }
    public bool GetInputHold(UnitInput input)
    {
        return player == null ? false : player.GetButtonLongPress(input.ActionIndex);
    }

    public InputClass GetInputClassWithID(int ActionID)
    {
        InputClass returnValue = null;
        _allMainInputClasses.ForEach(inputClass =>
        {
            switch (inputClass)
            {
                case InputVector2 inputVector2:
                    if (inputVector2.InputClassX.ActionID == ActionID)
                    {
                        returnValue = inputVector2.InputClassX;
                        return;
                    }
                    if (inputVector2.InputClassY.ActionID == ActionID)
                    {
                        returnValue = inputVector2.InputClassY;
                        return;
                    }
                    break;
                default:
                    if (inputClass.ActionID == ActionID)
                    {
                        returnValue = inputClass;
                        return;
                    }
                    break;
            }
        });
        return returnValue;
    }
}
