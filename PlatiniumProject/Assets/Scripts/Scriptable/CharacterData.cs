using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class CharacterData : ScriptableObject
{
    public int beatAmountUnitlAction;
    
    [Header("Bouncer")]
    public int movementAmountInQueue;

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
