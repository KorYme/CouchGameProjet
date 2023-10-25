using UnityEngine;

public class CharacterStateDancing : CharacterState
{
    int _currentSatisfaction;

    public override void EnterState()
    {
        _currentSatisfaction = StateMachine.CharacterDataObject.maxSatisafactionDJ;
    }

    public override void OnBeat()
    {
        StateMachine.SpriteRenderer.color = Random.ColorHSV();
        _currentSatisfaction = StateMachine.CurrentSlot.IsEnlighted ?
            Mathf.Clamp(_currentSatisfaction + StateMachine.CharacterDataObject.incrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ) :
            Mathf.Clamp(_currentSatisfaction - StateMachine.CharacterDataObject.decrementationValueOnFloor, 0, StateMachine.CharacterDataObject.maxSatisafactionDJ);
        if (_currentSatisfaction <= 0)
        {
            StateMachine.ChangeState(StateMachine.DieState);
        }
    }
}
