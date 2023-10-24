using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateIdleBouncer : CharacterState
{
    List<SlotInformation> availableSlots;

    public override void BeatAction()
    {

        if (StateMachine.CurrentMovementInBouncer > StateMachine.CharacterDataObject.movementAmountInQueue)
        {
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.ChangeState(StateMachine.RoamState);
            StateMachine.NextState = StateMachine.BarManQueueState;
        }
        else
        {
            SlotInformation newSlot = StateMachine.AreaManager.BouncerBoard.GetNextBouncerSlot(StateMachine.CurrentSlot);

            if (newSlot == null)
                return;

            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.MoveToLocation = newSlot.transform.position;
            StateMachine.CurrentSlot = newSlot;
            StateMachine.CurrentMovementInBouncer++;
            StateMachine.CurrentSlot.Occupant = StateMachine;

            StateMachine.ChangeState(StateMachine.MoveToState);
        }

    }
}
