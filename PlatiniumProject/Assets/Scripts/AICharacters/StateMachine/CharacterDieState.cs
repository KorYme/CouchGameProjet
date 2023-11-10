using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterDieState : CharacterState
{
    public override void EnterState()
    {
        base.EnterState();
        StateMachine.OnCharacterDeath?.Invoke();
        if (StateMachine.CurrentSlot != null)
        {
            StateMachine.CurrentSlot.Occupant = null;
        }
        
        StateMachine.GoBackInPull();
    }
}
