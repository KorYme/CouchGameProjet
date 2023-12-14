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
    [SerializeField] private Sprite[] _slotsSprites;
    public Vector2Int BoardDimension => _boardDimension;
    public int BoardLength => _boardDimension.x * _boardDimension.y;
    int[] Directions => new int[4] { 1, -1, _boardDimension.x, -_boardDimension.x };
    public float HorizontalSpacing => _horizontalSpacing;

    public Vector3 Center
    {
        get
        {
            return transform.position +
                   new Vector3(_horizontalSpacing * _boardDimension.x / 2f, -(_verticalSpacing * _boardDimension.y / 2f));
        }
    }
    public List <SlotInformation> AvailableSlots { get; private set; } = new List<SlotInformation>();
    public List<SlotInformation> EntrySlots { get; private set; } = new List<SlotInformation>();
    
    public List<SlotInformation> Board;

    private void Awake()
    {
        SetAvailableSlots();
        SetEnterySlots();
    }

    private void SetEnterySlots()
    {
        for (int i = 0; i < BoardLength; ++i)
        {
            EntrySlots.Add(Board[i]);
        }
    }

    public bool AreAnyEntryFree()
    {
        if (EntrySlots.TrueForAll(x => x.Occupant != null && x.PlayerOccupant != null))
            return false;
        return true;
    }

    public SlotInformation GetFreeEntrySlot()
    {
        List<SlotInformation> result = new List<SlotInformation>();
        EntrySlots.ForEach(x =>
        {
            if (x.Occupant == null && x.PlayerOccupant == null)
            {
                result.Add(x);
            }
        });
        return result[Random.Range(0, result.Count)];
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

    public void RandomizeTilesSprites()
    {
        List<int> ids = ShuffleSprite();
        int index = 0;
        for (int i = 0; i < Board.Count; ++i)
        {
            if (ids.Count <= 0)
                ids = ShuffleSprite();
            
            index = ids[Random.Range(0, ids.Count)];
            ids.Remove(index);
            Board[i].GetComponent<SpriteRenderer>().sprite = _slotsSprites[index];
        }
    }

    private List<int> ShuffleSprite()
    {
        List<int> ids = new List<int>();
        List<int> result = new List<int>();
        
        for (int i = 0; i < _slotsSprites.Length; ++i)
        {
            ids.Add(i);
        }
        
        for (int i = 0; i < ids.Count; ++i)
        {
            int index = Random.Range(0, ids.Count);
            result.Add(ids[index]);
            ids.Remove(index);
        }
        return result;
    }
}
