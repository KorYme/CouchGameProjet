using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMoveTo : CharacterState
{
    private Action OnAnim;
    public override void EnterState()
    {
        base.EnterState();
        OnAnim += AnimationSetter;
        StateMachine.StartCoroutine(MoveRoutine());
    }

    private void AnimationSetter()
    {
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.MOVE);
    }
    IEnumerator MoveRoutine()
    {
        StateMachine.CharacterMove.MoveToPosition(StateMachine.MoveToLocation, StateMachine.UseTp);
        StateMachine.MoveToLocation = Vector2.zero;
    
        yield return new WaitUntil(() => !StateMachine.CharacterMove.IsMoving);
        
        StateMachine.ChangeState(StateMachine.NextState != null ? StateMachine.NextState : StateMachine.PreviousState);
        StateMachine.NextState = null;        
    }

    public override void ExitState()
    {
        StateMachine.UseTp = false;
        OnAnim -= AnimationSetter;
    }
}
