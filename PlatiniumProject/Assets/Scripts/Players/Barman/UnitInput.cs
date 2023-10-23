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
    //Rotation
    public bool checkRotation = false;
    public float angleRotation = 90f;
    public float angleRotationAcceptance = 10f;
}
