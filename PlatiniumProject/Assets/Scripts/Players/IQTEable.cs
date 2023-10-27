using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQTEable
{
    void OnQTEStarted(QTESequence sequence);
    void OnQTEComplete();
    void OnQTECorrectInput();
}
