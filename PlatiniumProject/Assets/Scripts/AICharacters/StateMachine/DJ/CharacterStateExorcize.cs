using UnityEngine;


public class CharacterStateExorcize : CharacterState
{
    public override void EnterState()
    {
        Globals.DropManager.OnDropSuccess += RemovePriest;
    }
    
    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.Animation.SetAnim(ANIMATION_TYPE.EXORCIZE);
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
