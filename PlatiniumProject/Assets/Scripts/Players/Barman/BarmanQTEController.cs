using System;
using UnityEngine;
using UnityEngine.Events;

public class BarmanQTEController : MonoBehaviour
{
    #region Events
    public event Action<string> OnBarmanQTEStarted;
    public event Action<string> OnBarmanQTEEnded;
    public event Action<string> OnBarmanQTEChanged;

    public UnityEvent onDrinkComplete;
    #endregion

    public void StartQTE(string qteString)
    {
        OnBarmanQTEStarted?.Invoke(qteString);
    }

    public void ModifyQTE(string qteString)
    {
        OnBarmanQTEChanged?.Invoke(qteString);
    }

    public void EndQTE(string qteString)
    {
        OnBarmanQTEEnded?.Invoke(qteString);
    }
}
