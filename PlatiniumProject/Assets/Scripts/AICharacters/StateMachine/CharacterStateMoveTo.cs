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
        StateMachine.SpriteRenderer.sprite =
            StateMachine.Animation.GetAnimationSprite(CharacterAnimation.ANIMATION_TYPE.MOVING);
    }
    IEnumerator MoveRoutine()
    {
        StateMachine.CharacterMove.MoveToPosition(StateMachine.MoveToLocation, StateMachine.Animation.CharacterAnimationObject.walkAnimation.AnimationLenght);
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
