using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IListenerBarmanActions
{
    void CallOnBarmanStartCorrectSequence();

    void CallOnBarmanEndCorrectSequence();
    void CallOnBarmanStartWrongSequence();
    void CallOnBarmanEndWrongSequence();
}
