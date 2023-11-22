using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputStatus
{
    SHORT,
    LONG,
}
public enum LongInputType
{
    HOLD,
    SHAKE
}

public enum InputsSequence
{
    SEQUENCE,
    SIMULTANEOUS
}

[Serializable]
public class QTESequence : ScriptableObject
{
    public int Index;
    public PlayerRole PlayerRole;
    public InputsSequence SequenceType;
    public CharacterColor ClientType;
    public Evilness Evilness;
    public int QTELevel = 1;
    public InputStatus Status = InputStatus.SHORT;
    public LongInputType LongInputType = LongInputType.HOLD;
    public int DurationHold = 1;
    public List<UnitInput> ListSubHandlers = new List<UnitInput>();

}