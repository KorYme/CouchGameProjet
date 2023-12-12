using System;
using System.Collections;
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
        ON_DROP_WAIT_FOR_RELEASE,
        ON_DROP_RELEASING,
        ON_DROP_SUCCESS,
        ON_DROP_MISSED,
    }

    [Header("Wwise Event References"), Space]
    [SerializeField] AK.Wwise.Event _secondMusicStateEvent;
    [SerializeField] AK.Wwise.Event _thirdMusicStateEvent;
    [SerializeField] AK.Wwise.Event _endMusicStateEvent;
    [SerializeField] AK.Wwise.Event _dropSuccessEvent;
    [SerializeField] AK.Wwise.Event _dropMissEvent;
    [Header("Drop Parameters"), Space]
    [SerializeField, Range(0f, 1f)] float _inputDeadZone = .8f;
    [SerializeField, Range (0f, 5f)] float _pressingSynchronizationTime = 1f;
    [SerializeField, Range(.1f, 20)] float _pressingBuildUpTime = 3;
    [SerializeField, Range(.1f, 20)] float _releasingBuildUpTime = 3;
    [Header("Other References"), Space]
    [SerializeField] DropAnimationBehaviour _dropAnimationBehaviour;
    [Header("Unity Events"), Space]
    [SerializeField] UnityEvent _onBeginBuildUp;
    [SerializeField] UnityEvent _onDropTriggered;
    [SerializeField] UnityEvent _onDropSuccess;
    [SerializeField] UnityEvent _onDropFail;

    public float InputDeadZone => _inputDeadZone;
    DROP_STATE _dropState;
    public DROP_STATE DropState
    {
        get => _dropState;
        private set
        {
            _dropState = value;
            DropStateChange(value);
        }
    }
    public bool CanYouLetMeMove => _dropState == DROP_STATE.OUT_OF_DROP;
    public event Action OnBeginBuildUp;
    public event Action OnDropLoaded;
    public event Action OnDropSuccess;
    public event Action OnDropFail;
    public event Action OnGameEnd;
    int _currentPhase;
    int _triggerPressedNumber;
    BeatManager _beatManager;
    List<DropController> _allDropControllers = new();
    public List<DropController> AllDropControllers => _allDropControllers;
    public float PressingSynchronizationTime => _pressingSynchronizationTime;

    private void Awake()
    {
        Globals.DropManager ??= this;
    }

    private void Start()
    {
        _beatManager = Globals.BeatManager as BeatManager;
        _triggerPressedNumber = 0;
        _currentPhase = 0;
        DropState = DROP_STATE.OUT_OF_DROP;
        OnDropSuccess += () => _onDropSuccess?.Invoke();
        OnDropFail += () => _onDropFail?.Invoke();
        OnBeginBuildUp += () => _onBeginBuildUp?.Invoke();
        OnDropLoaded += () => _onDropTriggered?.Invoke();
        OnDropLoaded += () => _dropAnimationBehaviour.gameObject.SetActive(true);
        _beatManager.OnUserCueReceived += CheckUserCueName;
        _dropAnimationBehaviour.OnDropAnimationClimax += () => DropState = DROP_STATE.ON_DROP_RELEASING;
    }

    private void OnDestroy()
    {
        _beatManager.OnUserCueReceived -= CheckUserCueName;
        _dropAnimationBehaviour.OnDropAnimationClimax -= () => DropState = DROP_STATE.ON_DROP_RELEASING;
    }

    private void DropStateChange(DROP_STATE newState)
    {
        switch (newState)
        {
            case DROP_STATE.ON_DROP_PRESSING:   
                _allDropControllers.ForEach(x => x.DisplayTriggers(true));
                break;
            case DROP_STATE.ON_DROP_MISSED:
                _dropMissEvent?.Post(gameObject);
                OnDropFail?.Invoke();
                break;
            case DROP_STATE.ON_DROP_SUCCESS:
                _dropSuccessEvent?.Post(gameObject);
                OnDropSuccess?.Invoke();
                break;
            case DROP_STATE.OUT_OF_DROP:
                _allDropControllers.ForEach(x => x.DisplayTriggers(false));
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
                _beatManager.OnNextEntryCue += () =>
                {
                    StartCoroutine(BuildUpCoroutine());
                };
                break;
            case "MusicStart":
                _beatManager.OnNextExitCue += () =>
                {
                    _currentPhase++;
                    DropState = DROP_STATE.OUT_OF_DROP;
                    switch (_currentPhase)
                    {
                        case 1: _secondMusicStateEvent?.Post(gameObject); break;
                        case 2: _thirdMusicStateEvent?.Post(gameObject); break;
                        case 3: default: OnGameEnd?.Invoke(); _endMusicStateEvent?.Post(gameObject); break;
                    }
                };
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
                if (!triggerPressed) break;
                _triggerPressedNumber = _allDropControllers.Sum(x => x.TriggerPressed);
                if (_triggerPressedNumber == Players.PlayerConnected * 2)
                    DropState = DROP_STATE.ON_DROP_ALL_PRESSED;
                break;
            case DROP_STATE.ON_DROP_ALL_PRESSED:
                if (triggerPressed) break;
                _triggerPressedNumber = 0;
                DropState = DROP_STATE.ON_DROP_PRESSING;
                break;
            case DROP_STATE.ON_DROP_WAIT_FOR_RELEASE:
                if (triggerPressed) break;
                DropState = DROP_STATE.ON_DROP_MISSED;
                break;
            case DROP_STATE.ON_DROP_RELEASING:
                _triggerPressedNumber = _allDropControllers.Sum(x => x.TriggerPressed);
                if (_triggerPressedNumber == 0)
                    DropState = DROP_STATE.ON_DROP_SUCCESS;
                break;
            default:
                break;
        }
    }

    IEnumerator BuildUpCoroutine()
    {
        DropState = DROP_STATE.ON_DROP_PRESSING;
        OnBeginBuildUp?.Invoke();
        yield return new WaitForSeconds(_pressingBuildUpTime);
        if (_triggerPressedNumber >= Players.PlayerConnected * 2)
        {
            OnDropLoaded?.Invoke();
            DropState = DROP_STATE.ON_DROP_WAIT_FOR_RELEASE;
            yield return new WaitWhile(() => DropState == DROP_STATE.ON_DROP_WAIT_FOR_RELEASE);
            if (DropState == DROP_STATE.ON_DROP_RELEASING)
            {
                yield return new WaitForSeconds(_releasingBuildUpTime);
                if (DropState == DROP_STATE.ON_DROP_RELEASING)
                {
                    DropState = DROP_STATE.ON_DROP_MISSED;
                }
            }
            else
            {
                DropState = DROP_STATE.ON_DROP_MISSED;
            }
        }
        else
        {
            DropState = DROP_STATE.ON_DROP_MISSED;
        }
    }
}