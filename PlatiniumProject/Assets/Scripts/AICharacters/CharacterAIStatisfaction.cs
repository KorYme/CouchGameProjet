using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterAIStatisfaction : MonoBehaviour
{
    public enum SATISFACTION_STATE
    {
        BORED,
        DANCING,
        LOYAL
    }
    
    private CharacterData _characterData;
    private CharacterTypeData _characterTypeData;
    private CharacterStateMachine _stateMachine;
    public SATISFACTION_STATE _currentState;
    public Action OnSatsifactionZero;

    public SATISFACTION_STATE CurrentState => _currentState;
    public int CurrentMaxStatisafction { get; private set; }
    public int CurrentStatisafction { get; private set; }
    public int CurrentSatisactionToGetLoyal { get; private set; }

    private void Awake()
    {
        _stateMachine = GetComponent<CharacterStateMachine>();
        _characterData = _stateMachine.CharacterDataObject;
    }

    public void InitializeStatisfaction(int maxSatisfction, int maxLoyalSatisfaction, bool randomiseSatisfaction = false)
    {
        if (maxSatisfction <= 0)
        {
            Debug.LogError("Statisfaction must be positive ");
            return;
        }
        _characterTypeData = _stateMachine.TypeData;
        CurrentMaxStatisafction = maxSatisfction;
        CurrentStatisafction = !randomiseSatisfaction? CurrentMaxStatisafction : Random.Range(CurrentMaxStatisafction / 2, CurrentMaxStatisafction + 1);
        CurrentSatisactionToGetLoyal = maxLoyalSatisfaction;
        
        _stateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY);
        _stateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY2);
    }

    public void IncreaseSatisfaction(int amount)
    {
        if(_currentState == SATISFACTION_STATE.LOYAL)
            return;
        
        if (amount <= 0)
        {
            Debug.LogError("value must be positive");
            return;
        }

        CurrentStatisafction += amount;
        if (CurrentStatisafction > CurrentMaxStatisafction * (1f/3f) && CurrentStatisafction < CurrentMaxStatisafction)
        {
            _stateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY2);
        }
        if (CurrentStatisafction > CurrentMaxStatisafction && CurrentStatisafction < CurrentSatisactionToGetLoyal &&
            _currentState != SATISFACTION_STATE.DANCING)
        {
            _currentState = SATISFACTION_STATE.DANCING;
            _stateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY);
            _stateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY2);
        }
        else if (CurrentStatisafction >= CurrentSatisactionToGetLoyal && _currentState != SATISFACTION_STATE.LOYAL)
        {
            _currentState = SATISFACTION_STATE.LOYAL;
            _stateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.SATISAFCTION);
            _stateMachine.CurrentSlot.SlotRenderer.ChangeColor(true);
            Globals.WinMenu.SacrifiedClient++;
            if (_stateMachine.CharacterDataObject.isTutorialNpc)
            {
                Globals.TutorialManager.HandledTutoCharacter++;
            }
        }
    }
    
    public void DecreaseSatisfaction(int amount)
    {
        if(_currentState == SATISFACTION_STATE.LOYAL)
            return;
        
        if (amount <= 0)
        {
            Debug.LogError("value must be positive");
            return;
        }
        CurrentStatisafction -= amount;
        
        if (CurrentStatisafction <= 0)
        {
            OnSatsifactionZero?.Invoke();
            _stateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY);
            _stateMachine.CharacterAnimation.VfxHandeler.StopVfx(VfxHandeler.VFX_TYPE.ANGRY2);
        }
        else if (CurrentStatisafction <= CurrentMaxStatisafction * (1f/3f))
        {
            _stateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ANGRY2);
        }
        else if (CurrentStatisafction <= CurrentMaxStatisafction * (2f/3f))
        {
            _stateMachine.CharacterAnimation.VfxHandeler.PlayVfx(VfxHandeler.VFX_TYPE.ANGRY);
        }
        
        if(CurrentStatisafction < CurrentMaxStatisafction && _currentState != SATISFACTION_STATE.BORED)
        {
            _currentState = SATISFACTION_STATE.BORED;
        }
    }
    
    
}
