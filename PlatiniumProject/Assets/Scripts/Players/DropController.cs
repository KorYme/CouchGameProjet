using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropController : MonoBehaviour
{
    [SerializeField] PlayerRole _playerRole;
    [SerializeField] Image _rtImage, _ltImage;

    TriggerInputCheck _triggerInputCheckRT;
    TriggerInputCheck _triggerInputCheckLT;

    public int TriggerPressed
    {
        get
        {
            if (_triggerInputCheckRT == null) return 0;
            return (_triggerInputCheckRT.TriggerState == TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_BEAT ? 1 : 0) 
                + (_triggerInputCheckLT.TriggerState == TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_BEAT ? 1 : 0);
        }
    }

    private IEnumerator Start()
    {
        _ltImage.enabled = false;
        _rtImage.enabled = false;
        yield return new WaitWhile(() => Players.PlayersController[(int)_playerRole] == null);
        _triggerInputCheckRT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].RT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckLT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].LT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckRT.OnTriggerPerformed += Globals.DropManager.UpdateTriggerValue;
        _triggerInputCheckLT.OnTriggerPerformed += Globals.DropManager.UpdateTriggerValue;
        Globals.DropManager.AllDropControllers.Add(this);
        _triggerInputCheckRT.OnTriggerStateChange += value => _rtImage.color = ChangeColor(value);
        _triggerInputCheckLT.OnTriggerStateChange += value => _ltImage.color = ChangeColor(value);
    }

    private void OnDestroy()
    {
        if (Players.PlayersController[(int)_playerRole] == null) return;
        _triggerInputCheckRT.OnTriggerPerformed -= Globals.DropManager.UpdateTriggerValue;
        _triggerInputCheckLT.OnTriggerPerformed -= Globals.DropManager.UpdateTriggerValue;
        Globals.DropManager?.AllDropControllers.Remove(this);
        _triggerInputCheckRT.OnTriggerStateChange -= value => _rtImage.color = ChangeColor(value);
        _triggerInputCheckLT.OnTriggerStateChange -= value => _ltImage.color = ChangeColor(value);
    }

    Color ChangeColor(TriggerInputCheck.TRIGGER_STATE state)
    {
        switch (state)
        {
            case TriggerInputCheck.TRIGGER_STATE.RELEASED:
                return Color.yellow;
            case TriggerInputCheck.TRIGGER_STATE.PRESSED_ON_BEAT:
                return Color.green;
            case TriggerInputCheck.TRIGGER_STATE.NEED_TO_BE_RELEASED:
                return Color.red;
            default:
                return Color.white;
        }
    }

    public void ForceTriggersRelease()
    {
        _triggerInputCheckLT.TriggerState = TriggerInputCheck.TRIGGER_STATE.NEED_TO_BE_RELEASED;
        _triggerInputCheckRT.TriggerState = TriggerInputCheck.TRIGGER_STATE.NEED_TO_BE_RELEASED;
    }

    public void DisplayTriggers(bool enableTriggers)
    {
        _ltImage.gameObject.SetActive(enableTriggers);
        _rtImage.gameObject.SetActive(enableTriggers);
    }
}