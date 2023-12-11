using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.UI;

public class DropController : MonoBehaviour, IIsControllable
{
    [SerializeField] PlayerSyncEvents _syncEvents;
    [SerializeField] Image _rtImage, _ltImage;
    [SerializeField] PlayerRole _playerRole;

    TriggerInputCheck _triggerInputCheckRT;
    TriggerInputCheck _triggerInputCheckLT;

    Coroutine _syncCoroutine;

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
        _ltImage.enabled = false;
        _rtImage.enabled = false;
        yield return new WaitWhile(() => Players.PlayersController[(int)_playerRole] == null);
        Players.AddListenerPlayerController(this);
        _triggerInputCheckRT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].RT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckLT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].LT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckRT.OnTriggerPerformed += value => CheckTriggerState(_triggerInputCheckLT, value);
        _triggerInputCheckLT.OnTriggerPerformed += value => CheckTriggerState(_triggerInputCheckRT, value);
        Globals.DropManager.AllDropControllers.Add(this);
        _triggerInputCheckRT.OnTriggerStateChange += state => _rtImage.color = ChangeColor(state);
        _triggerInputCheckLT.OnTriggerStateChange += state => _ltImage.color = ChangeColor(state);
        _syncEvents[_playerRole].isNotSyncEvent?.Post(gameObject);
    }

    private void OnDestroy()
    {
        if (Players.PlayersController[(int)_playerRole] == null) return;
        Players.RemoveListenerPlayerController(this);
        _triggerInputCheckRT.OnTriggerPerformed -= value => CheckTriggerState(_triggerInputCheckLT, value);
        _triggerInputCheckLT.OnTriggerPerformed -= value => CheckTriggerState(_triggerInputCheckRT, value);
        Globals.DropManager?.AllDropControllers.Remove(this);
        _triggerInputCheckRT.OnTriggerStateChange -= state => _rtImage.color = ChangeColor(state);
        _triggerInputCheckLT.OnTriggerStateChange -= state => _ltImage.color = ChangeColor(state);
    }

    void CheckTriggerState(TriggerInputCheck trigger, bool value)
    {
        Globals.DropManager.UpdateTriggerValue(value);
        if (_syncCoroutine == null)
        {
            _syncCoroutine = StartCoroutine(TriggerSynchronize());
        }
        if (value)
        {
            if (trigger.TriggerState == TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_TIME)
            {
                _syncEvents[_playerRole].isSyncEvent?.Post(gameObject);
                StopCoroutine(_syncCoroutine);
                _syncCoroutine = null;
            }
        }
        else
        {
            _syncEvents[_playerRole].isNotSyncEvent?.Post(gameObject);
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

    public void DisplayTriggers(bool enableTriggers)
    {
        _ltImage.gameObject.SetActive(enableTriggers);
        _rtImage.gameObject.SetActive(enableTriggers);
    }

    public void ChangeController()
    {
        if (Players.PlayersController[(int)_playerRole] != null) {
            _triggerInputCheckRT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].RT, Globals.DropManager.InputDeadZone);
            _triggerInputCheckLT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].LT, Globals.DropManager.InputDeadZone);
        }
    }
}