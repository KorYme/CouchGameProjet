using System;
using TMPro;
using UnityEngine;

public class BouncerQTEController : MonoBehaviour, IQTEable
{
    QTEHandler _qteHandler;

    #region Events
    public event Action<string> OnBouncerQTEStarted;
    public event Action<string> OnBouncerQTEEnded; //Arg1 peut être enlevé
    public event Action<string> OnBouncerQTEChanged;
    #endregion
    void Start()
    {
        TryGetComponent(out _qteHandler);
        if (_qteHandler != null)
        {
            _qteHandler.RegisterQTEable(this);
        }
    }
    public void StartQTE(CharacterTypeData typeData)
    {
        _qteHandler.StartNewQTE(typeData);
    }

    public void OnQTEComplete()
    {
        OnBouncerQTEEnded?.Invoke(_qteHandler.GetCurrentInputString());
    }

    public void OnQTECorrectInput()
    {
        OnBouncerQTEChanged?.Invoke(_qteHandler.GetCurrentInputString());
    }

    public void OnQTEStarted()
    {
        OnBouncerQTEStarted?.Invoke(_qteHandler.GetCurrentInputString());
    }

    public void OnQTEWrongInput()
    {
        _qteHandler.DeleteCurrentCoroutine();
        OnBouncerQTEEnded?.Invoke(_qteHandler.GetCurrentInputString());
    }
}
