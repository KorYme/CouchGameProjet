using System;
using UnityEngine;

public class SlotInformation : MonoBehaviour
{
    private CharacterStateMachine _occupant;
    public CharacterStateMachine Occupant {
        get {
            return _occupant;
        }
        set { 
            if (_occupant != value)
            {
                _occupant = value;
                OnOccupantChanges?.Invoke();
            }
        } }
    public BouncerMovement PlayerOccupant { get; set; }
    public SlotInformation Next { get; set; }
    public int Id { get; set; }
    public SlotInformation[] Neighbours = new SlotInformation[4] {null, null,null, null};

    public bool IsEnlighted { get; set; } = false;

    public event Action OnOccupantChanges;
}