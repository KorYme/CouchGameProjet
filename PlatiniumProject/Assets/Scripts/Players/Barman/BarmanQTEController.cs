using System;
using UnityEngine;
using UnityEngine.Events;

public class BarmanQTEController : MonoBehaviour, IListenerBarmanActions
{
    private BarmanMovement _barmanMovement;
    
    #region Events
    public event Action<string> OnBarmanQTEStarted;
    public event Action<string> OnBarmanQTEEnded;
    public event Action<string> OnBarmanQTEChanged;

    public UnityEvent OnStartQteAll;
    public UnityEvent OnStopQteAll;
    #endregion

    private WaitingLineBar[] _waitingLines;
    private void Awake()
    {
        _barmanMovement = GetComponent<BarmanMovement>();
        _waitingLines = FindObjectsOfType<WaitingLineBar>();
    }

    private void Start()
    {
        foreach (var v in _waitingLines)
        {
            v.GetComponent<QTEHandler>().RegisterListener(this);
        }
    }

    private void OnDestroy()
    {
        foreach (var v in _waitingLines)
        {
            v.GetComponent<QTEHandler>().UnregisterListener(this);
        }
    }

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

    public void CallOnBarmanStartCorrectSequence()
    {
        _barmanMovement.IsPlayingFullQte = true;
        OnStartQteAll?.Invoke();
    }

    public void CallOnBarmanEndCorrectSequence()
    {
        OnStopQteAll?.Invoke();
        _barmanMovement.IsPlayingFullQte = false;
    }
}
