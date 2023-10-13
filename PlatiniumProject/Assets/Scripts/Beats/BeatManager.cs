using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BeatManager : MonoBehaviour, ITimingable
{
    [Header("References"),Space]
    [SerializeField] AK.Wwise.Event _beatWwiseEvent;

    [Header("Events"), Space]
    [SerializeField] UnityEvent _onBeatEvent;
    [SerializeField] UnityEvent _onBeatStartEvent;
    [SerializeField] UnityEvent _onBeatEndEvent;
    public UnityEvent OnBeatEvent => _onBeatEvent;
    public UnityEvent OnBeatStartEvent => _onBeatStartEvent;
    public UnityEvent OnBeatEndEvent => _onBeatEndEvent;

    public float TimeSinceLastBeat => Time.time - TimeOfLastBeat;

    public bool IsOnBeat => TimeSinceLastBeat < _timingWindow / 2f || TimeSinceLastBeat + (_timingWindow / 2f) >= BeatDuration;

    float _timeOfLastBeat;
    public float TimeOfLastBeat => _timeOfLastBeat;

    float _beatDuration;
    public float BeatDuration => _beatDuration;

    [Header("Parameters"), Space]
    [SerializeField, Tooltip("Sum of time before and after the beat for the input (in seconds)")] int _timingWindow;

    private void Start()
    {
        _beatDuration = 1;
        //StartCoroutine(BeatCoroutine());
        _beatWwiseEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, BeatCallBack);
    }

    private void BeatCallBack(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        switch (in_info)
        {
            case AkMusicSyncCallbackInfo info:
                _beatDuration = info.segmentInfo_fBeatDuration;
                break;
            default:
                break;
        }
        _onBeatEvent?.Invoke();
    }

    //IEnumerator BeatCoroutine()
    //{
    //    while (true)
    //    {
    //        _timerInMilliseconds += 20;
    //        if (IsInsideBeat)
    //        {
    //            if (_timerInMilliseconds >= _timingWindowInMilliseconds / 2 && _beatDurationInMilliseconds - _timerInMilliseconds > _timingWindowInMilliseconds / 2)
    //            {
    //                IsInsideBeat = false;
    //                _onBeatEndEvent?.Invoke();
    //            }
    //        }
    //        else
    //        {
    //            if (_beatDurationInMilliseconds - _timerInMilliseconds <= _timingWindowInMilliseconds / 2)
    //            {
    //                IsInsideBeat = true;
    //                _onBeatStartEvent?.Invoke();
    //            }
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    IEnumerator EventBeatCoroutine()
    {
        while (true)
        {
            yield return new WaitWhile(() => !IsOnBeat);
            OnBeatEndEvent?.Invoke();
            yield return new WaitWhile(() => IsOnBeat);
            OnBeatStartEvent?.Invoke();
        }
        yield return null;
    }
}
