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
        StateMachine.CurrentBeatAmount = 0;
        if (StateMachine.CharacterDataObject.isTutorialNpc)
        {
            Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.BARMAN).SetShadowMaterial(false);
        }
        base.EnterState();
        Vector2 destination = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0f, StateMachine.AreaManager.CircleRadius);
        StateMachine.CharacterMove.MoveTo(StateMachine.AreaManager.CircleOrigin.position + (Vector3) destination, true);
        StateMachine.AreaManager.RoamQueue.Add(StateMachine);
        
    }
    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.IDLE);
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
