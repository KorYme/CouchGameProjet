using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMoveTo : CharacterState
{
    public override void EnterState()
    {
        //StateMachine.transform.position = StateMachine.MoveToLocation.position;
        StateMachine.CharacterMove.MoveToPosition(StateMachine.MoveToLocation);
        StateMachine.MoveToLocation = Vector2.zero;
        StateMachine.ChangeState(StateMachine.IdleState);
    }

}
