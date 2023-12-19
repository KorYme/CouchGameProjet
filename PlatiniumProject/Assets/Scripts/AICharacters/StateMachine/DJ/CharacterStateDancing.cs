using System;
using UnityEngine;

public class CharacterStateDancing : CharacterState
{
    int _currentSatisfaction;
    
    public override void EnterState()
    {
        if (StateMachine.CharacterDataObject.isTutorialNpc)
        {
            Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DJ).SetShadowMaterial(false);
            Globals.CameraProfileManager.FindCamera(CAMERA_TYPE.DANCEFLOOR).SetShadowMaterial(false);
        }
        base.EnterState();
        StateMachine.Satisafaction.InitializeStatisfaction(StateMachine.CharacterDataObject.maxSatisafactionDJ, StateMachine.CharacterDataObject.satisfactionAmountToGetLoyal, StateMachine.RandomiseSatisfaction);
        StateMachine.Satisafaction.OnSatsifactionZero += RunOutOfSatisfaction;

        Globals.PriestCalculator.OnPriestNearToExorcize += StartExorcize;
        
        if (StateMachine.CharacterTypeData.Evilness == Evilness.EVIL &&
            (Globals.PriestCalculator.ExorcizeState == PriestCalculator.EXORCIZE_STATE.EXORCIZING || Globals.PriestCalculator.ExorcizeState == PriestCalculator.EXORCIZE_STATE.EXORCIZED))
        {
            StateMachine.ChangeState(StateMachine.ExorcizeState);
        }

        Globals.DropManager.OnDropSuccess += Validate;
    }

    public override void OnBeat()
    {
        if(!Globals.DropManager.CanYouLetMeMove)
            return;

        if (StateMachine.CharacterTypeData.Evilness == Evilness.EVIL)
        {
            StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.DANCING);
            return;
        }
        
        switch (StateMachine.Satisafaction.CurrentState)
        {
            case CharacterAIStatisfaction.SATISFACTION_STATE.BORED:
                StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.IDLE);
                break;
            case CharacterAIStatisfaction.SATISFACTION_STATE.DANCING:
                StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.DANCING);
                break;
            case CharacterAIStatisfaction.SATISFACTION_STATE.LOYAL:
                StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.DANCING);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        if (!StateMachine.CurrentSlot.IsEnlighted && StateMachine.CharacterTypeData.Evilness == Evilness.GOOD && !StateMachine.CharacterDataObject.isTutorialNpc)
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

    public void Validate()
    {
        if (StateMachine.Satisafaction.CurrentState == CharacterAIStatisfaction.SATISFACTION_STATE.LOYAL)
        {
            StateMachine.CurrentSlot.SlotRenderer.ChangeColor(false);
            StateMachine.MoveToLocation = StateMachine.AreaManager.DanceFloorSideArea.GetOffDanceFloorPostion(StateMachine.transform.position);
            StateMachine.CurrentSlot.Occupant = null;
            StateMachine.NextState = StateMachine.DancingOffFloor;
            StateMachine.ChangeState(StateMachine.MoveToState);
        }
    }

    private void RunOutOfSatisfaction()
    {
        StateMachine.ChangeState(StateMachine.DieState);
    }

    public void OnQTECorrectInput(int nbTotalInputs)
    {
        if (StateMachine.CurrentSlot.IsEnlighted && StateMachine.TypeData.Evilness == Evilness.GOOD)
        {
            StateMachine.Satisafaction.IncreaseSatisfaction(StateMachine.CharacterDataObject.maxSatisafactionDJ * 1 / nbTotalInputs);
        }
    }

    public override void ExitState()
    {
        StateMachine.Satisafaction.OnSatsifactionZero -= RunOutOfSatisfaction;
        Globals.PriestCalculator.OnPriestNearToExorcize -= StartExorcize;
        Globals.DropManager.OnDropSuccess -= Validate;
    }
}
