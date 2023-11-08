using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    [SerializeField] PlayerRole _playerRole;

    TriggerInputCheck _triggerInputCheckRT;
    TriggerInputCheck _triggerInputCheckLT;

    public int TriggerPressed
    {
        get
        {
            if (_triggerInputCheckRT == null) return 0;
            return (_triggerInputCheckRT.IsPressedOnBeat ? 1 : 0) + (_triggerInputCheckLT.IsPressedOnBeat ? 1 : 0);
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitWhile(() => Players.PlayersController[(int)_playerRole] == null);
        _triggerInputCheckRT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].RT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckLT = new TriggerInputCheck(Players.PlayersController[(int)_playerRole].LT, Globals.DropManager.InputDeadZone);
        _triggerInputCheckRT.OnTriggerStateChange += Globals.DropManager.UpdateTriggerValue;
        _triggerInputCheckLT.OnTriggerStateChange += Globals.DropManager.UpdateTriggerValue;
        Globals.DropManager.AllDropControllers.Add(this);
    }

    private void OnDestroy()
    {
        if (Players.PlayersController[(int)_playerRole] == null) return;
        _triggerInputCheckRT.OnTriggerStateChange -= Globals.DropManager.UpdateTriggerValue;
        _triggerInputCheckLT.OnTriggerStateChange -= Globals.DropManager.UpdateTriggerValue;
        Globals.DropManager?.AllDropControllers.Remove(this);
    }

    public void ForceTriggersRelease()
    {
        _triggerInputCheckLT.NeedToBeReleased = true;
        _triggerInputCheckRT.NeedToBeReleased = true;
    }
}
