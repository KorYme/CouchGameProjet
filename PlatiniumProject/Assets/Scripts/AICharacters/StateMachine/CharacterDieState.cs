using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterDieState : CharacterState
{
    public override void EnterState()
    {
        base.EnterState();
        StateMachine.OnCharacterDeath?.Invoke();
        if (StateMachine.CurrentSlot != null)
        {
            StateMachine.CurrentSlot.Occupant = null;
        }

        if (StateMachine.TypeData.Evilness == Evilness.GOOD)
        {
            StateMachine.CharacterMove.MoveTo(Globals.ExitPoints.FindClosestExitPoint(StateMachine.transform.position));
        }
        else
        {
            StateMachine.CharacterAnimation.SetFullAnim(ANIMATION_TYPE.DIE, 1f);
        }
        StateMachine.StartCoroutine(ExitRoutine());
    }

    IEnumerator ExitRoutine()
    {
        if (StateMachine.TypeData.Evilness == Evilness.GOOD)
        {
            yield return new WaitUntil(() => !StateMachine.CharacterMove.IsMoving);
        }
        else
        {
            yield return new WaitUntil(() => !StateMachine.CharacterAnimation.IsAnimationPlaying);
        }
        StateMachine.GoBackInPull();
    }
}
