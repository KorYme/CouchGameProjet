using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAiPuller : MonoBehaviour
{
    private CharacterAnimation _anim;
    private CharacterAIMovement _movement;
    private CharacterStateMachine _stateMachine;

    public CharacterStateMachine StateMachine => _stateMachine;
    
    public Vector3 PullPos { get; set; }

    private void Awake()
    {
        _anim = GetComponent<CharacterAnimation>();
        _movement = GetComponent<CharacterAIMovement>();
        _stateMachine = GetComponent<CharacterStateMachine>();
    }

    public void PullCharacter(CharacterObject chara ,CharacterState startState = null)
    {
        _anim.CharacterAnimationObject = chara.animation;
        _movement.MovementData = chara.movement;
        _stateMachine.CharacterDataObject = chara.data;
        _stateMachine.TypeData = chara.type;
        
        _stateMachine.PullCharacter(startState);
    }
}
