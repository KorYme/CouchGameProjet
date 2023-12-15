using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static BouncerMovement;
using static UnityEngine.Rendering.DebugUI;

public class BouncerQTEController : MonoBehaviour, IQTEable
{
    QTEHandler _qteHandler;
    private CharacterAnimation _characterAnimation;
    private BouncerMovement _bouncerMovement;
    
    [FormerlySerializedAs("_onPunch")] [SerializeField] UnityEvent OnPunch;
    #region Events
    public event Action OnBouncerCheckingStarted;
    public event Action<Sprite[]> OnBouncerQTEStarted;
    public event Action<Sprite[]> OnBouncerQTEEnded;
    public event Action<Sprite[]> OnBouncerQTEChanged;
    [SerializeField] UnityEvent _onSucces;
    [SerializeField] UnityEvent _onFail;
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
        //OnBouncerQTEStarted?.Invoke(null);
    }

    public void OpenBubble()
    {
        OnBouncerCheckingStarted?.Invoke();
    }
    public void CloseBubble()
    {
        OnBouncerQTEEnded?.Invoke(null);
    }
    public void OnQTEComplete()
    {
        OnBouncerQTEEnded?.Invoke(null);
    }

    public void OnQTECorrectInput()
    {
        OnBouncerQTEChanged?.Invoke(_qteHandler.GetQTESprites());
        _characterAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.FIGHT, false);
        _onSucces?.Invoke();
        OnPunch?.Invoke();
        
    }

    public void OnQTEStarted()
    {
        //OnBouncerQTEStarted?.Invoke(_qteHandler.GetCurrentInputString());
        OnBouncerQTEStarted?.Invoke(_qteHandler.GetQTESprites());
    }

    public void OnQTEWrongInput()
    {
        if (!_bouncerMovement.CurrentClient.StateMachine.CharacterDataObject.isTutorialNpc)
        {
            _qteHandler.DeleteCurrentCoroutine();
            OnBouncerQTEEnded?.Invoke(_qteHandler.GetQTESprites());
            _onFail?.Invoke();
        }
        _characterAnimation.SetLatency(2);
        _characterAnimation.SetAnim(ANIMATION_TYPE.WRONG_INPUT, false);
    }
    public void OnQTEMissedInput()
    {

    }

    public void OnBeginDrop()
    {
        CloseBubble();
        _qteHandler.PauseQTE(true);
    }

    public void OnDropEnd(CHECKING_STATE checkingState)
    {
        _qteHandler.PauseQTE(false);
        //INDICATION DE SI ON EST EN REFUSE/CHECKING
        if (checkingState == CHECKING_STATE.CHECKING)
        {
            OnBouncerCheckingStarted?.Invoke();
            OnBouncerQTEStarted?.Invoke(null);
        }
        else if (checkingState == CHECKING_STATE.QTE)
        {
            OnBouncerQTEStarted?.Invoke(_qteHandler.GetQTESprites());
        }
    }
}
