using UnityEngine;

public class CharacterStateDancing : CharacterState
{
    int _currentSatisfaction;
    
    public override void EnterState()
    {
        base.EnterState();
        StateMachine.Satisafaction.InitializeStatisfaction(StateMachine.CharacterDataObject.maxSatisafactionDJ);
        StateMachine.Satisafaction.OnSatsifactionZero += RunOutOfSatisfaction;

        Globals.PriestCalculator.OnPriestNearToExorcize += StartExorcize;
        
        if (StateMachine.CharacterTypeData.Evilness == Evilness.EVIL &&
            (Globals.PriestCalculator.ExorcizeState == PriestCalculator.EXORCIZE_STATE.EXORCIZING || Globals.PriestCalculator.ExorcizeState == PriestCalculator.EXORCIZE_STATE.EXORCIZED))
        {
            StateMachine.ChangeState(StateMachine.ExorcizeState);
        }
    }

    public override void OnBeat()
    {
        if(Globals.DropManager.CanYouLetMeMove)
            return;
        
        StateMachine.Animation.SetAnim(ANIMATION_TYPE.DANCING);
        if (!StateMachine.CurrentSlot.IsEnlighted)
        {
            StateMachine.Satisafaction.DecreaseSatisfaction(StateMachine.CharacterDataObject.decrementationValueOnFloor);
        }
    }

    private void StartExorcize()
    {
        if (StateMachine.CharacterTypeData.Evilness == Evilness.EVIL)
        {
            StateMachine.ChangeState(StateMachine.ExorcizeState);
        }
    }

    private void RunOutOfSatisfaction()
    {
        StateMachine.ChangeState(StateMachine.DieState);
    }

    public void OnQTECorrectInput(int nbTotalInputs)
    {
        if (StateMachine.CurrentSlot.IsEnlighted)
        {
            StateMachine.Satisafaction.IncreaseSatisfaction(StateMachine.CharacterDataObject.maxSatisafactionDJ * 1 / nbTotalInputs);
        }
    }

    public override void ExitState()
    {
        StateMachine.Satisafaction.OnSatsifactionZero -= RunOutOfSatisfaction;
        Globals.PriestCalculator.OnPriestNearToExorcize -= StartExorcize;
    }
}
