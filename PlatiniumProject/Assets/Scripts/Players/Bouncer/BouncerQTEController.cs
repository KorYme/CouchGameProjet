using System;
using TMPro;
using UnityEngine;

public class BouncerQTEController : MonoBehaviour, IQTEable
{
    QTEHandler _qteHandler;
    private CharacterAnimation _characterAnimation;
    private BouncerMovement _bouncerMovement;

    #region Events
    public event Action<string> OnBouncerQTEStarted;
    public event Action<string> OnBouncerQTEEnded; //Arg1 peut être enlevé
    public event Action<string> OnBouncerQTEChanged;
    #endregion

    private void Awake()
    {
        _characterAnimation = GetComponent<CharacterAnimation>();
        _bouncerMovement = GetComponent<BouncerMovement>();
    }

    void Start()
    {
        TryGetComponent(out _qteHandler);
        if (_qteHandler != null)
        {
            _qteHandler.RegisterListener(this);
        }
    }
    public void StartQTE(CharacterTypeData typeData)
    {
        _qteHandler.StartNewQTE(typeData);
    }

    public void OpenBubble()
    {
        OnBouncerQTEStarted?.Invoke("<color=green>A</color> : Accept\n<color=red>B</color> : Refuse");
    }
    public void CloseBubble()
    {
        OnBouncerQTEEnded?.Invoke("");
    }
    public void OnQTEComplete()
    {
        OnBouncerQTEEnded?.Invoke(_qteHandler.GetCurrentInputString());
    }

    public void OnQTECorrectInput()
    {
        OnBouncerQTEChanged?.Invoke(_qteHandler.GetCurrentInputString());
        _characterAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.FIGHT, false);
        
    }

    public void OnQTEStarted()
    {
        //OnBouncerQTEStarted?.Invoke(_qteHandler.GetCurrentInputString());
        OnBouncerQTEChanged?.Invoke(_qteHandler.GetCurrentInputString());
    }

    public void OnQTEWrongInput()
    {
        if (!_bouncerMovement.CurrentClient.StateMachine.CharacterDataObject.isTutorialNpc)
        {
            _qteHandler.DeleteCurrentCoroutine();
            OnBouncerQTEEnded?.Invoke(_qteHandler.GetCurrentInputString());
        }
        _characterAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.WRONG_INPUT, false);
    }
}
