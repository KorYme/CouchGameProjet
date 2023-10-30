using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour, ITimingable
{
    #region FIELDS
    [Header("References"), Space]
    [SerializeField, Tooltip("Wwise play event to launch the sound and the beat\nDon't need to be modified by GDs")] 
    List<AK.Wwise.Event> _beatWwiseEvent;

    [SerializeField, Range(0,2)]
    int _musicIndex;

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

    int _beatDurationInMilliseconds;
    DateTime _lastBeatTime;
    Coroutine _beatCoroutine;
    #endregion

    #region PROPERTIES
    public int BeatDurationInMilliseconds => _beatDurationInMilliseconds;
    public UnityEvent OnBeatEvent => _onBeatEvent;
    public UnityEvent OnBeatStartEvent => _onBeatStartEvent;
    public UnityEvent OnBeatEndEvent => _onBeatEndEvent;

    public bool IsInsideBeat => IsInBeatWindowBefore || IsInBeatWindowAfter;
    public bool IsInBeatWindowBefore => (DateTime.Now - _lastBeatTime).TotalMilliseconds < (_timingAfterBeat * _beatDurationInMilliseconds);
    public bool IsInBeatWindowAfter => (DateTime.Now - _lastBeatTime).TotalMilliseconds > _beatDurationInMilliseconds - (_timingBeforeBeat * _beatDurationInMilliseconds);
    #endregion

    #region PROCEDURES
    private void Awake()
    {
        if (Globals.BeatTimer != null)
        {
            Destroy(gameObject);
            return;
        }
        Globals.BeatTimer = this;
    }

    private IEnumerator Start()
    {
        _beatDurationInMilliseconds = 1000;
        yield return null;
        _beatWwiseEvent[_musicIndex].Post(gameObject, (uint)AkCallbackType.AK_MusicSyncBeat, BeatCallBack);
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
        OnBeatEvent?.Invoke();
    }

    IEnumerator BeatCoroutine()
    {
        while (true)
        {
            yield return new WaitWhile(() => IsInsideBeat);
            OnBeatEndEvent?.Invoke();
            yield return new WaitUntil(() => IsInsideBeat);
            OnBeatStartEvent?.Invoke();
        }
    }
    #endregion
}
