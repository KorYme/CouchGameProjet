using Rewired;
using System;
using UnityEngine;

[Serializable]
public class UnitInput : ScriptableObject
{
    public int Index;
    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int ActionIndex = 0; //Input action index
    public bool IsInputPositive = true;
    //Press
    public InputStatus Status = InputStatus.PRESS;
    //Hold
    public bool CheckHold = false;
    public int NbBeatHoldDuration = 2;
}
