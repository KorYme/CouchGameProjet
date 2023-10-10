using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateIdle : CharacterState
{
    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnBeat()
    {
        if (StateMachine.CurrentSlot.Next.Occupant == null)
        {
            StateMachine.ChangeState(StateMachine.MoveToState);
            StateMachine.MoveToLocation = StateMachine.CurrentSlot.Next.Occupant.transform;
        }
    }
}
