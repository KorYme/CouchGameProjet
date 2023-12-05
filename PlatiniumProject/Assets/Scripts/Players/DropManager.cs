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

    [SerializeField, Range (0f, 20f)] float _buildUpTime = 1f;
    [SerializeField, Range (0f, 5f)] float _synchronizationTime = 1f;
    [SerializeField, Range (0f, 5f)] float _timeToReleaseAfterAnimation = 1f;
    [SerializeField, Range(0f, 1f)] private float _inputDeadZone = .8f;
    [SerializeField] TMP_Text _text;
    [SerializeField] GameObject _dropSuccess;
    [SerializeField] DropAnimationBehaviour _dropAnimationBehaviour;


    [SerializeField] AK.Wwise.Event _dropMissedEvent;
    [SerializeField] List<AK.Wwise.Event> _musicEvents;
    [SerializeField] List<AK.Wwise.Event> _buildUpEvents;
    [SerializeField] List<AK.Wwise.Event> _dropEvents;
    [Space]
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
            _text.text = value.ToString();
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
        OnDropSuccess += () => _onDropSuccess?.Invoke();
        OnDropFail += () => _onDropFail?.Invoke();
        OnBeginBuildUp += () => _onBeginBuildUp?.Invoke();
        OnDropLoaded += () => _onDropTriggered?.Invoke();
        OnDropLoaded += () => _dropAnimationBehaviour.gameObject.SetActive(true);
        _triggerPressedNumber = 0;
        DropState = DROP_STATE.OUT_OF_DROP;
        _dropSuccess.SetActive(false);
        _beatManager = Globals.BeatManager as BeatManager;
        _beatManager.OnUserCueReceived += CheckUserCueName;
        OnDropStateChange += DropStateChange;
        _currentPhase = 0;
        _triggerSyncCoroutine = null;
        _dropAnimationBehaviour.OnDropAnimationEnd += () => StartCoroutine(DropTriggerRelease());
        _dropAnimationBehaviour.OnDropAnimationClimax += OnDropAnimationClimax;
    }

    private void OnDestroy()
    {
        _beatManager.OnUserCueReceived -= CheckUserCueName;
        _dropAnimationBehaviour.OnDropAnimationEnd -= () => StartCoroutine(DropTriggerRelease());
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
                _allDropControllers.ForEach(x => x.DisplayTriggers(false));
                DropState = DROP_STATE.OUT_OF_DROP;
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
            case "BuildUpStart":
                _buildUpEvents[_currentPhase]?.Post(gameObject);
                _beatManager.OnNextEntryCue += () =>
                {
                    _beatManager.OnBeatEndEvent.AddListener(AllTriggerRelease);
                    DropState = DROP_STATE.ON_DROP_PRESSING;
                    OnBeginBuildUp?.Invoke();
                };
                break;
            case "DropStart":
                _dropEvents[_currentPhase]?.Post(gameObject);
                _beatManager.OnNextEntryCue += () =>
                {
                    _beatManager.OnBeatEndEvent.RemoveListener(AllTriggerRelease);
                    if (_triggerPressedNumber >= Players.PlayerConnected * 2)
                    {
                        DropState = DROP_STATE.ON_DROP_RELEASING;
                        OnDropLoaded?.Invoke();
                    }
                    else
                    {
                        _dropMissedEvent?.Post(gameObject);
                        DropState = DROP_STATE.ON_DROP_MISSED;
                    }
                };
                break;
            case "MusicStart":
                _currentPhase++;
                if (_currentPhase >= 3)
                {
                    OnGameEnd?.Invoke();
                    DropState = DROP_STATE.OUT_OF_DROP;
                    return;
                }
                _musicEvents[_currentPhase].Post(gameObject);
                _beatManager.OnNextEntryCue += () =>
                {
                    DropState = DROP_STATE.OUT_OF_DROP;
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

    IEnumerator TriggerSynchronize()
    {
        yield return new WaitForSeconds(_synchronizationTime);
        _allDropControllers.ForEach(x => x.ForceTriggersRelease());
        _triggerSyncCoroutine = null;
    }

    private IEnumerator DropTriggerRelease()
    {
        yield return new WaitForSeconds(_timeToReleaseAfterAnimation);
        if (DropState == DROP_STATE.ON_DROP_RELEASING)
        {
            DropState = DROP_STATE.ON_DROP_MISSED;
        }
    }

    void OnDropAnimationClimax()
    {

    }
}
