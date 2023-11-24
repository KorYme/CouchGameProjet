using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIStatisfaction : MonoBehaviour
{
    private CharacterData _characerData;
    private CharacterTypeData _characterTypeData;
    private CharacterStateMachine _stateMachine;
    public Action OnSatsifactionZero;
    
    public int CurrentMaxStatisafction { get; private set; }
    public int CurrentStatisafction { get; private set; }

    private void Awake()
    {
        _stateMachine = GetComponent<CharacterStateMachine>();
        _characerData = _stateMachine.CharacterDataObject;
    }

    public void InitializeStatisfaction(int maxSatisfction)
    {
        if (maxSatisfction <= 0)
        {
            Debug.LogError("Statisfaction must be positive ");
            return;
        }
        _characterTypeData = _stateMachine.TypeData;
        CurrentMaxStatisafction = maxSatisfction;
        CurrentStatisafction = CurrentMaxStatisafction;
    }

    public void IncreaseSatisfaction(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("value must be positive");
            return;
        }

        CurrentStatisafction = Mathf.Min(CurrentMaxStatisafction, CurrentStatisafction + amount);
    }
    
    public void DecreaseSatisfaction(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("value must be positive");
            return;
        }

        CurrentStatisafction -= amount;
        
        if (CurrentStatisafction <= 0)
        {
            OnSatsifactionZero?.Invoke();
        }
    }
    
    
}
