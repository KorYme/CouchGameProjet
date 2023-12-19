using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightIntensityData", menuName = "ScriptableObject/LightIntensityData", order = 1)]
public class LightIntensityData : ScriptableObject
{
    public AnimationCurve dropLoopCurve;
    [Range(.1f, 10f)] public float curveMultiplier = 1f;
    [Range(.1f, 10f)] public float lightRecoveryTime = 1f;
}
