using Rewired;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] PlayerRole _gamePlayerRole;
    public Rewired.Player newPlayer { get; private set; }

    #region InputsActions
    //Button Inputs
    public InputBool Action1 { get; private set; } = new(RewiredConsts.Action.ACTION1);
    public InputBool Action2 { get; private set; } = new(RewiredConsts.Action.ACTION2);
    public InputBool Action3 { get; private set; } = new(RewiredConsts.Action.ACTION3);
    public InputBool Action4 { get; private set; } = new(RewiredConsts.Action.ACTION4);
    public InputFloat RT { get; private set; } = new(RewiredConsts.Action.RT);
    public InputFloat LT { get; private set; } = new(RewiredConsts.Action.LT);
    public InputBool RB { get; private set; } = new(RewiredConsts.Action.RB);
    public InputBool LB { get; private set; } = new(RewiredConsts.Action.LB);
    public InputBool Pause { get; private set; } = new(RewiredConsts.Action.PAUSE);

    //Axis Inputs
    public InputVector2 LeftJoystick { get; private set; } = new(RewiredConsts.Action.MOVE_HORIZONTAL, RewiredConsts.Action.MOVE_VERTICAL);
    public InputVector2 RightJoystick { get; private set; } = new(RewiredConsts.Action.AXISX, RewiredConsts.Action.AXISY);

    //Container with all Input Classes
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
        Pause,
    };
    #endregion

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ReInput.isReady && PlayerInputsAssigner.GetRewiredPlayerByRole(_gamePlayerRole) != null);
        SetPlayer();
        Players.AddPlayerToList(this, (int)_gamePlayerRole);
        Pause.OnInputStart += () => Globals.GameManager.AssignPlayerToPauseMenuAndPause((int)_gamePlayerRole);
    }

    private void OnDestroy()
    {
        Pause.OnInputStart = null;
    }

    public void SetPlayer()
    {
        newPlayer = PlayerInputsAssigner.GetRewiredPlayerByRole(_gamePlayerRole);
        if (newPlayer != null)
            SetUpAllInputClasses();
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

    public InputClass GetInputClassWithID(int ActionID, bool getParentClass = false)
    {
        InputClass returnValue = null;
        _allMainInputClasses.ForEach(inputClass =>
        {
            switch (inputClass)
            {
                case InputVector2 inputVector2:
                    if (inputVector2.InputClassX.ActionID == ActionID)
                    {
                        returnValue = getParentClass ? inputVector2 : inputVector2.InputClassX;
                        return;
                    }
                    if (inputVector2.InputClassY.ActionID == ActionID)
                    {
                        returnValue = getParentClass ? inputVector2 : inputVector2.InputClassY;
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
