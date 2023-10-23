using System;

[Serializable]
public class UnitInput : SODrinkInputs
{
    public int actionIndex; //Input action index
                            //Press
    public InputStatus status;
    //Hold
    public bool checkHold = false;
    public int nbBeatHoldDuration = 2;
}
