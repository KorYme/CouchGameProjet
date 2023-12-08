using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour, ITimingable
{

    #region FIELDS
    [Header("References"), Space]
    [SerializeField] AK.Wwise.Event _mainMusicEvent;
    [SerializeField] AK.Wwise.Event _firstStateEvent;

    [Header("Parameters"), Space]
    [SerializeField, Range(0f, .5f), Tooltip("Timing window before the beat which allows input")]
    float _timingBeforeBeat = .1f;

    [SerializeField, Range(0f, .5f), Tooltip("Timing window after the beat which allows input")]
    float _timingAfterBeat = .3f;


    [Header("Events"), Space]
    [SerializeField, Tooltip("This event is called exactly on the thiming of the beat")] 
    UnityEvent _onBeatEvent;
    [SerializeField, Tooltip("This event is called on the first frame an input can be received")]
    UnityEvent _onBeatStartEvent;
    [SerializeField, Tooltip("This event is called on the first frame an input cannot be received anymore")] 
    UnityEvent _onBeatEndEvent;

    int _beatDurationInMilliseconds = 0;
    DateTime _lastBeatTime;
    Coroutine _beatCoroutine;

    public event Action OnNextBeatStart;
    public event Action OnNextBeat;
    public event Action OnNextBeatEnd;
    public event Action OnNextEntryCue;
    public event Action<string> OnUserCueReceived;
    #endregion

    #region PROPERTIES
    public int BeatDurationInMilliseconds => _beatDurationInMilliseconds;
    public UnityEvent OnBeatEvent => _onBeatEvent;
    public UnityEvent OnBeatStartEvent => _onBeatStartEvent;
    public UnityEvent OnBeatEndEvent => _onBeatEndEvent;

    public bool IsInsideBeatWindow => IsInBeatWindowBefore || IsInBeatWindowAfter;
    public bool IsInBeatWindowBefore => (DateTime.Now - _lastBeatTime).TotalMilliseconds < (_timingAfterBeat * _beatDurationInMilliseconds);
    public bool IsInBeatWindowAfter => (DateTime.Now - _lastBeatTime).TotalMilliseconds > _beatDurationInMilliseconds - (_timingBeforeBeat * _beatDurationInMilliseconds);

    public double BeatDeltaTime => (DateTime.Now - _lastBeatTime).TotalMilliseconds;

    #endregion

    #region PROCEDURES
    private void Awake()
    {
        if (Globals.BeatManager != null)
        {
            Destroy(gameObject);
            return;
        }
        Globals.BeatManager = this;
    }

    private IEnumerator Start()
    {
        _beatDurationInMilliseconds = 1000;
        _onBeatEvent.AddListener(() =>
        {
            OnNextBeat?.Invoke();
            OnNextBeat = null;
        });
        _onBeatStartEvent.AddListener(() =>
        {
            OnNextBeatStart?.Invoke();
            OnNextBeatStart = null;
        });
        _onBeatEndEvent.AddListener(() =>
        {
            OnNextBeatEnd?.Invoke();
            OnNextBeatEnd = null;
        });
        yield return null;
        _mainMusicEvent?.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncGrid | (uint)AkCallbackType.AK_MusicSyncUserCue | (uint)AkCallbackType.AK_MusicSyncEntry, BeatCallBack);
        _firstStateEvent?.Post(gameObject);
    }

    private void OnDestroy()
    {
        _onBeatEvent.RemoveAllListeners();
        _onBeatStartEvent.RemoveAllListeners();
        _onBeatEndEvent.RemoveAllListeners();
        OnNextBeatStart = null;
        OnNextBeat = null;
        OnNextBeatEnd = null;
    }

    private void BeatCallBack(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        AkMusicSyncCallbackInfo info = in_info as AkMusicSyncCallbackInfo;
        switch (in_type)
        { 
            case AkCallbackType.AK_MusicSyncGrid:
                _beatCoroutine ??= StartCoroutine(BeatCoroutine());
                _lastBeatTime = DateTime.Now;
                _beatDurationInMilliseconds = (int)((info?.segmentInfo_fGridDuration ?? 1) * 1000);
                OnBeatEvent?.Invoke();
                if (OnNextBeat != null)
                {
                    OnNextBeat.Invoke();
                    OnNextBeat = null;
                }
                break;
            case AkCallbackType.AK_MusicSyncUserCue:
                OnUserCueReceived?.Invoke(info?.userCueName ?? "");
                break;
            case AkCallbackType.AK_MusicSyncEntry:
                OnNextEntryCue?.Invoke();
                OnNextEntryCue = null;
                break;
            default:
                break;
        }
    }

    IEnumerator BeatCoroutine()
    {
        while (true)
        {
            yield return new WaitWhile(() => IsInsideBeatWindow);
            OnBeatEndEvent?.Invoke();
            yield return new WaitUntil(() => IsInsideBeatWindow);
            OnBeatStartEvent?.Invoke();
        }
    }
    #endregion
}
