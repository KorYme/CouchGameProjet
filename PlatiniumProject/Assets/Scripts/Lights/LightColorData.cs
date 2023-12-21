using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightColorData", menuName = "ScriptableObject/LightColorData", order = 1)]
public class LightColorData : ScriptableObject
{
    [Header("Basic Behaviour")]
    public List<Color> onBeatColorList;

    [Header("Drop Failed Behaviour")]
    public Color onDropFailedColor;
    
    [Header("Drop Success Behaviour")]
    public Color onDropSuccessColor;
}
