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
        OnMove += AnimationSetter;
    }

    private void OnDisable()
    {
        OnMove -= AnimationSetter;
    }

    public void MoveTo(Vector3 pos)
    {
        MoveToPosition(pos, _stateMachine.Animation.CharacterAnimationObject.Animations[ANIMATION_TYPE.MOVE].AnimationLenght);
    }

    private void AnimationSetter()
    {
        _stateMachine.SpriteRenderer.sprite =
            _stateMachine.Animation.GetAnimationSprite(ANIMATION_TYPE.MOVE);
    }


}
