using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEHandlerEvents
{
    //List of listener on QTEHandler
    List<IQTEable> _QTEables = new List<IQTEable>();

    public void RegisterQTEable(IQTEable QTEable)
    {
        _QTEables.Add(QTEable);
    }
    public void UnregisterQTEable(IQTEable QTEable)
    {
        _QTEables.Remove(QTEable);
    }

    public void CallOnCorrectInput()
    {
        //Debug.LogWarning("CORRECT INPUT");
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTECorrectInput();
        }
    }
    public void CallOnWrongInput()
    {
        //Debug.LogWarning("WRONG INPUT");
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEWrongInput();
        }
    }
    public void CallOnQTEComplete()
    {
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEComplete();
        }
    }

    public void CallOnQTEStarted()
    {
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEStarted();
        }
    }

    public void CallOnMissedInput()
    {
        /*foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEMissedInput();
        }*/
    }
}
