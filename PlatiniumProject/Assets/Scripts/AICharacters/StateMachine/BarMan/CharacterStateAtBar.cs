using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateAtBar : CharacterState
{
    private int _currentSatisfaction;

    public override void EnterState()
    {
        base.EnterState();
        StateMachine.Satisafaction.InitializeStatisfaction(StateMachine.CharacterDataObject.maxBarManSatisafactionBar);
        StateMachine.Satisafaction.OnSatsifactionZero += RunOutOfSatisfaction;
    }

    private void RunOutOfSatisfaction()
    {
        StateMachine.ChangeState(StateMachine.DieState);
        StateMachine.CurrentWaitingLine.OnFailDrink();
    }
    public override void OnBeat()
    {
        StateMachine.Satisafaction.DecreaseSatisfaction(StateMachine.CharacterDataObject.decrementationValueOnBarMan);
        StateMachine.Animation.SetAnim(ANIMATION_TYPE.IDLE);
    }

    public override void ExitState()
    {
        StateMachine.Satisafaction.OnSatsifactionZero -= RunOutOfSatisfaction;
    }
}
