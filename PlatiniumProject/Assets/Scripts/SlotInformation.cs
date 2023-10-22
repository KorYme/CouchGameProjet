using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotInformation : MonoBehaviour
{
    public GameObject Occupant { get; set; }
    public SlotInformation Next { get; set; }
    public Vector2 Id { get; set; }
    public SlotInformation[] Neighbours { get; set; } = new SlotInformation[4]{null, null,null, null};
}
