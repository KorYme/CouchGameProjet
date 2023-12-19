using System;
using UnityEngine;

public class CharacterStateDancingValidate : CharacterState
{
    public override void OnBeat()
    {
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.DANCING);
        StateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.SATISAFCTION);
    }

}
