using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField] TMP_Text _text;
    [SerializeField] GameObject _dropSuccess;

    public float InputDeadZone => _inputDeadZone;

    DROP_STATE _dropState;
    public DROP_STATE DropState
    {
        get => _dropState;
        private set
        {
            OnDropStateChange?.Invoke(value);
            _text.text = value.ToString();
            _dropState = value;
        }
    }
    event Action<DROP_STATE> OnDropStateChange;

    public event Action OnDropSuccess;
    public event Action OnDropFail;

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
        DropState = DROP_STATE.OUT_OF_DROP;
        _dropSuccess.SetActive(false);
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
                _allDropControllers.ForEach(x => x.DisplayTriggers(false));
                break;
            case DROP_STATE.ON_DROP_PRESSING:
                Globals.SpawnManager.CanSpawnClients = false;
                _allDropControllers.ForEach(x => x.DisplayTriggers(true));
                break;
            case DROP_STATE.ON_DROP_MISSED:
                OnDropFail?.Invoke();
                _beatManager.OnNextBeatStart += () => DropState = DROP_STATE.OUT_OF_DROP;
                break;
            default:
                break;
        }
    }

    void CheckUserCueName(string userCueName)
    {
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
                if (_triggerPressedNumber >= Players.PlayerConnected * 2)
                {
                    DropState = DROP_STATE.ON_DROP_RELEASING;
                }
                else
                {
                    Debug.Log("Not enough triggers pushed, drop missed");
                    DropState = DROP_STATE.ON_DROP_MISSED;
                }
                break;
            case "DropEnd":
                if (DropState != DROP_STATE.OUT_OF_DROP)
                {
                    Debug.Log("Not enough triggers released, drop missed");
                    DropState = DROP_STATE.ON_DROP_MISSED;
                }
                break;
            default:
                break;
        }
    }

    public void UpdateTriggerValue(bool triggerPressed)
    {
        switch (DropState)
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
                    Debug.Log("Success Drop");
                    _dropSuccess.SetActive(true);
                    StartCoroutine(SuccessDisplay());
                    DropState = DROP_STATE.OUT_OF_DROP;
                    OnDropSuccess?.Invoke();
                }
                break;
            default:
                break;
        }
    }

    IEnumerator SuccessDisplay()
    {
        yield return new WaitForSeconds(7f);
        _dropSuccess.SetActive(false);
    }

    void AllTriggerRelease()
    {
        if (DropState == DROP_STATE.ON_DROP_ALL_PRESSED) return;
        _allDropControllers.ForEach(x => x.ForceTriggersRelease());
    }
}
