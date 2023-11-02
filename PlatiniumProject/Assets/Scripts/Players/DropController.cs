using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DropController : MonoBehaviour
{
    List<TriggerInputCheck> _allTriggerChecks = new List<TriggerInputCheck>();

    [SerializeField, Range(0f, 1f)] private float _inputDeadZone = .8f;

    int _triggerPushed;

    private void Awake()
    {
        _triggerPushed = 0;
        Players.OnPlayerConnect += GenerateTriggerChecks;
    }

    private void OnDestroy()
    {
        Players.OnPlayerConnect -= GenerateTriggerChecks;
        _allTriggerChecks.ForEach(x => x.OnTriggerStateChange -= ChangeValue);
    }

    private void GenerateTriggerChecks(int playerRole)
    {
        _allTriggerChecks.Add(new TriggerInputCheck(Players.PlayersController[playerRole].RT, _inputDeadZone));
        _allTriggerChecks[^1].OnTriggerStateChange += ChangeValue;
        _allTriggerChecks.Add(new TriggerInputCheck(Players.PlayersController[playerRole].LT, _inputDeadZone));
        _allTriggerChecks[^1].OnTriggerStateChange += ChangeValue;
    }

    private void ChangeValue(bool value)
    {
        _triggerPushed += value ? 1 : -1;
    }
}
