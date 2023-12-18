using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEHandlerEvents
{
    //List of listener on QTEHandler
    List<IQTEable> _QTEables = new List<IQTEable>();
    List<IMissedInputListener> _missedInputListeners = new List<IMissedInputListener>();
    List<IListenerBarmanActions> _barmanListeners = new List<IListenerBarmanActions>();

    public void RegisterQTEable(IQTEable QTEable)
    {
        _QTEables.Add(QTEable);
    }
    public void UnregisterQTEable(IQTEable QTEable)
    {
        _QTEables.Remove(QTEable);
    }
    public void RegisterMissedInputListener(IMissedInputListener listener)
    {
        _missedInputListeners.Add(listener);
    }
    public void UnregisterMissedInputListener(IMissedInputListener listener)
    {
        _missedInputListeners.Remove(listener);
    }
    public void RegisterBarmanListener(IListenerBarmanActions listener)
    {
        _barmanListeners.Add(listener);
    }
    public void UnregisterBarmanListener(IListenerBarmanActions listener)
    {
        _barmanListeners.Remove(listener);
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
        foreach (IMissedInputListener reciever in _missedInputListeners)
        {
            reciever.OnQTEMissedInput();
        }
        foreach (IQTEable reciever in _QTEables)
        {
            reciever.OnQTEMissedInput();
        }
    }

    public void CallOnBarmanStartCorrectSequence()
    {
        foreach (IListenerBarmanActions reciever in _barmanListeners)
        {
            reciever.CallOnBarmanStartCorrectSequence();
        }
    }

    public void CallOnBarmanEndCorrectSequence()
    {
        foreach (IListenerBarmanActions reciever in _barmanListeners)
        {
            reciever.CallOnBarmanEndCorrectSequence();
        }
    }

    public void CallOnBarmanStartWrongSequence()
    {
        foreach (IListenerBarmanActions reciever in _barmanListeners)
        {
            reciever.CallOnBarmanStartWrongSequence();
        }
    }

    public void CallOnBarmanEndWrongSequence()
    {
        foreach (IListenerBarmanActions reciever in _barmanListeners)
        {
            reciever.CallOnBarmanEndWrongSequence();
        }
    }
}
