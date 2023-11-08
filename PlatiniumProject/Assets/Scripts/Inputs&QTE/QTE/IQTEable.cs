using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQTEable
{
    void OnQTEStarted();
    void OnQTEComplete();
    void OnQTECorrectInput();
    void OnQTEWrongInput();
}
