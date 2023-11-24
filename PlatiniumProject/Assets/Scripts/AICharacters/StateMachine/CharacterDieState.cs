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
        StateMachine.CharacterMove.MoveTo(Globals.ExitPoints.FindClosestExitPoint(StateMachine.transform.position));
        StateMachine.StartCoroutine(ExitRoutine());
    }

    IEnumerator ExitRoutine()
    {
        yield return new WaitUntil(() => !StateMachine.CharacterMove.IsMoving);
        StateMachine.GoBackInPull();
    }
}
