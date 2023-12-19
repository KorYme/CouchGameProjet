using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightColorData", menuName = "ScriptableObject/LightColorData", order = 1)]
public class LightColorData : ScriptableObject
{
    public List<Color> onBeatColorList;
    public Color onDropFailedColor;
    public Color onDropSuccessColor;
}
