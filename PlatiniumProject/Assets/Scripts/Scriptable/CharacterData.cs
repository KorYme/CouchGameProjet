using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CharacterData : ScriptableObject
{
    public float movementDuration;
    public AnimationCurve movementCurve;
}
