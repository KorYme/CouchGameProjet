using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMoveTo : CharacterState
{
    public override void EnterState()
    {
        StateMachine.transform.position = StateMachine.MoveToLocation.position;
        StateMachine.MoveToLocation = null;
        StateMachine.ChangeState(StateMachine.IdleState);
    }

}
