using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public interface ITimingable
{
    public bool IsPlaying { get; }
    public bool IsInsideBeatWindow => IsInBeatWindowBefore || IsInBeatWindowAfter;
    public bool IsInBeatWindowBefore { get; }
    public bool IsInBeatWindowAfter { get; }
    public int BeatDurationInMilliseconds { get; }
    public double BeatDeltaTimeInMilliseconds { get; }
    public float BeatDurationInSeconds { get; }
    public double BeatDeltaTimeInSeconds { get; }
    public UnityEvent OnBeatEvent { get; }
    public UnityEvent OnBeatStartEvent { get; }
    public UnityEvent OnBeatEndEvent { get; }
    public event Action OnNextBeatStart;
    public event Action OnNextBeat;
    public event Action OnNextBeatEnd;
}