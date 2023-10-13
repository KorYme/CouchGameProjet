using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateIdle : CharacterState
{
    List<SlotInformation> availableSlots;
    Vector2[] direction = new Vector2[4] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
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
            StateMachine.MoveToLocation = StateMachine.CurrentSlot.Next.transform;
            StateMachine.ChangeState(StateMachine.MoveToState);
            StateMachine.CurrentSlot = StateMachine.CurrentSlot.Next;
                
            return;
        }

        switch (StateMachine.CharacterCurrentObjective)
        {
            case CharacterStateMachine.CharacterObjective.None:
                break;
            case CharacterStateMachine.CharacterObjective.Bouncer:
                if (StateMachine.CurrentTransitQueue != null && StateMachine.CurrentSlot == StateMachine.CurrentTransitQueue[^1])
                {
                    StateMachine.CurrentTransitQueue = null;
                    StateMachine.MoveToLocation = StateMachine.qt.Bouncer[0,0].transform;
                    StateMachine.ChangeState(StateMachine.MoveToState);
                    StateMachine.CurrentChekerBoardId = Vector2.zero;
                    return;
                }
                else
                {
                    availableSlots = new List<SlotInformation>();
                    for (int i = 0; i < 4; ++i)
                    {
                        SlotInformation slot =
                            StateMachine.qt.GetSlot(StateMachine.CurrentChekerBoardId + direction[i]);

                        if (slot != null)
                        {
                            availableSlots.Add(slot);
                        }
                    }
                    if(availableSlots.Count <= 0)
                        return;
                    
                    SlotInformation choicedSlot = availableSlots[Random.Range(0, availableSlots.Count)];
                    StateMachine.MoveToLocation = choicedSlot.transform;
                    StateMachine.ChangeState(StateMachine.MoveToState);
                    StateMachine.CurrentChekerBoardId = choicedSlot.Id;
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
