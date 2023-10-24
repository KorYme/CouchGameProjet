using UnityEngine;

public class CharacterStateDancing : CharacterState
{
    public override void OnBeat()
    {
        StateMachine.GetComponentInChildren<SpriteRenderer>().color = Random.ColorHSV();
    }
}
