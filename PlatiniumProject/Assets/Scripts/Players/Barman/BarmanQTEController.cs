using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BarmanQTEController : MonoBehaviour, IListenerBarmanActions
{
    private BarmanMovement _barmanMovement;
    
    #region Events
    public event Action<Sprite[], bool[]> OnBarmanQTEStarted;
    public event Action<Sprite[], bool[]> OnBarmanQTEEnded;
    public event Action<Sprite[], bool[]> OnBarmanQTEChanged;
    public event Action<Sprite[], bool[]> OnStartQteAllInputs;
    public event Action<Sprite[], bool[]> OnStopQteAllInputs;

    public UnityEvent OnStartQteAll;
    public UnityEvent OnStopQteAll;
    public UnityEvent OnStartWrongQte;
    public UnityEvent OnStopWrongQte;
    #endregion

    public float DurationValue => _barmanMovement == null? 0:_barmanMovement.CurrentDuration;
    private Coroutine _coroutineUpdateValue;
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

    public void ModifyQTE(Sprite[] sprites, bool[] colors)
    {
        OnBarmanQTEChanged?.Invoke(sprites,colors);
    }

    public void EndQTE(Sprite[] sprites, bool[] colors)
    {
        OnBarmanQTEEnded?.Invoke(sprites, colors);
        if (_coroutineUpdateValue != null)
        {
            StopCoroutine(_coroutineUpdateValue);
            _coroutineUpdateValue = null;
        }
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

    public void CallOnBarmanStartWrongSequence()
    {
        Debug.Log("CALL");
        OnStartWrongQte?.Invoke();
    }

    public void CallOnBarmanEndWrongSequence()
    {
        OnStopWrongQte?.Invoke();
    }
}
