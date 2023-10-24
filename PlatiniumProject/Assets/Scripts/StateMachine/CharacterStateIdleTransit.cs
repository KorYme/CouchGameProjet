using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateIdleTransit : CharacterState
{
    List<SlotInformation> availableSlots;

    public override void BeatAction()
    {
        if (StateMachine.CurrentSlot == StateMachine.AreaManager.BouncerTransit.Slots[^1] && StateMachine.AreaManager.BouncerBoard.Board[0].Occupant == null)
        {
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.MoveToLocation = StateMachine.AreaManager.BouncerBoard.Board[0].transform.position;
            StateMachine.CurrentSlot = StateMachine.AreaManager.BouncerBoard.Board[0];
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
