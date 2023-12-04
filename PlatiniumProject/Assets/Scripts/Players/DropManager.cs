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

    [SerializeField, Range (0f, 5f)] float _synchronizationTime = 1f;
    [SerializeField, Range(0f, 1f)] private float _inputDeadZone = .8f;
    [SerializeField] TMP_Text _text;
    [SerializeField] GameObject _dropSuccess;
    [SerializeField] DropAnimationBehaviour _dropAnimationBehaviour;

    [SerializeField] AK.Wwise.Event _firstDropEvent;
    [SerializeField] AK.Wwise.Event _secondStateEvent;
    [SerializeField] AK.Wwise.Event _secondDropEvent;
    [SerializeField] AK.Wwise.Event _thirdStateEvent;
    [SerializeField] AK.Wwise.Event _thirdDropEvent;
    [Space]
    [SerializeField] UnityEvent _onDropTriggering;
    [SerializeField] UnityEvent _onDropTriggered;
    [SerializeField] UnityEvent _onDropSuccess;
    [SerializeField] UnityEvent _onDropFail;
    [SerializeField] UnityEvent _onDropEnd;

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
    public List<DropController> AllDropControllers => _allDropControllers;

    int _dropPassed;
    int _triggerPressedNumber;
    BeatManager _beatManager;
    List<DropController> _allDropControllers = new();
    Coroutine _triggerSyncCoroutine;


    private void Awake()
    {
        Globals.DropManager ??= this;
    }

    private void Start()
    {
        OnDropSuccess += () => _onDropSuccess?.Invoke();
        OnDropFail += () => _onDropFail?.Invoke();
        OnBeginBuildUp += () => _onDropTriggering?.Invoke();
        OnDropLoaded += () =>
        {
            _onDropTriggered?.Invoke();
            _dropAnimationBehaviour.gameObject.SetActive(true);
        };
        _triggerPressedNumber = 0;
        DropState = DROP_STATE.OUT_OF_DROP;
        _dropSuccess.SetActive(false);
        _beatManager = Globals.BeatManager as BeatManager;
        _beatManager.OnUserCueReceived += CheckUserCueName;
        OnDropStateChange += DropStateChange;
        _dropPassed = 0;
        _triggerSyncCoroutine = null;
    }

    private void OnDestroy()
    {
        _beatManager.OnUserCueReceived -= CheckUserCueName;
    }

    private void DropStateChange(DROP_STATE newState)
    {
        switch (newState)
        {
            case DROP_STATE.OUT_OF_DROP:
                _allDropControllers.ForEach(x => x.DisplayTriggers(false));
                break;
            case DROP_STATE.ON_DROP_PRESSING:   
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
                    OnBeginBuildUp?.Invoke();
                };
                break;
            case "DropStart":
                _beatManager.OnBeatEndEvent.RemoveListener(AllTriggerRelease);
                if (_triggerPressedNumber >= Players.PlayerConnected * 2)
                {
                    DropState = DROP_STATE.ON_DROP_RELEASING;
                    OnDropLoaded?.Invoke();
                }
                else
                {
                    DropState = DROP_STATE.ON_DROP_MISSED;
                }
                break;
            case "DropEnd":
                if (DropState != DROP_STATE.OUT_OF_DROP)
                {
                    DropState = DROP_STATE.ON_DROP_MISSED;
                }
                _dropPassed++;
                if (_dropPassed >= 3)
                {
                    Time.timeScale = 0f;
                }
                _onDropEnd?.Invoke();
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
}
