using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TransitQueue : MonoBehaviour
{
    [SerializeField] private GameObject _slot;
    public List<SlotInformation> Slots { get; private set; } = new List<SlotInformation>();

    public void AddSlot(Vector3 position)
    {
        GameObject go = Instantiate(_slot, position, quaternion.identity, transform);
        Slots.Add(go.GetComponent<SlotInformation>());
    }

    public void DeleteAll()
    {
        foreach (var v in Slots)
        {
            DestroyImmediate(v.gameObject);
        }
        Slots.Clear();
    }
    

}
