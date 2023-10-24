using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CharacterData : ScriptableObject
{
    [Header("Bouncer")]
    public int movementAmountInQueue;
    public int beatAmountUnitlAction;

    [Header("BarMan")] 
    [Tooltip("Currently decrementing on beat")]
    public int maxSatisafactionBar;

}
