using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void InitializeStatisfaction(int maxSatisfction, int maxLoyalSatisfaction)
    {
        if (maxSatisfction <= 0)
        {
            Debug.LogError("Statisfaction must be positive ");
            return;
        }
        _characterTypeData = _stateMachine.TypeData;
        CurrentMaxStatisafction = maxSatisfction;
        CurrentStatisafction = CurrentMaxStatisafction;
        CurrentSatisactionToGetLoyal = maxLoyalSatisfaction;
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
        if (CurrentStatisafction > CurrentMaxStatisafction && CurrentStatisafction < CurrentSatisactionToGetLoyal &&
            _currentState != SATISFACTION_STATE.DANCING)
        {
            _currentState = SATISFACTION_STATE.DANCING;
        }
        else if (CurrentStatisafction >= CurrentSatisactionToGetLoyal && _currentState != SATISFACTION_STATE.LOYAL)
        {
            _currentState = SATISFACTION_STATE.LOYAL;
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
        Debug.Log(CurrentStatisafction);
        CurrentStatisafction -= amount;
        
        if (CurrentStatisafction <= 0)
        {
            OnSatsifactionZero?.Invoke();
        }
        else if(CurrentStatisafction < CurrentMaxStatisafction && _currentState != SATISFACTION_STATE.BORED)
        {
            _currentState = SATISFACTION_STATE.BORED;
        }
    }
    
    
}
