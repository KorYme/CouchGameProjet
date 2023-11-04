using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputStatus
{
    PRESS,
    HOLD
}

public enum InputsSequence
{
    SEQUENCE,
    SIMULTANEOUS
}

public enum Difficulty
{
    NORMAL,
    SPECIAL
}

[Serializable]
public class QTESequence : ScriptableObject
{
    public int Index;
    public PlayerRole PlayerRole;
    public Difficulty Difficulty;
    public InputsSequence SequenceType;
    public CharacterColor ClientType;
    public Evilness Evilness;
    public int QTELevel = 1;
    public InputStatus Status = InputStatus.PRESS;
    public int DurationHold = 1;
    public List<UnitInput> ListSubHandlers = new List<UnitInput>();
}