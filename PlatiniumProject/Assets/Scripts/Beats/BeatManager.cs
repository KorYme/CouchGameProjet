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

    [Header("Parameters"), Space]
    [SerializeField] int _timingWindowInMilliseconds;

    int _beatDurationInMilliseconds;
    int _timerInMilliseconds;

    public bool IsInsideBeat { get; private set; }


    private void Start()
    {
        _beatDurationInMilliseconds = 1000;
        _timerInMilliseconds = 0;
        IsInsideBeat = true;
        StartCoroutine(BeatCoroutine());
        _beatWwiseEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, BeatCallBack);
    }

    private void BeatCallBack(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        _timerInMilliseconds = 0;
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
            _timerInMilliseconds += 20;
            if (IsInsideBeat)
            {
                if (_timerInMilliseconds >= _timingWindowInMilliseconds / 2 && _beatDurationInMilliseconds - _timerInMilliseconds > _timingWindowInMilliseconds / 2)
                {
                    IsInsideBeat = false;
                    _onBeatEndEvent?.Invoke();
                }
            }
            else
            {
                if (_beatDurationInMilliseconds - _timerInMilliseconds <= _timingWindowInMilliseconds / 2)
                {
                    IsInsideBeat = true;
                    _onBeatStartEvent?.Invoke();
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
