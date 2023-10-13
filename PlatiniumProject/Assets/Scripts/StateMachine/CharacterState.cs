using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState
{
    public CharacterStateMachine StateMachine {get; private set; }

    public virtual void InitState(CharacterStateMachine sm)
    {
        StateMachine = sm;
    }
    public virtual void EnterState(){}
    public virtual void UpdateState(){}
    public virtual void ExitState(){}
    public virtual void OnBeat(){}
}
