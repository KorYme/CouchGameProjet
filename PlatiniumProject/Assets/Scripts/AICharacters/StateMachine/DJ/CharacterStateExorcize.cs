using System;
using UnityEngine;


public class CharacterStateExorcize : CharacterState
{
    public Action OnCharacterStartExorcize;
    private int i;

    public override void EnterState()
    {
        OnCharacterStartExorcize?.Invoke();
    }
    
    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.EXORCIZE);
        i++;
        if (i % 2 == 0)
        {
            i = 0;
            StateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.INCANTATION);
        }
    }
    
    
}
