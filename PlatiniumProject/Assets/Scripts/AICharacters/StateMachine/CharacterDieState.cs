using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterDieState : CharacterState
{
    private Color _baseColor;
    public override void EnterState()
    {
        base.EnterState();
        StateMachine.OnCharacterDeath?.Invoke();
        if (StateMachine.CurrentSlot != null)
        {
            StateMachine.CurrentSlot.Occupant = null;
        }
        
        _baseColor = StateMachine.CharacterAnimation.Sp.material.GetColor("_Color");

        // if (StateMachine.TypeData.Evilness == Evilness.GOOD)
        // {
        //     StateMachine.CharacterMove.MoveTo(Globals.ExitPoints.FindClosestExitPoint(StateMachine.transform.position));
        // }
        StateMachine.StartCoroutine(ExitRoutine());
    }

    IEnumerator ExitRoutine()
    {
        if (StateMachine.TypeData.Evilness == Evilness.GOOD)
        {
            float timer = 0;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                StateMachine.CharacterAnimation.Sp.material.SetFloat("_Fade", Mathf.Lerp(1,0,timer/ 1f));
                StateMachine.CharacterAnimation.Sp.material.SetColor("_Color", Color.red);

                yield return new WaitUntil(() => Globals.BeatManager?.IsPlaying ?? true);
            }
            yield return new WaitUntil(() => !StateMachine.CharacterMove.IsMoving);
        }
        else
        {
            StateMachine.CharacterAnimation.SetFullAnim(ANIMATION_TYPE.DIE, 1f);
            yield return new WaitUntil(() => !StateMachine.CharacterAnimation.IsAnimationPlaying);
        }
        StateMachine.GoBackInPull();
        StateMachine.CharacterAnimation.Sp.material.SetFloat("_Fade", 1);
        StateMachine.CharacterAnimation.Sp.material.SetColor("_Color", _baseColor);
    }
}
