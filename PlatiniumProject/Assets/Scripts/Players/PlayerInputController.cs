using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] int gamePlayerId = 0;
    private Rewired.Player player { get { return PlayerInputsAssigner.GetRewiredPlayer(gamePlayerId); } }
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
    public InputBool RT { get; private set; } = new(RewiredConsts.Action.RT);
    public InputBool LT { get; private set; } = new(RewiredConsts.Action.LT);

    //Axis Inputs
    public InputVector2 LeftJoystick { get; private set; } = new(RewiredConsts.Action.MOVE_HORIZONTAL, RewiredConsts.Action.MOVE_VERTICAL);
    public InputVector2 RightJoystick { get; private set; } = new(RewiredConsts.Action.AXISX, RewiredConsts.Action.AXISY);


    List<InputClass> _allInputClasses => new List<InputClass>()
    {
        Action1,
        Action2,
        Action3,
        Action4,
        RT,
        LT,
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
        yield return new WaitUntil(() => ReInput.isReady);
        Debug.Log("Rewired ready");
        newPlayer = PlayerInputsAssigner.GetRewiredPlayer(gamePlayerId);
        if (newPlayer != null)
        {
            _isRegistered = true;
            Players.AddPlayerToList(this, (int) PlayerInputsAssigner.GetRolePlayer(gamePlayerId));
            SetUpAllInputClasses();
        }
        GetInputs();
    }

    void SetUpAllInputClasses()
    {
        _allInputClasses.ForEach(inputClass =>
        {
            newPlayer.AddInputEventDelegate(inputClass.InputCallback, UpdateLoopType.Update, inputClass.ActionID);
            switch (inputClass)
        {
                case InputVector2 inputVector2:
                    Debug.Log("Add Vector2 delegate");
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

    public InputClass GetInputClassWithID(int ActionID) => _allInputClasses.FirstOrDefault(x => x.ActionID == ActionID);
}
