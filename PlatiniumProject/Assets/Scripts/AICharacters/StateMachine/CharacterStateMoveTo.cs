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
        StateMachine.StartCoroutine(StateMachine.NextState == null? MoveRoutine() : MoveRoutineTP());
    }

    private void AnimationSetter()
    {
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.MOVE);
    }
    IEnumerator MoveRoutineTP()
    {
        float timer = 0f;
        float percentage = 0f;
        while (timer < StateMachine.CharacterMove.MovementData.tpDuration)
        {
            timer += Time.deltaTime;
            percentage = StateMachine.CharacterMove.MovementData.tpCurve.Evaluate(timer);
            StateMachine.CharacterAnimation.Sp.material.SetFloat("_Fade", Mathf.Lerp(1,0, percentage));
            yield return null;
        }
        StateMachine.transform.position = StateMachine.MoveToLocation;
        StateMachine.MoveToLocation = Vector2.zero;
        
        timer = 0;
        percentage = 0;
        while (timer < StateMachine.CharacterMove.MovementData.tpDuration)
        {
            timer += Time.deltaTime;
            percentage = StateMachine.CharacterMove.MovementData.tpCurve.Evaluate(timer);
            StateMachine.CharacterAnimation.Sp.material.SetFloat("_Fade", Mathf.Lerp(0,1, percentage));
            yield return null;
        }
        

        yield return new WaitUntil(() => !StateMachine.CharacterMove.IsMoving);
        
        StateMachine.ChangeState(StateMachine.NextState != null ? StateMachine.NextState : StateMachine.PreviousState);
        StateMachine.NextState = null;        
    }
    
    IEnumerator MoveRoutine()
    {
        StateMachine.CharacterMove.MoveToPosition(StateMachine.MoveToLocation);
        StateMachine.MoveToLocation = Vector2.zero;
    
        yield return new WaitUntil(() => !StateMachine.CharacterMove.IsMoving);
        
        StateMachine.ChangeState(StateMachine.NextState != null ? StateMachine.NextState : StateMachine.PreviousState);
        StateMachine.NextState = null;        
    }

    public override void ExitState()
    {
        OnAnim -= AnimationSetter;
    }
}
