using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum CLIENT_TYPE
{
    PUNK,
    DAD,
    DISCOMOM,
    PRIEST,
}

[CreateAssetMenu()]
public class CharacterData : ScriptableObject
{
    public CLIENT_TYPE clienType;
    
    [Header("Bouncer")]
    public int movementAmountInQueue;
    public int beatAmountUnitlAction;

    [FormerlySerializedAs("maxSatisafactionBar")]
    [Header("BarMan")] 
    [Tooltip("Currently decrementing on beat")]
    public int maxBarManSatisafactionBar;
    public int decrementationValueOnBarMan;

    [Header("DJ")]
    [Tooltip("Max value of satisfaction when the character is on the dancefloor")]
    public int maxSatisafactionDJ;
    public int decrementationValueOnFloor;
    public int incrementationValueOnFloor;
}
