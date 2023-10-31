using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckerBoard : MonoBehaviour
{
    
    [SerializeField] protected Vector2Int _boardDimension;
    [SerializeField] protected float _horizontalSpacing;
    [SerializeField] protected float _verticalSpacing;
    [SerializeField] protected GameObject _slot;
    [SerializeField] private bool _useShader = false;
    public Vector2Int BoardDimension => _boardDimension;
    public int BoardLength => _boardDimension.x * _boardDimension.y;
    int[] Directions => new int[4] { 1, -1, _boardDimension.x, -_boardDimension.x };
    public float HorizontalSpacing => _horizontalSpacing;
    public List <SlotInformation> AvailableSlots { get; private set; } = new List<SlotInformation>();


    public List<SlotInformation> Board;

    private void Awake()
    {
        SetAvailableSlots();
    }

    public void Delete()
    {
        Transform[] gos = GetComponentsInChildren<Transform>();
        foreach (var v in gos)
        {
            if (v != this.transform)
            {
                DestroyImmediate(v.gameObject);
            }
        }
        Board?.Clear();
        Board = new List<SlotInformation>();
    }

#if UNITY_EDITOR
    public void UpdateData()
    {
        Delete();   
        
        int j = 0;
        for (int i = 0; i < BoardLength; ++i)
        {
            if (i % _boardDimension.x == 0)
            {
                j++;
            }
            GameObject go = UnityEditor.PrefabUtility.InstantiatePrefab(_slot) as GameObject;
            go.transform.position = transform.position + new Vector3(_horizontalSpacing * (i % _boardDimension.x),
                    _verticalSpacing * -(j - 1 % _boardDimension.y), 0);
            go.transform.rotation = Quaternion.identity;
            go.transform.parent = transform;
            //Setup renderer

            SlotRenderer renderer = go.GetComponent<SlotRenderer>();
            renderer.SetUpMaterialDancefloor(i % _boardDimension.x, j - 1 % _boardDimension.y,_useShader);

            Board.Add(go.GetComponent<SlotInformation>());
        }

        RenameSlots();
        SetNeighbours();
    }
#endif

    private void SetNeighbours()
    {
        for (int i = 0; i < BoardLength; ++i)
        {
            SlotInformation slot = Board[i];

            for (int k = 0; k < Directions.Length; k++)
            {
                slot.Neighbours[k] = null;
                if (i + Directions[k] <= BoardLength - 1 && i + Directions[k] >= 0)
                {
                    slot.Neighbours[k] = Board[i + Directions[k]];
                }
            }
            //Debug.Log((i + 1)%_boardDimension.x);   
            if ((i + 1) % _boardDimension.x == 1)
                slot.Neighbours[1] = null;
            else if ((i + 1) % _boardDimension.x == 0)
                slot.Neighbours[0] = null;
        }
    }


    public SlotInformation GetNextBouncerSlot(SlotInformation currentSlot)
    {
        List<SlotInformation> availableSlot = new List<SlotInformation>();
        foreach(SlotInformation slot in currentSlot.Neighbours)
        {
            if(slot != null && slot.Occupant == null && slot.PlayerOccupant == null)
            {
                availableSlot.Add(slot);
            }
        }

        if (availableSlot.Count <= 0) return null;

        return availableSlot[Random.Range(0, availableSlot.Count)];
    }

    public void SetAvailableSlots()
    {
        foreach (var v in Board)
        {
            if (v.Occupant == null)
            {
                AvailableSlots.Add(v);
            }
        }
    }

    public void AddAvailableSlot(SlotInformation slot)
    {
        AvailableSlots.Add(slot);
    }
    
    public SlotInformation GetRandomAvailableSlot()
    {
        if (AvailableSlots.Count <= 0)
            return null;
        
        SlotInformation result = AvailableSlots[Random.Range(0, AvailableSlots.Count)];
        AvailableSlots.Remove(result);
        return result;
    }

    public void RenameSlots()
    {
        int j = 0;
        for (int i = 0; i < BoardLength; ++i)
        {
            if (i % _boardDimension.x == 0)
            {
                j++;
            }
            SlotInformation slot = Board[i];
            string name = "";
            gameObject.name.ToList().ForEach( x=> {
                if (x == char.ToUpper(x))
                {
                    name += x;
                } });
            slot.gameObject.name = $"{name}: X: {i % _boardDimension.x}, Y: {j - 1}";

        }
            
    }
}
