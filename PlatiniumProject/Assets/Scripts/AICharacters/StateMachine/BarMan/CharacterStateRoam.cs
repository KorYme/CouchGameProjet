using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStateRoam : CharacterState
{
    private Action OnAnim;
    public override void EnterState()
    {
        base.EnterState();
        Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, StateMachine.AreaManager.CircleRadius);
        StateMachine.CharacterMove.MoveTo(StateMachine.AreaManager.CircleOrigin.position + (Vector3) destination);
        StateMachine.AreaManager.RoamQueue.Add(StateMachine);
        
    }
    public override void OnBeat()
    {
        StateMachine.CurrentBeatAmount++;
        StateMachine.Animation.SetAnim(ANIMATION_TYPE.IDLE);
        if (StateMachine.CurrentBeatAmount >= StateMachine.CharacterDataObject.beatAmountUnitlAction)
        {
            StateMachine.CurrentBeatAmount = 0;
            BeatAction();
        }
    }
    
    private void FindLine()
    {
        if (StateMachine.WaitingLines.Length > 0) 
        {
            int indexLine = 0;
            int nbCharactersInLine = StateMachine.WaitingLines[0].NbCharactersWaiting;
        
            for (int i = 1; i < StateMachine.WaitingLines.Length; i++)
            {
                if (nbCharactersInLine > StateMachine.WaitingLines[i].NbCharactersWaiting) {
                    nbCharactersInLine = StateMachine.WaitingLines[i].NbCharactersWaiting;
                    indexLine = i;
                }
            }
            StateMachine.WaitingLines[indexLine].AddToWaitingLine(StateMachine);
            StateMachine.CurrentWaitingLine = StateMachine.WaitingLines[indexLine];
        }
    }

    public bool AreLinesFree()
    {
        foreach (var line in StateMachine.WaitingLines)
        {
            if (!line.IsFull)
                return true;
        }
        return false;
    }

    public override void BeatAction()
    {
        if (StateMachine.AreaManager.RoamQueue[0] == StateMachine && AreLinesFree())
        {
            StateMachine.AreaManager.RoamQueue.Remove(StateMachine);
            StateMachine.ChangeState(StateMachine.BarManQueueState);
        }
        else
        {
            Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, StateMachine.AreaManager.CircleRadius);
            StateMachine.CharacterMove.MoveTo(StateMachine.AreaManager.CircleOrigin.position + (Vector3) destination);
        }
    }
}
