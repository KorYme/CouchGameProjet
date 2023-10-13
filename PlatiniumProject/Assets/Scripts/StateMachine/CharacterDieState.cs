using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterDieState : CharacterState
{
    public override void EnterState()
    {
        StateMachine.GetComponentInChildren<SpriteRenderer>().DOColor(Vector4.zero, 1.5f).SetEase(Ease.InCubic).OnComplete(() => StateMachine.DestroySelf());
    }
}
