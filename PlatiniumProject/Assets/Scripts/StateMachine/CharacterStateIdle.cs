using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateIdle : CharacterState
{
    List<SlotInformation> availableSlots;
    public override void OnBeat()
    {
        StateMachine.CurrentBeatAmount++;
        if (StateMachine.CurrentBeatAmount >= StateMachine.CharacterDataObject.beatAmountUnitlAction)
        {
            StateMachine.CurrentBeatAmount = 0;
            BeatAction();
        }

        Debug.Log("Beat");
    }

    private void BeatAction()
    {
        Debug.Log("BeatAction");
        if (StateMachine.CurrentTransitQueue != null && StateMachine.CurrentSlot != StateMachine.qt.Transit[^1] && StateMachine.CurrentSlot.Next.Occupant == null )
        {
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.MoveToLocation = StateMachine.CurrentSlot.Next.transform.position;
            StateMachine.ChangeState(StateMachine.MoveToState);
            StateMachine.CurrentSlot = StateMachine.CurrentSlot.Next;
            StateMachine.CurrentSlot.Occupant = StateMachine.gameObject;
                
            return;
        }

        switch (StateMachine.CharacterCurrentObjective)
        {
            case CharacterStateMachine.CharacterObjective.None:
                break;
            case CharacterStateMachine.CharacterObjective.Bouncer:

                if (StateMachine.CurrentMovementInBouncer > StateMachine.CharacterDataObject.movementAmountInQueue)
                {
                    StateMachine.CurrentSlot.Occupant = null;
                    StateMachine.ChangeState(StateMachine.RoamState);
                    return;
                }
                if (StateMachine.CurrentTransitQueue != null && StateMachine.CurrentSlot == StateMachine.CurrentTransitQueue[^1])
                {
                    StateMachine.CurrentTransitQueue = null;
                    StateMachine.MoveToLocation = StateMachine.qt.Bouncer[0,0].transform.position;
                    StateMachine.CurrentSlot.Occupant = null;
                    StateMachine.ChangeState(StateMachine.MoveToState);
                    StateMachine.CurrentChekerBoardId = Vector2.zero;
                    StateMachine.CurrentMovementInBouncer++;
                    StateMachine.CurrentSlot.Occupant = StateMachine.gameObject;
                    return;
                }
                else
                {
                    // availableSlots = new List<SlotInformation>();
                    // for (int i = 0; i < 4; ++i)
                    // {
                    //     SlotInformation slot =
                    //         StateMachine.qt.GetSlot(StateMachine.CurrentChekerBoardId + direction[i]);
                    //
                    //     if (slot != null)
                    //     {
                    //         availableSlots.Add(slot);
                    //     }
                    // }
                    if(availableSlots.Count <= 0)
                        return;
                    
                    SlotInformation choicedSlot = availableSlots[Random.Range(0, availableSlots.Count)];
                    StateMachine.MoveToLocation = choicedSlot.transform.position;
                    StateMachine.CurrentSlot.Occupant = null;
                    StateMachine.ChangeState(StateMachine.MoveToState);
                    StateMachine.CurrentChekerBoardId = choicedSlot.Id;
                    StateMachine.CurrentMovementInBouncer++;
                    StateMachine.CurrentSlot.Occupant = StateMachine.gameObject;
                    return;
                }
                
                break;
            case CharacterStateMachine.CharacterObjective.BarTender:
                break;
            case CharacterStateMachine.CharacterObjective.DanceFloor:
                break;
            case CharacterStateMachine.CharacterObjective.Leave:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
