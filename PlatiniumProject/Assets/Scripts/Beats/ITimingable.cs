using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public interface ITimingable
{
    public float BeatDuration { get; }
    public float TimeOfLastBeat { get; }
    public float TimeSinceLastBeat { get; }
    public bool IsOnBeat { get; }
    public UnityEvent OnBeatEvent { get; }
    public UnityEvent OnBeatStartEvent { get; }
    public UnityEvent OnBeatEndEvent { get; }
}