using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class CharacterState
{
    public CharacterStateMachine StateMachine {get; private set; }

    public bool CanLetMeMove => Globals.DropManager.CanYouLetMeMove;

    public virtual void InitState(CharacterStateMachine sm)
    {
        StateMachine = sm;
    }

    public virtual void EnterState()
    {
        StateMachine.Animation.ResetLastAnimation();
    }
    public virtual void UpdateState(){}
    public virtual void ExitState(){}
    public virtual void OnBeat()
    {
        if(!CanLetMeMove)
            return;
        
        StateMachine.CurrentBeatAmount++;
        if (StateMachine.CurrentBeatAmount >= StateMachine.CharacterDataObject.beatAmountUnitlAction)
        {
            StateMachine.CurrentBeatAmount = 0;
            BeatAction();
        }
    }

    public virtual void BeatAction() {}
}
