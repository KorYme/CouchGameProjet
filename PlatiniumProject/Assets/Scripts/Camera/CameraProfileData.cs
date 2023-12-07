using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CameraProfileData : ScriptableObject
{

    [Header("Shake value")] 
    public float shakeDuration;
    public float shakeIntensity;
    public float shakeSpeed;

    [Header("Focus value")] 
    public Vector3 focusOffSet;
    public float focusDuration;
    public float focusPercentage;
    public float snapDuration;
    public AnimationCurve snapCurve;

    [Header("Camera pulse value")] 
    public float pulsePercentage;
    public AnimationCurve pulseCurve;
}
