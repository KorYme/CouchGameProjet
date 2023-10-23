using System;
using System.Collections.Generic;

public enum InputStatus
{
    PRESS,
    HOLD,
    ROTATION
}

public enum InputsSequence
{
    SEQUENCE,
    SIMULTANEOUS
}

[Serializable]
public class QTESequence: SODrinkInputs
{
    public List<SODrinkInputs> _listSubHandlers = new List<SODrinkInputs>();
    public InputsSequence _sequenceType;
}