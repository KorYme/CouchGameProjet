using UnityEngine;

public class CharacterStateDancing : CharacterState
{
    int _currentSatisfaction;
    
    public override void EnterState()
    {
        base.EnterState();
        StateMachine.Satisafaction.InitializeStatisfaction(StateMachine.CharacterDataObject.maxSatisafactionDJ);
        StateMachine.Satisafaction.OnSatsifactionZero += RunOutOfSatisfaction;
    }

    public override void OnBeat()
    {
        //StateMachine.SpriteRenderer.color = Random.ColorHSV();
        StateMachine.SpriteRenderer.sprite = StateMachine.Animation.GetAnimationSprite(CharacterAnimation.ANIMATION_TYPE.DANCING);
        if (!StateMachine.CurrentSlot.IsEnlighted)
        {
            StateMachine.Satisafaction.DecreaseSatisfaction(StateMachine.CharacterDataObject.decrementationValueOnFloor);
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
    }
}
