using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckerBoard : MonoBehaviour
{
    
    [SerializeField] protected Vector2Int _boardDimension;
    [SerializeField] protected float _horizontalSpacing;
    [SerializeField] protected float _verticalSpacing;
    [SerializeField] protected GameObject _slot;

    public Vector2Int BoardDimension => _boardDimension;
    public int BoardLength => _boardDimension.x * _boardDimension.y;
    int[] Directions => new int[4] { 1, -1, _boardDimension.x, -_boardDimension.x };


    public List<SlotInformation> Board;

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
        Debug.Log("dsfsfsdf");
        Board?.Clear();
        Board = new List<SlotInformation>();
    }

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
            GameObject go = Instantiate(_slot,
                transform.position + new Vector3(_horizontalSpacing * (i % _boardDimension.x),
                    _verticalSpacing * -(j - 1 % _boardDimension.y), 0), quaternion.identity, transform);
            
            Board.Add(go.GetComponent<SlotInformation>());
        }
        
        SetNeighbours();
    }

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
            if(slot != null && slot.Occupant == null)
            {
                availableSlot.Add(slot);
            }
        }

        if (availableSlot.Count <= 0) return null;

        return availableSlot[Random.Range(0, availableSlot.Count)];
    }
}
