using Rewired;
using System;
using UnityEngine;

[Serializable]
public class UnitInput : ScriptableObject
{
    public int Index;
    [ActionIdProperty(typeof(RewiredConsts.Action))]
    public int ActionIndex = 0; //Input action index
    public bool UseRotation = false;
    public int NbTurns = 1;
    public LongInputType LongInputType = LongInputType.HOLD;
}
