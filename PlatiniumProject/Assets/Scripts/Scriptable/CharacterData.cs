using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CharacterData : ScriptableObject
{
    [Header("Movement")]
    public float movementDuration;
    public AnimationCurve movementCurve;

    [Header("Bouncer")]
    public int movementAmountInQueue;
    public int beatAmountUnitlAction;
}
