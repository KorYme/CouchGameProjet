using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateIdleTransit : CharacterState
{
    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.Animation.SetAnim(ANIMATION_TYPE.IDLE);
    }

    public override void BeatAction()
    {
        if (StateMachine.CurrentSlot == StateMachine.AreaManager.BouncerTransit.Slots[^1] && StateMachine.AreaManager.BouncerBoard.AreAnyEntryFree())
        {
            SlotInformation newSlot = StateMachine.AreaManager.BouncerBoard.GetFreeEntrySlot();
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.MoveToLocation = newSlot.transform.position;
            StateMachine.CurrentSlot = newSlot;
            StateMachine.CurrentSlot.Occupant = StateMachine;
            StateMachine.NextState = StateMachine.IdleBouncerState;

            StateMachine.ChangeState(StateMachine.MoveToState);
            return;
        }
        else if (StateMachine.CurrentSlot.Next != null && StateMachine.CurrentSlot.Next.Occupant == null)
        {
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.MoveToLocation = StateMachine.CurrentSlot.Next.transform.position;
            StateMachine.CurrentSlot = StateMachine.CurrentSlot.Next;
            StateMachine.CurrentSlot.Occupant = StateMachine;

            StateMachine.ChangeState(StateMachine.MoveToState);
        }
    }
}
