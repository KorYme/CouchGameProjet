using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIMovement : EntityMovement
{
    private CharacterStateMachine _stateMachine;
    private void Awake()
    {
        _stateMachine = GetComponent<CharacterStateMachine>();
    }

    public void MoveTo(Vector3 pos)
    {
        MoveToPosition(pos);
    }

    private void AnimationSetter()
    {
        _stateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.MOVE);
    }


}
