using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMoveTo : CharacterState
{
    public override void EnterState()
    {
        StateMachine.CharacterMove.MoveToPosition(StateMachine.MoveToLocation);
        StateMachine.MoveToLocation = Vector2.zero;
        
        StateMachine.ChangeState(StateMachine.NextState != null ? StateMachine.NextState : StateMachine.PreviousState);
        StateMachine.NextState = null;
    }

}
