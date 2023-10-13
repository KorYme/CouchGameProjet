using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateRoam : CharacterState
{
    public override void EnterState()
    {
        Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, StateMachine.qt.CircleRadius);
        StateMachine.CharacterMove.MoveToPosition(StateMachine.qt.CircleOrigin.position + (Vector3) destination);
        StateMachine.qt.RoamQueue.Add(StateMachine);
    }

    public override void OnBeat()
    {
        StateMachine.CurrentBeatAmount++;
        if (StateMachine.CurrentBeatAmount >= StateMachine.CharacterDataObject.beatAmountUnitlAction)
        {
            StateMachine.CurrentBeatAmount = 0;
            BeatAction();
        }
    }

    private void BeatAction()
    {
        if (StateMachine.qt.RoamQueue[0] == StateMachine)
        {
            StateMachine.ChooseWaitingLine();
            StateMachine.ChangeState(StateMachine.BarManQueueState);
        }
        else
        {
            Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, StateMachine.qt.CircleRadius);
            StateMachine.CharacterMove.MoveToPosition(StateMachine.qt.CircleOrigin.position + (Vector3) destination);
        }
    }
}
