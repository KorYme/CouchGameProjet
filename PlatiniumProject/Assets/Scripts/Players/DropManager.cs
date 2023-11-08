using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DropManager : MonoBehaviour
{
    public enum DROP_STATE
    {
        OUT_OF_DROP,
        ON_DROP_PRESSING,
        ON_DROP_ALL_PRESSED,
        ON_DROP_RELEASING,
        ON_DROP_MISSED,
    }

    [SerializeField, Range(0f, 1f)] private float _inputDeadZone = .8f;
    public float InputDeadZone => _inputDeadZone;

    DROP_STATE _dropState;
    public DROP_STATE DropState
    {
        get => _dropState;
        set
        {
            OnDropStateChange?.Invoke(value);
            _dropState = value;
        }
    }
    event Action<DROP_STATE> OnDropStateChange;

    public event Action OnDropSuccess;

    int _triggerPressedNumber;
    BeatManager _beatManager;
    List<DropController> _allDropControllers = new();
    public List<DropController> AllDropControllers => _allDropControllers;

    private void Awake()
    {
        Globals.DropManager ??= this;
    }

    private void Start()
    {
        _triggerPressedNumber = 0;
        _dropState = DROP_STATE.OUT_OF_DROP;
        _beatManager = Globals.BeatManager as BeatManager;
        _beatManager.OnUserCueReceived += CheckUserCueName;
        OnDropStateChange += DropStateChange;
    }

    private void OnDestroy()
    {
        _beatManager.OnUserCueReceived -= CheckUserCueName;
        OnDropStateChange -= DropStateChange;
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
                    Debug.Log("Not enough triggers pushed, drop missed");
                }
                break;
            case DROP_STATE.ON_DROP_MISSED:
                _beatManager.OnNextBeatStart += () => DropState = DROP_STATE.OUT_OF_DROP;
                break;
            default:
                break;
        }
    }

    void CheckUserCueName(string userCueName)
    {
        //Debug.Log(userCueName);
        switch (userCueName)
        {
            case "BuildUpStart":
                _beatManager.OnNextBeatStart += () =>
                {
                    DropState = DROP_STATE.ON_DROP_PRESSING;
                    _beatManager.OnBeatEndEvent.AddListener(AllTriggerRelease);
                };
                break;
            case "DropStart":
                _beatManager.OnBeatEndEvent.RemoveListener(AllTriggerRelease);
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

    public void UpdateTriggerValue(bool triggerPressed)
    {
        switch (_dropState)
        {
            case DROP_STATE.ON_DROP_PRESSING:
                if (!_beatManager.IsInsideBeatWindow) return;
                _triggerPressedNumber = _allDropControllers.Sum(x => x.TriggerPressed);
                if (_triggerPressedNumber == Players.PlayerConnected * 2)
                {
                    DropState = DROP_STATE.ON_DROP_ALL_PRESSED;
                }
                break;
            case DROP_STATE.ON_DROP_ALL_PRESSED:
                if (!triggerPressed)
                {
                    _allDropControllers.ForEach(x => x.ForceTriggersRelease());
                    _triggerPressedNumber = 0;
                    DropState = DROP_STATE.ON_DROP_PRESSING;
                }
                break;
            case DROP_STATE.ON_DROP_RELEASING:
                _triggerPressedNumber = _allDropControllers.Sum(x => x.TriggerPressed);
                if (_triggerPressedNumber == 0)
                {
                    Success();
                }
                break;
            default:
                break;
        }
    }

    void AllTriggerRelease()
    {
        if (DropState == DROP_STATE.ON_DROP_ALL_PRESSED) return;
        _allDropControllers.ForEach(x => x.ForceTriggersRelease());
    }

    private void Success()
    {
        Debug.Log("Success Drop");
        OnDropSuccess?.Invoke();
        DropState = DROP_STATE.OUT_OF_DROP;
    }
}
