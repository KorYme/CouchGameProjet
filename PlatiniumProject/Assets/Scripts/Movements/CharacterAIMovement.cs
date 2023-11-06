using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIMovement : EntityMovement
{
    public Action OnMove;
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

    private void AnimationSetter()
    {
        _stateMachine.SpriteRenderer.sprite =
            _stateMachine.Animation.GetAnimationSprite(CharacterAnimation.ANIMATION_TYPE.MOVING);
    }

    public void MoveTo(Vector3 position)
    {
        MoveToPositionWithAnim(position, OnMove,
            _stateMachine.Animation.CharacterAnimationObject.walkAnimation.AnimationLenght);
    }
}
