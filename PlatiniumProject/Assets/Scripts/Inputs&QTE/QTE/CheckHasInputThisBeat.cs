using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckHasInputThisBeat
{
    public bool HadInputThisBeat { get; private set; } = false;
    ITimingable _timingable;

    public CheckHasInputThisBeat(ITimingable timingable)
    {
        _timingable = timingable;
    }

    public void ChangeHadInputThisBeat(){
        HadInputThisBeat = true;
    }
    public void ResetInputThisBeat()
    {
        if (HadInputThisBeat)
        {
            //Debug.Log("ACTIVATE BEAT");
        }
        HadInputThisBeat = false;
    }
}
