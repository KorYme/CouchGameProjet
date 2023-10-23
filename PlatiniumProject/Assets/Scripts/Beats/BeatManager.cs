using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour, ITimingable
{
    #region FIELDS
    [Header("References"), Space]
    [SerializeField] AK.Wwise.Event _beatWwiseEvent;


    [Header("Parameters"), Space]
    [SerializeField, Range(0f, .5f), Tooltip("Timing window before the beat which allows input")]
    float _timingWindowPercentBeforeBeat = .1f;

    [SerializeField, Range(0f, .5f), Tooltip("Timing window after the beat which allows input")]
    float _timingWindowPercentAfterBeat = .3f;


    [Header("Events"), Space]
    [SerializeField, Tooltip("This event is called exactly on the thiming of the beat")] 
    UnityEvent _onBeatEvent;
    [SerializeField, Tooltip("This event is called on the first frame an input can be received")] 
    UnityEvent _onBeatStartEvent;
    [SerializeField, Tooltip("This event is called on the first frame an input cannot be received anymore")] 
    UnityEvent _onBeatEndEvent;


    int _beatDurationInMilliseconds;
    DateTime _lastBeatTime;
    Coroutine _beatCoroutine;
    #endregion

    #region PROPERTIES
    public bool IsInsideBeat => (DateTime.Now - _lastBeatTime).TotalMilliseconds < (_timingWindowPercentAfterBeat * _beatDurationInMilliseconds)
        || (DateTime.Now - _lastBeatTime).TotalMilliseconds > _beatDurationInMilliseconds - (_timingWindowPercentBeforeBeat * _beatDurationInMilliseconds); // Modulo ?
    public UnityEvent OnBeatEvent => _onBeatEvent;
    public UnityEvent OnBeatStartEvent => _onBeatStartEvent;
    public UnityEvent OnBeatEndEvent => _onBeatEndEvent;
    public int BeatDurationInMilliseconds => _beatDurationInMilliseconds;
    #endregion

    #region PROCEDURES
    private IEnumerator Start()
    {
        _beatDurationInMilliseconds = 1000;
        yield return null;
        _beatWwiseEvent.Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, BeatCallBack);
    }

    private void BeatCallBack(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        _lastBeatTime = DateTime.Now;
        if (_beatCoroutine == null)
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
            yield return new WaitWhile(() => !IsInsideBeat);
            _onBeatStartEvent?.Invoke();
        }
    }
    #endregion
}
