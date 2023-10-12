using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBounceable
{
    public bool IsBouncing { get; }
    public AnimationCurve BounceCurve { get; }
    public float BounceMultiplier { get; }
    public Vector2 BounceFactors { get; }
}
