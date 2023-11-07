using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropController : MonoBehaviour
{
    public enum DROP_STATE
    {
        OUT_OF_DROP,
        ON_DROP_PRESSING,
        ON_DROP_RELEASING,
        ON_DROP_MISSED,
    }

    [SerializeField, Range(0f, 1f)] private float _inputDeadZone = .8f;

    DROP_STATE _dropState;
    event Action<DROP_STATE> OnDropStateChange;
    public DROP_STATE DropState
    {
        get => _dropState;
        set
        {
            OnDropStateChange?.Invoke(value);
            _dropState = value;
        }
    }
    int _triggerPressedNumber;
    BeatManager _beatManager;
    readonly List<TriggerInputCheck> _allTriggerChecks = new();

    private void Awake()
    {
        Globals.DropController ??= this;
        Players.OnPlayerConnect += GenerateTriggerChecks;
    }

    private void Start()
    {
        _beatManager = Globals.BeatTimer as BeatManager;
        _beatManager.OnUserCueReceived += CheckUserCueName;
        _dropState = DROP_STATE.OUT_OF_DROP;
        _triggerPressedNumber = 0;
        OnDropStateChange += DropStateChange;
    }

    private void OnDestroy()
    {
        Players.OnPlayerConnect -= GenerateTriggerChecks;
        _allTriggerChecks.ForEach(x => x.OnTriggerStateChange -= ChangeValue);
        _beatManager.OnUserCueReceived -= CheckUserCueName;
        OnDropStateChange -= DropStateChange;
    }

    private void GenerateTriggerChecks(int playerRole)
    {
        _allTriggerChecks.Add(new TriggerInputCheck(Players.PlayersController[playerRole].RT, _inputDeadZone));
        _allTriggerChecks[^1].OnTriggerStateChange += ChangeValue;
        _allTriggerChecks.Add(new TriggerInputCheck(Players.PlayersController[playerRole].LT, _inputDeadZone));
        _allTriggerChecks[^1].OnTriggerStateChange += ChangeValue;
    }

    private void ChangeValue(bool triggerPressed)
    {
        _triggerPressedNumber = _allTriggerChecks.Where(x => x.IsPressed).Count();
        switch (_dropState)
        {
            case DROP_STATE.ON_DROP_RELEASING:
                if (_triggerPressedNumber == 0)
                {
                    Success();
                }
                break;
            default:
                break;
        }
    }

    void CheckUserCueName(string userCueName)
    {
        Debug.Log(userCueName);
        switch (userCueName)
        {
            case "PreDropStart":
                DropState = DROP_STATE.ON_DROP_PRESSING;
                break;
            case "DropStart":
                DropState = DROP_STATE.ON_DROP_RELEASING;
                break;
            case "DropEnd":
                if (DropState != DROP_STATE.ON_DROP_RELEASING) return;
                if (_triggerPressedNumber == 0)
                {
                    Success();
                }
                else
                {
                    DropState = DROP_STATE.ON_DROP_MISSED;
                }
                break;
            default:
                break;
        }
    }

    private void Success()
    {
        Debug.Log("Success Drop");
        DropState = DROP_STATE.OUT_OF_DROP;
    }

    private void DropStateChange(DROP_STATE newState)
    {
        Debug.Log(newState.ToString());
        switch (newState)
        {
            case DROP_STATE.OUT_OF_DROP:
                Globals.SpawnManager.CanSpawnClients = true;
                break;
            case DROP_STATE.ON_DROP_PRESSING:
                Globals.SpawnManager.CanSpawnClients = false;
                break;
            case DROP_STATE.ON_DROP_RELEASING:
                if (_triggerPressedNumber != Players.PlayerConnected * 2)
                {
                    DropState = DROP_STATE.ON_DROP_MISSED;
                }
                break;
            case DROP_STATE.ON_DROP_MISSED:
                _beatManager.OnNextBeatStart += () => DropState = DROP_STATE.OUT_OF_DROP;
                break;
            default:
                break;
        }
    }
}
