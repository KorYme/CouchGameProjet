using System;
using UnityEngine;


public class CharacterStateExorcize : CharacterState
{
    public Action OnCharacterStartExorcize;

    public override void EnterState()
    {
        OnCharacterStartExorcize?.Invoke();
    }
    
    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.EXORCIZE);
    }
    
    
}
