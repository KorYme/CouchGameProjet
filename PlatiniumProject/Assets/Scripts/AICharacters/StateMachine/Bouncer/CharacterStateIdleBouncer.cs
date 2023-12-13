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
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.IDLE);
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
        
        if (!StateMachine.CharacterDataObject.isTutorialNpc && StateMachine.CurrentMovementInBouncer >
            (int)(StateMachine.CharacterDataObject.movementAmountInQueue * (2f / 3f)))
        {
            StateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ANGRY2);
        }
        else if (!StateMachine.CharacterDataObject.isTutorialNpc && StateMachine.CurrentMovementInBouncer >
                 (int)(StateMachine.CharacterDataObject.movementAmountInQueue * (1f / 3f)))
        {
            StateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ANGRY);
        }

        if (!StateMachine.CharacterDataObject.isTutorialNpc && StateMachine.CurrentMovementInBouncer > StateMachine.CharacterDataObject.movementAmountInQueue)
        {
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.UseTp = true;
            StateMachine.ChangeState(StateMachine.RoamState);
            StateMachine.NextState = StateMachine.BarManQueueState;
            StateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY);
            StateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY2);
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
