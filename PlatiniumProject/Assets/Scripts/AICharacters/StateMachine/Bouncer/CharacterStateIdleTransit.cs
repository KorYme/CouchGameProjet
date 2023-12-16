using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateIdleTransit : CharacterState
{
    public override void OnBeat()
    {
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.IDLE);
        if(!CanLetMeMove)
            return;
        
        StateMachine.CurrentBeatAmount++;
        if (StateMachine.CurrentBeatAmount >= StateMachine.CharacterDataObject.transitNeatAmountUnitlAction)
        {
            StateMachine.CurrentBeatAmount = 0;
            BeatAction();
        }
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
            StateMachine.UseTp = true;

            StateMachine.ChangeState(StateMachine.MoveToState);
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
