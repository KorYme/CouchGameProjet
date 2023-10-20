using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour, ITimingable
{
    [Header("References"),Space]
    [SerializeField] AK.Wwise.Event _beatWwiseEvent;

    [Header("Events"), Space]
    [SerializeField] UnityEvent _onBeatEvent;
    [SerializeField] UnityEvent _onBeatStartEvent;
    [SerializeField] UnityEvent _onBeatEndEvent;

    [Header("Parameters"), Space]
    [SerializeField] int _timingWindowInMilliseconds;

    DateTime _lastBeatTime;
    int _beatDurationInMilliseconds;
    Coroutine _beatCoroutine;


    public UnityEvent OnBeatEvent => _onBeatEvent;
    public UnityEvent OnBeatStartEvent => _onBeatStartEvent;
    public UnityEvent OnBeatEndEvent => _onBeatEndEvent;

    public bool IsInsideBeat => (DateTime.Now - _lastBeatTime).TotalMilliseconds < _timingWindowInMilliseconds / 2f 
        || (DateTime.Now - _lastBeatTime).TotalMilliseconds > _beatDurationInMilliseconds - _timingWindowInMilliseconds / 2f;


    private void Start()
    {
        _beatDurationInMilliseconds = 1000;
        _beatWwiseEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, BeatCallBack);
    }

    private void BeatCallBack(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        _lastBeatTime = DateTime.Now;
        if (_beatCoroutine != null)
        {
            _beatCoroutine = StartCoroutine(BeatCoroutine());
        }
        switch (in_info)
        {
            case AkMusicSyncCallbackInfo info:
                _beatDurationInMilliseconds = (int)(info.segmentInfo_fBeatDuration * 1000);
                break;
            default:
                break;
        }
        _onBeatEvent?.Invoke();
    }

    IEnumerator BeatCoroutine()
    {
        while (true)
        {
            yield return new WaitWhile(() => IsInsideBeat);
            _onBeatEndEvent?.Invoke();
            yield return new WaitUntil(() => IsInsideBeat);
            _onBeatStartEvent?.Invoke();
        }
    }
}
