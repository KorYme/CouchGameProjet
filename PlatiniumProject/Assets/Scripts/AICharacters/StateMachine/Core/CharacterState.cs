using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class CharacterState
{
    public CharacterStateMachine StateMachine {get; private set; }

    public virtual void InitState(CharacterStateMachine sm)
    {
        StateMachine = sm;
    }

    public virtual void EnterState()
    {
        StateMachine.Animation.ResetAnimation();
    }
    public virtual void UpdateState(){}
    public virtual void ExitState(){}
    public virtual void OnBeat()
    {
        //StateMachine.gameObject.transform.DOShakeScale(.15f, 1.25f).SetEase(Ease.InOutBounce)
            //.SetLoops(2, LoopType.Yoyo).OnComplete(() => StateMachine.gameObject.transform.localScale = Vector3.one);
        
        StateMachine.CurrentBeatAmount++;
        if (StateMachine.CurrentBeatAmount >= StateMachine.CharacterDataObject.beatAmountUnitlAction)
        {
            StateMachine.CurrentBeatAmount = 0;
            BeatAction();
        }
    }

    public virtual void BeatAction() {}
}
