using System;
using UnityEngine;


public class CharacterStateExorcize : CharacterState
{
    public Action OnCharacterStartExorcize;

    public override void EnterState()
    {
        Globals.DropManager.OnDropSuccess += RemovePriest;
        OnCharacterStartExorcize?.Invoke();
    }
    
    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.EXORCIZE);
    }

    private void RemovePriest()
    {
        StateMachine.ChangeState(StateMachine.DieState);
    }

    public override void ExitState()
    {
        Globals.DropManager.OnDropSuccess -= RemovePriest;
    }
}
