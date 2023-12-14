using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class BeatManager : MonoBehaviour, ITimingable
{

    #region FIELDS
    [Header("Wwise Events References"), Space]
    [SerializeField] AK.Wwise.Event _mainMusicEvent;
    [SerializeField] AK.Wwise.Event _introMusicStateEvent;
    [SerializeField] AK.Wwise.Event _firstMusicStateEvent;
    [SerializeField] AK.Wwise.Event _pauseMusicEvent;
    [SerializeField] AK.Wwise.Event _resumeMusicEvent;

    [Header("Parameters"), Space]
    [SerializeField, Range(0f, .5f), Tooltip("Timing window before the beat which allows input")]
    float _timingBeforeBeat = .1f;
    [SerializeField, Range(0f, .5f), Tooltip("Timing window after the beat which allows input")]
    float _timingAfterBeat = .3f;


    [Header("Unity Events"), Space]
    [SerializeField, Tooltip("This event is called exactly on the thiming of the beat")] 
    UnityEvent _onBeatEvent;
    [SerializeField, Tooltip("This event is called on the first frame an input can be received")]
    UnityEvent _onBeatStartEvent;
    [SerializeField, Tooltip("This event is called on the first frame an input cannot be received anymore")] 
    UnityEvent _onBeatEndEvent;


    int _beatDurationInMilliseconds = 0;
    DateTime _lastBeatTime;
    DateTime _lastPauseTime;
    Coroutine _beatCoroutine;

    public event Action OnNextBeatStart;
    public event Action OnNextBeat;
    public event Action OnNextBeatEnd;
    public event Action OnNextEntryCue;
    public event Action OnNextExitCue;
    public event Action<string> OnUserCueReceived;
    #endregion

    #region PROPERTIES
    public bool IsPlaying { get; private set; }
    public int BeatDurationInMilliseconds => _beatDurationInMilliseconds;
    public UnityEvent OnBeatEvent => _onBeatEvent;
    public UnityEvent OnBeatStartEvent => _onBeatStartEvent;
    public UnityEvent OnBeatEndEvent => _onBeatEndEvent;

    public bool IsInsideBeatWindow => IsInBeatWindowBefore || IsInBeatWindowAfter;
    public bool IsInBeatWindowBefore => BeatDeltaTime < (_timingAfterBeat * _beatDurationInMilliseconds);
    public bool IsInBeatWindowAfter => BeatDeltaTime > _beatDurationInMilliseconds - (_timingBeforeBeat * _beatDurationInMilliseconds);

    public double BeatDeltaTime => (DateTime.Now - _lastBeatTime).TotalMilliseconds - (IsPlaying ? 0 : (DateTime.Now - _lastPauseTime).TotalMilliseconds);

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
        IsPlaying = false;
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
        Globals.MenuMusicPlayer?.StopMenuMusic();
        IsPlaying = true;
        _mainMusicEvent?.Post(gameObject, 
            (uint)AkCallbackType.AK_MusicSyncGrid | (uint)AkCallbackType.AK_MusicSyncUserCue | (uint)AkCallbackType.AK_MusicSyncEntry | (uint)AkCallbackType.AK_MusicSyncExit, 
            BeatCallBack);
        (Globals.TutorialManager.UseTutorial ? _introMusicStateEvent : _firstMusicStateEvent)?.Post(gameObject);
    }

    private void BeatCallBack(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        if (!Globals.DropManager.IsGamePlaying) return;
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
                Action tmpEntry = OnNextEntryCue;
                OnNextEntryCue = null;
                tmpEntry?.Invoke();
                break;
            case AkCallbackType.AK_MusicSyncExit:
                Action tmpExit = OnNextExitCue;
                OnNextExitCue = null;
                tmpExit?.Invoke();
                break;
            default:
                break;
        }
    }

    public void PauseOrResumeMainMusic(bool isGamePaused)
    {
        IsPlaying = !isGamePaused;
        if (!IsPlaying)
        {
            _lastPauseTime = DateTime.Now;
        }
        (isGamePaused ? _pauseMusicEvent : _resumeMusicEvent)?.Post(gameObject);
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

    public void PlayFirstMusic() => _firstMusicStateEvent?.Post(gameObject);

    public void StopBeat()
    {
        StopCoroutine(_beatCoroutine);
        _lastBeatTime = DateTime.Now;
    }
    #endregion
}
