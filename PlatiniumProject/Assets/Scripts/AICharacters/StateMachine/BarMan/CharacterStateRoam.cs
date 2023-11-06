using System;
using System.Collections;
using System.Collections.Generic;
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
        StateMachine.SpriteRenderer.sprite = StateMachine.Animation.GetAnimationSprite(CharacterAnimation.ANIMATION_TYPE.IDLE);
        //Debug.Log("Beat");
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
            StateMachine.CharacterMove.MoveTo(StateMachine.AreaManager.CircleOrigin.position + (Vector3) destination);
        }
    }
}
