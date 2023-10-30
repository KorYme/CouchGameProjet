using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TransitQueue : MonoBehaviour
{
    [SerializeField] private GameObject _slot;
    public List<SlotInformation> Slots  = new List<SlotInformation>();

#if UNITY_EDITOR
    public void AddSlot(Vector3 position)
    {
        GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(_slot) as GameObject;
        go.transform.position = new Vector3(position.x, position.y, 0);
        go.transform.rotation = Quaternion.identity;
        go.transform.parent = transform;
        Slots.Add(go.GetComponent<SlotInformation>());
    }
#endif
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

    public SlotInformation GetRandomSlotInQueue()
    {
        List<SlotInformation> availableSlots = new List<SlotInformation>();
        foreach (var s in Slots)
        {
            if(s.Occupant == null)
                availableSlots.Add(s);
        }

        if (availableSlots.Count <= 0)
            return null;

        return availableSlots[Random.Range(0, availableSlots.Count)];
    }
}
