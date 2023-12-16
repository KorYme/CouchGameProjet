using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DropController : MonoBehaviour, IIsControllable
{
    [SerializeField] PlayerSyncEvents _syncEvents;
    [SerializeField] PlayerRole _playerRole;

    TriggerInputCheck _triggerInputCheckRT;
    TriggerInputCheck _triggerInputCheckLT;

    Coroutine _syncCoroutine;

    public UnityEvent OnHoldDrop;
    public UnityEvent OnDropDrop;

    public int TriggerPressed
    {
        get
        {
            if (_triggerInputCheckRT == null) return 0;
            return (_triggerInputCheckRT.TriggerState == TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_TIME ? 1 : 0) 
                + (_triggerInputCheckLT.TriggerState == TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_TIME ? 1 : 0);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitWhile(() => Players.PlayersController[(int)_playerRole] == null);
        Players.AddListenerPlayerController(this);
        _triggerInputCheckRT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].RT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckLT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].LT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckRT.OnTriggerPerformed += value => CheckTriggerState(_triggerInputCheckLT, value);
        _triggerInputCheckLT.OnTriggerPerformed += value => CheckTriggerState(_triggerInputCheckRT, value);
        Globals.DropManager.OnDropFail += () => OnDropDrop?.Invoke();
        Globals.DropManager.AllDropControllers.Add(this);
        _syncEvents[_playerRole].isNotSyncEvent?.Post(gameObject);
    }

    private void OnDestroy()
    {
        if (Players.PlayersController[(int)_playerRole] == null) return;
        Players.RemoveListenerPlayerController(this);
        _triggerInputCheckRT.OnTriggerPerformed -= value => CheckTriggerState(_triggerInputCheckLT, value);
        _triggerInputCheckLT.OnTriggerPerformed -= value => CheckTriggerState(_triggerInputCheckRT, value);
        Globals.DropManager?.AllDropControllers.Remove(this);
    }

    void CheckTriggerState(TriggerInputCheck trigger, bool value)
    {
        if(Globals.DropManager.DropState == DropManager.DROP_STATE.OUT_OF_DROP)
            return;
        
        Globals.DropManager.UpdateTriggerValue(value);
        if (_syncCoroutine == null)
        {
            _syncCoroutine = StartCoroutine(TriggerSynchronize());
        }
        if (value)
        {
            if (trigger.TriggerState == TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_TIME)
            {
                OnHoldDrop?.Invoke();
                _syncEvents[_playerRole].isSyncEvent?.Post(gameObject);
                StopCoroutine(_syncCoroutine);
                _syncCoroutine = null;
            }
        }
        else
        {
            _syncEvents[_playerRole].isNotSyncEvent?.Post(gameObject);
            OnDropDrop?.Invoke();
            if (_syncCoroutine == null)
            {
                trigger.TriggerState = TriggerInputCheck.TRIGGER_STATE.NEED_TO_BE_RELEASED;
            }
        }
    }

    IEnumerator TriggerSynchronize()
    {
        yield return new WaitForSeconds(Globals.DropManager.PressingSynchronizationTime);
        _triggerInputCheckLT.TriggerState = TriggerInputCheck.TRIGGER_STATE.NEED_TO_BE_RELEASED;
        _triggerInputCheckRT.TriggerState = TriggerInputCheck.TRIGGER_STATE.NEED_TO_BE_RELEASED;
        _syncEvents[_playerRole].isNotSyncEvent?.Post(gameObject);
        _syncCoroutine = null;
    }

    Color ChangeColor(TriggerInputCheck.TRIGGER_STATE state)
    {
        switch (state)
        {
            case TriggerInputCheck.TRIGGER_STATE.RELEASED:
                return Color.yellow;
            case TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_TIME:
                return Color.green;
            case TriggerInputCheck.TRIGGER_STATE.NEED_TO_BE_RELEASED:
                return Color.red;
            default:
                return Color.white;
        }
    }

    public void ChangeController()
    {
        if (Players.PlayersController[(int)_playerRole] != null) {
            _triggerInputCheckRT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].RT, Globals.DropManager.InputDeadZone);
            _triggerInputCheckLT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].LT, Globals.DropManager.InputDeadZone);
        }
    }
}