using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MovementData", menuName ="ScriptableObject/MovementData", order = 1)]
public class MovementData : ScriptableObject
{
    [Header("Movement Parameters")]
    public float MovementDurationPercent = .2f;
    [Tooltip("This curve represents the movement of the objects on which it is applied\nIt needs to start at (0,0) and ends at (1,0)")] public AnimationCurve MovementCurve = new AnimationCurve(new(0f, 0f), new(1f, 1f));

    [Header("Bounce Parameters")]
    [Tooltip("This curve needs to start at (0,0) and ends at (1,0)")] public AnimationCurve BounceCurve = new AnimationCurve(new(0f, 0f), new(0.5f, 1f), new(1f, 0f));
    [Range(-1f,10f)] public float BounceMultiplierX = 1;
    [Range(-1f,10f)] public float BounceMultiplierY = 1;
}