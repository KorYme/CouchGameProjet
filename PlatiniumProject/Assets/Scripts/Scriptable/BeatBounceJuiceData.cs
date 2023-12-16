using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BeatBounceJuiceData : ScriptableObject
{
    [System.Serializable]
    public struct PhaseData
    {
        public bool usedThisPhase;
        [Range(0f, 2f)]
        public float maxScale;
        [Range(0f, 1f)] 
        public float beatPercentage;
    }

    public AnimationCurve bounceCurve;
    public PhaseData[] phaseData;

}
