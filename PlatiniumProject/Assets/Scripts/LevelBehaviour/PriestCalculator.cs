using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriestCalculator : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private int _priestAmountToStartExorcize;
    [SerializeField] private int _priestAmountToExorcize;
    
    [Header("References")]
    [SerializeField] private CheckerBoard _danceFloor;
    public List<CharacterStateMachine> CurrentPriestList;
    
    public Action OnPriestNearToExorcize;
    public Action OnPriestExorcize;

    private void Start()
    {
        //Globals.DropController
    }

    public void CleanPriests()
    {
        foreach (var p in CurrentPriestList)
        {
            p.ChangeState(p.DieState);
        }
        CurrentPriestList.Clear();
    }

    public void PriestOnDanceFloor(CharacterStateMachine chara)
    {
        CurrentPriestList.Add(chara);
        if (CurrentPriestList.Count == _priestAmountToStartExorcize)
        {
            Debug.Log("Start exorcisme");
            OnPriestNearToExorcize?.Invoke();
        }
        else if (CurrentPriestList.Count == _priestAmountToExorcize)
        {
            Debug.Log("GAME OVER");
            OnPriestExorcize?.Invoke();
        }
    }
}
