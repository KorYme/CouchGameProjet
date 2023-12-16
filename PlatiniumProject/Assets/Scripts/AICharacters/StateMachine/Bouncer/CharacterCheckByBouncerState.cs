using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterCheckByBouncerState : CharacterState
{
        public override void OnBeat()
        {
                StateMachine.CharacterAnimation.SetAnim(ANIMATION_TYPE.IDLE);
        }

        public override void EnterState()
        {
                base.EnterState();
                StateMachine.transform.position +=
                        new Vector3(-StateMachine.AreaManager.BouncerBoard.HorizontalSpacing / 2f, 0, 0);
                //StateMachine.CharacterMove.MoveToPosition(StateMachine.transform.position + new Vector3(-StateMachine.AreaManager.BouncerBoard.HorizontalSpacing / 2f,0,0));
        }

        public void BouncerAction(bool canEnter)
        {
                StateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY);
                StateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY2);
                if (canEnter)
                {
                        StateMachine.CurrentSlot.Occupant = null;
                        StateMachine.ChangeState(StateMachine.RoamState);
                        StateMachine.NextState = StateMachine.BarManQueueState;
                }
                else
                {
                        StateMachine.CurrentSlot.Occupant = null;
                        StateMachine.ChangeState(StateMachine.DieState);
                }
        }
}
