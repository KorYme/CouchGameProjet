using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterCheckByBouncerState : CharacterState
{
        public override void OnBeat()
        {
                //Bounce
        }

        public void BouncerAction(bool canEnter)
        {
                if (canEnter)
                {
                        //StateMachine.ChangeState(StateMachine.MoveToState());
                }
                else
                {
                        //StateMachine.ChangeState();
                }
        }
}
