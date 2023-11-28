using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateIdleBouncer : CharacterState
{
    List<SlotInformation> availableSlots;

    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.Animation.SetAnim(ANIMATION_TYPE.IDLE);
    }

    public override void UpdateState()
    {
        if (StateMachine.CurrentSlot.PlayerOccupant != null)
        {
            StateMachine.ChangeState(StateMachine.BouncerCheckState);
            StateMachine.CurrentSlot.PlayerOccupant.CheckMode(StateMachine, StateMachine.BouncerCheckState as CharacterCheckByBouncerState);
        }
    }

    public override void BeatAction()
    {

        if (!StateMachine.CharacterDataObject.isTutorialNpc && StateMachine.CurrentMovementInBouncer > StateMachine.CharacterDataObject.movementAmountInQueue)
        {
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.ChangeState(StateMachine.RoamState);
            StateMachine.NextState = StateMachine.BarManQueueState;
        }
        else
        {
            SlotInformation newSlot = StateMachine.AreaManager.BouncerBoard.GetNextBouncerSlot(StateMachine.CurrentSlot);
            StateMachine.CurrentMovementInBouncer++;

            if (newSlot == null)
                return;

            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.MoveToLocation = newSlot.transform.position;
            StateMachine.CurrentSlot = newSlot;
            StateMachine.CurrentSlot.Occupant = StateMachine;

            StateMachine.ChangeState(StateMachine.MoveToState);
        }

    }
}
