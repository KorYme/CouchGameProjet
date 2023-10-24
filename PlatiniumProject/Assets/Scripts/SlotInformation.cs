using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotInformation : MonoBehaviour
{
    public CharacterStateMachine Occupant { get; set; }
    public BouncerMovement PlayerOccupant { get; set; }
    public SlotInformation Next { get; set; }
    public int Id { get; set; }
    public SlotInformation[] Neighbours = new SlotInformation[4]{null, null,null, null};
}
