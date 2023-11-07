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
        _timingable?.OnBeatStartEvent.AddListener(ResetInputThisBeat);
    }

    public void ChangeHadInputThisBeat(){
        HadInputThisBeat = true;
    }
    void ResetInputThisBeat()
    {
        HadInputThisBeat = false;
    }

    ~CheckHasInputThisBeat()
    {
        _timingable?.OnBeatStartEvent.RemoveListener(ResetInputThisBeat);
    }
}
