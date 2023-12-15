using System;
using UnityEngine;
using UnityEngine.Events;

public class BarmanQTEController : MonoBehaviour, IListenerBarmanActions
{
    private BarmanMovement _barmanMovement;
    
    #region Events
    public event Action<Sprite[], bool[]> OnBarmanQTEStarted;
    public event Action<Sprite[], bool[]> OnBarmanQTEEnded;
    public event Action<Sprite[], bool[],float> OnBarmanQTEChanged;

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

    public void StartQTE(Sprite[] sprites, bool[] colors)
    {
        OnBarmanQTEStarted?.Invoke(sprites,colors);
    }

    public void ModifyQTE(Sprite[] sprites, bool[] colors, float value)
    {
        OnBarmanQTEChanged?.Invoke(sprites,colors,value);
    }

    public void EndQTE(Sprite[] sprites, bool[] colors)
    {
        OnBarmanQTEEnded?.Invoke(sprites, colors);
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
