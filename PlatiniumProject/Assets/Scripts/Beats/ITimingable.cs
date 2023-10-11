using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public interface ITimingable
{
    public bool IsInsideBeat { get; }
    public UnityEvent OnBeatEvent { get; }
    public UnityEvent OnBeatStartEvent { get; }
    public UnityEvent OnBeatEndEvent { get; }
}