using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateBarmanQueue : CharacterState
{
    public override void OnBeat()
    {
        base.OnBeat();
        StateMachine.Animation.SetAnim(ANIMATION_TYPE.IDLE);
    }
    public override void EnterState()
    {
        base.EnterState();
        if (StateMachine.WaitingLines.Length > 0) 
        {
            int indexLine = 0;
            int nbCharactersInLine = StateMachine.WaitingLines[0].NbCharactersWaiting;
        
            for (int i = 1; i < StateMachine.WaitingLines.Length; i++)
            {
                if (nbCharactersInLine > StateMachine.WaitingLines[i].NbCharactersWaiting) {
                    nbCharactersInLine = StateMachine.WaitingLines[i].NbCharactersWaiting;
                    indexLine = i;
                }
            }
            StateMachine.WaitingLines[indexLine].AddToWaitingLine(StateMachine);
            StateMachine.CurrentWaitingLine = StateMachine.WaitingLines[indexLine];
        }
    }
    
}
