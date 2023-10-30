using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterDieState : CharacterState
{
    public override void EnterState()
    {
        if (StateMachine.CurrentSlot != null)
        {
            StateMachine.CurrentSlot.Occupant = null;
        }
        StateMachine.SpriteRenderer.DOColor(Vector4.zero, 1.5f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            StateMachine.GoBackInPull();
        });
    }
}
