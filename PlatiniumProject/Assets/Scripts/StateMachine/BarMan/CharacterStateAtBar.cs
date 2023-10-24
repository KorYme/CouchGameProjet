using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateAtBar : CharacterState
{
    private int _currentSatisfaction;

    public override void EnterState()
    {
        _currentSatisfaction = StateMachine.CharacterDataObject.maxSatisafactionBar;
    }

    public override void OnBeat()
    {
        _currentSatisfaction--;
        if (_currentSatisfaction <= 0)
        {
            StateMachine.CurrentWaitingLine.OnFailDrink();
            StateMachine.ChangeState(StateMachine.DieState);
        }
    }
    
}
