using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RumbleValues : ScriptableObject
{
    public string rumbleName;
    public bool isHolding;
    [Range(.1f, 10f)] public float time;
    public AnimationCurve rumbleCurve;
}