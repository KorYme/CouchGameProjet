using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MovementData", menuName ="ScriptableObject/MovementData", order = 1)]
public class MovementData : ScriptableObject
{
    [Header("Movement Parameters")]
    public float MovementDuration = .2f;
    public int SpeedMultiplier = 1;
    public AnimationCurve MovementCurve = new AnimationCurve(new(0f, 0f), new(1f, 1f));

    [Header("Bounce Parameters")]
    public AnimationCurve BounceCurve = new AnimationCurve(new(0f, 0f), new(0.5f, 1f), new(1f, 0f));
    public float BounceMultiplier = 1;
    public Vector2 BounceFactors = new Vector2(2, -0.5f);
}
