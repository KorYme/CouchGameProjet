using UnityEngine;

public class CharacterStateDancing : CharacterState
{
    public override void OnBeat()
    {
        StateMachine.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
    }
}
