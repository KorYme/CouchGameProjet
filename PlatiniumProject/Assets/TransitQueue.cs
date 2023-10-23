using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TransitQueue : MonoBehaviour
{
    [SerializeField] private GameObject _slot;
    public List<SlotInformation> Slots  = new List<SlotInformation>();

    public void AddSlot(Vector3 position)
    {
        GameObject go = Instantiate(_slot, new Vector3(position.x, position.y, 0), quaternion.identity, transform);
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

    private void Awake()
    {
        for(int i = 0; i < Slots.Count - 1; i++)
        {
            Slots[i].Next = Slots[i + 1];
        }
    }


}
