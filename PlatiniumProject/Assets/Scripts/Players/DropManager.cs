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
        ON_DROP_ALL_RELEASED,
        ON_DROP_MISSED,
    }

    [Header("Wwise Event References"), Space]
    [SerializeField] AK.Wwise.Event _secondMusicStateEvent;
    [SerializeField] AK.Wwise.Event _thirdMusicStateEvent;
    [SerializeField] AK.Wwise.Event _endMusicStateEvent;
    [SerializeField] AK.Wwise.Event _dropDefaultEvent;
    [SerializeField] AK.Wwise.Event _dropSucceededEvent;
    [SerializeField] AK.Wwise.Event _dropMissedEvent;

    [Header("Drop Parameters"), Space]
    [SerializeField, Range(0f, 1f)] float _inputDeadZone = .8f;
    [SerializeField, Range (0f, 5f)] float _synchronizationTime = 1f;
    [SerializeField, Range(1, 10)] int _pressingLoop = 3;
    [SerializeField, Range(1, 10)] int _releasingLoop = 3;
    [Header("Other References"), Space]
    [SerializeField] TMP_Text _dropStateText;
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
            OnDropStateChange?.Invoke(value);
            _dropStateText.text = value.ToString();
            _dropState = value;
        }
    }
    event Action<DROP_STATE> OnDropStateChange;
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
    Coroutine _triggerSyncCoroutine;
    public List<DropController> AllDropControllers => _allDropControllers;


    private void Awake()
    {
        Globals.DropManager ??= this;
    }

    private void Start()
    {
        _beatManager = Globals.BeatManager as BeatManager;
        _triggerPressedNumber = 0;
        _currentPhase = 0;
        _triggerSyncCoroutine = null;
        DropState = DROP_STATE.OUT_OF_DROP;
        OnDropSuccess += () => _onDropSuccess?.Invoke();
        OnDropFail += () => _onDropFail?.Invoke();
        OnBeginBuildUp += () => _onBeginBuildUp?.Invoke();
        OnDropLoaded += () => _onDropTriggered?.Invoke();
        OnDropLoaded += () => _dropAnimationBehaviour.gameObject.SetActive(true);
        OnDropStateChange += DropStateChange;
        _beatManager.OnUserCueReceived += CheckUserCueName;
        _dropAnimationBehaviour.OnDropAnimationClimax += () => RepetitionCounter(_releasingLoop, DropReleaseCheck);
        _dropDefaultEvent?.Post(gameObject);
    }

    private void OnDestroy()
    {
        _beatManager.OnUserCueReceived -= CheckUserCueName;
        _dropAnimationBehaviour.OnDropAnimationEnd -= () => RepetitionCounter(_releasingLoop, DropReleaseCheck);
    }

    private void DropStateChange(DROP_STATE newState)
    {
        switch (newState)
        {
            case DROP_STATE.ON_DROP_PRESSING:   
                _allDropControllers.ForEach(x => x.DisplayTriggers(true));
                break;
            case DROP_STATE.ON_DROP_MISSED:
                OnDropFail?.Invoke();
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
                    DropState = DROP_STATE.ON_DROP_PRESSING;
                    OnBeginBuildUp?.Invoke();
                    RepetitionCounter(_pressingLoop + 1, DropPressingCheck);
                };
                break;
            case "MusicStart":
                _beatManager.OnNextExitCue += () =>
                {
                    _currentPhase++;
                    DropState = DROP_STATE.OUT_OF_DROP;
                    _dropDefaultEvent?.Post(gameObject);
                    switch (_currentPhase)
                    {
                        case 1:
                            _secondMusicStateEvent?.Post(gameObject);
                            break;
                        case 2:
                            _thirdMusicStateEvent?.Post(gameObject);
                            break;
                        case 3:
                        default:
                            OnGameEnd?.Invoke();
                            _endMusicStateEvent?.Post(gameObject);
                            break;
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
                if (_triggerSyncCoroutine == null)
                {
                    _triggerSyncCoroutine = StartCoroutine(TriggerSynchronize());
                }
                else
                {
                    _triggerPressedNumber = _allDropControllers.Sum(x => x.TriggerPressed);
                }
                if (_triggerPressedNumber == Players.PlayerConnected * 2)
                {
                    StopCoroutine(_triggerSyncCoroutine);
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
                    DropState = DROP_STATE.ON_DROP_ALL_RELEASED;
                    _dropSucceededEvent?.Post(gameObject);
                    OnDropSuccess?.Invoke();
                }
                break;
            default:
                break;
        }
    }

    IEnumerator TriggerSynchronize()
    {
        yield return new WaitForSeconds(_synchronizationTime);
        _allDropControllers.ForEach(x => x.ForceTriggersRelease());
        _triggerSyncCoroutine = null;
    }
    
    void DropPressingCheck()
    {
        if (_triggerPressedNumber >= Players.PlayerConnected * 2)
        {
            OnDropLoaded?.Invoke();
            DropState = DROP_STATE.ON_DROP_RELEASING;
        }
        else
        {
            _dropMissedEvent?.Post(gameObject);
            DropState = DROP_STATE.ON_DROP_MISSED;
        }
    }

    void DropReleaseCheck()
    {
        if (DropState == DROP_STATE.ON_DROP_RELEASING)
        {
            DropState = DROP_STATE.ON_DROP_MISSED;
        }
    }

    void RepetitionCounter(int repetitionNeeded, Action action)
    {
        repetitionNeeded--;
        if (repetitionNeeded > 0)
        {
            _beatManager.OnNextEntryCue += () => RepetitionCounter(repetitionNeeded, action);
        }
        else
        {
            action?.Invoke();
        }
    }
}
