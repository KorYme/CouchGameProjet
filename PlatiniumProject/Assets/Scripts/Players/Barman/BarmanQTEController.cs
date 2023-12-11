using System;
using UnityEngine;
using UnityEngine.Events;

public class BarmanQTEController : MonoBehaviour
{
    private BarmanMovement _barmanMovement;
    
    #region Events
    public event Action<string> OnBarmanQTEStarted;
    public event Action<string> OnBarmanQTEEnded;
    public event Action<string> OnBarmanQTEChanged;
    #endregion

    private void Awake()
    {
        _barmanMovement = GetComponent<BarmanMovement>();
    }

    public void StartQTE(string qteString)
    {
        OnBarmanQTEStarted?.Invoke(qteString);
        _barmanMovement.IsInQte = true;
    }

    public void ModifyQTE(string qteString)
    {
        OnBarmanQTEChanged?.Invoke(qteString);
    }

    public void EndQTE(string qteString)
    {
        OnBarmanQTEEnded?.Invoke(qteString);
        _barmanMovement.IsInQte = false;
    }
}
