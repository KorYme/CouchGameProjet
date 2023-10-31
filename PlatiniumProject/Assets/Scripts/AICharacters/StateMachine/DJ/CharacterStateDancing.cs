using UnityEngine;

public class CharacterStateDancing : CharacterState
{
    int _currentSatisfaction;
    
    public override void EnterState()
    {
        StateMachine.Satisafaction.InitializeStatisfaction(StateMachine.CharacterDataObject.maxSatisafactionDJ);
        StateMachine.Satisafaction.OnSatsifactionZero += RunOutOfSatisfaction;
    }

    public override void OnBeat()
    {
        StateMachine.SpriteRenderer.color = Random.ColorHSV();
        /*currentSatisfaction = StateMachine.CurrentSlot.IsEnlighted ?
            Mathf.Clamp(_currentSatisfaction + StateMachine.CharacterDataObject.incrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ) :
            Mathf.Clamp(_currentSatisfaction - StateMachine.CharacterDataObject.decrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ);*/
        if (!StateMachine.CurrentSlot.IsEnlighted)
        {
            StateMachine.Satisafaction.DecreaseSatisfaction(StateMachine.CharacterDataObject.decrementationValueOnFloor);
        }
    }

    private void RunOutOfSatisfaction()
    {
        StateMachine.ChangeState(StateMachine.DieState);
    }

    public void OnQTECorrectInput()
    {
        if (StateMachine.CurrentSlot.IsEnlighted)
        {
            StateMachine.Satisafaction.IncreaseSatisfaction(StateMachine.CharacterDataObject.incrementationValueOnFloor);
            //Debug.Log($"{_currentSatisfaction} {StateMachine.name}");
        }
    }

    public override void ExitState()
    {
        StateMachine.Satisafaction.OnSatsifactionZero -= RunOutOfSatisfaction;
    }
}
