using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PriestCalculator : MonoBehaviour
{

    public enum EXORCIZE_STATE
    {
        NORMAL,
        EXORCIZING,
        EXORCIZED
    }
    
    [Header("Values")]
    [SerializeField] private int _priestAmountToStartExorcize;
    [SerializeField] private int _priestAmountToExorcize;
    
    [Header("References")]
    [SerializeField] private CheckerBoard _danceFloor;
    public List<CharacterStateMachine> CurrentPriestList;
    
    public EXORCIZE_STATE ExorcizeState { get; private set; } = EXORCIZE_STATE.NORMAL;
    
    public Action OnPriestNearToExorcize;
    public Action OnPriestExorcize;

    public UnityEvent OnLoose;

    private void Awake()
    {
        Globals.PriestCalculator ??= this;
    }

    private void Start()
    {
        Globals.DropManager.OnDropSuccess += DropSucced;
    }

    private void OnDisable()
    {
        Globals.DropManager.OnDropSuccess -= DropSucced;
    }

    public void DropSucced()
    {
        CleanPriests();
        ExorcizeState = EXORCIZE_STATE.NORMAL;
    }
    public void CleanPriests()
    {
        foreach (CharacterStateMachine m in CurrentPriestList)
        {
            m.ChangeState(m.DieState);
        }
        CurrentPriestList.Clear();
    }

    public void PriestOnDanceFloor(CharacterStateMachine chara)
    {
        CurrentPriestList.Add(chara);
        if (CurrentPriestList.Count == _priestAmountToStartExorcize)
        {
            Debug.Log("Start exorcisme");
            ExorcizeState = EXORCIZE_STATE.EXORCIZING;
            OnPriestNearToExorcize?.Invoke();
        }
        else if (CurrentPriestList.Count == _priestAmountToExorcize)
        {
            Debug.Log("GAME OVER");
            ExorcizeState = EXORCIZE_STATE.EXORCIZED;
            OnPriestExorcize?.Invoke();
            OnLoose?.Invoke();
        }
    }
}
