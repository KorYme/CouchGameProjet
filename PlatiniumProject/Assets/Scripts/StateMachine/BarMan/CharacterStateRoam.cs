using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateRoam : CharacterState
{
    public override void EnterState()
    {
        Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, StateMachine.AreaManager.CircleRadius);
        StateMachine.CharacterMove.MoveToPosition(StateMachine.AreaManager.CircleOrigin.position + (Vector3) destination);
        StateMachine.AreaManager.RoamQueue.Add(StateMachine);
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

    public override void BeatAction()
    {
        if (StateMachine.AreaManager.RoamQueue[0] == StateMachine)
        {
            StateMachine.AreaManager.RoamQueue.Remove(StateMachine);
            StateMachine.ChangeState(StateMachine.BarManQueueState);
        }
        else
        {
            Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, StateMachine.AreaManager.CircleRadius);
            StateMachine.CharacterMove.MoveToPosition(StateMachine.AreaManager.CircleOrigin.position + (Vector3) destination);
        }
    }
}
