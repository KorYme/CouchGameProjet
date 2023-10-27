using UnityEngine;

public class CharacterStateDancing : CharacterState, IQTEable
{
    int _currentSatisfaction;

    public override void EnterState()
    {
        _currentSatisfaction = StateMachine.CharacterDataObject.maxSatisafactionDJ;
    }

    public override void OnBeat()
    {
        StateMachine.SpriteRenderer.color = Random.ColorHSV();
        /*currentSatisfaction = StateMachine.CurrentSlot.IsEnlighted ?
            Mathf.Clamp(_currentSatisfaction + StateMachine.CharacterDataObject.incrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ) :
            Mathf.Clamp(_currentSatisfaction - StateMachine.CharacterDataObject.decrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ);*/
        _currentSatisfaction = Mathf.Clamp(_currentSatisfaction - StateMachine.CharacterDataObject.decrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ);
        
        if (_currentSatisfaction <= 0)
        {
            StateMachine.ChangeState(StateMachine.DieState);
        }
    }

    public void OnQTEComplete()
    {
    }

    public void OnQTECorrectInput()
    {
        if (StateMachine.CurrentSlot.IsEnlighted)
        {
            _currentSatisfaction = Mathf.Clamp(_currentSatisfaction + StateMachine.CharacterDataObject.incrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ);
            Debug.Log($"{_currentSatisfaction} {StateMachine.name}");
        }
    }

    public void OnQTEStarted(QTESequence sequence)
    {
    }
}
