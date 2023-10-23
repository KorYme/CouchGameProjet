using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class QueueTest : MonoBehaviour
{
    [SerializeField] private int _arrayHorizontalLength;
    [SerializeField] private SlotInformation[] _slotList;
    [SerializeField] private SlotInformation[] _transitQueue;
    
    [Header("Barman")]
    [SerializeField] private Transform _circleOrigin;
    [SerializeField] private float _circleRadius;
    
    private SlotInformation[,] list;

    public SlotInformation[] Transit => _transitQueue;
    public SlotInformation[,] Bouncer => list;
    public List<CharacterStateMachine> RoamQueue { get; set; } = new List<CharacterStateMachine>();
    public Transform CircleOrigin => _circleOrigin;
    public float CircleRadius => _circleRadius;

    private void Start()
    {
        list = new SlotInformation[_arrayHorizontalLength, _slotList.Length / _arrayHorizontalLength];
        int index = 0;
        for (int i = 0; i < _arrayHorizontalLength; ++i)
        {
            for(int j = 0; j < _slotList.Length / _arrayHorizontalLength; ++j)
            {
                list[i,j] = _slotList[index];
                //list[i, j].Id = new Vector2(i, j);
                index++;
            }
        }

        for (int i = 0; i < _transitQueue.Length; ++i)
        {
            if(i >= _transitQueue.Length - 1)
                break;
            
            Transit[i].Next = Transit[i + 1];
        }

        
    }

    public SlotInformation GetSlot(Vector2 id)
    {
        if (id.x > _arrayHorizontalLength - 1 || id.x < 0 ||
            id.y > _slotList.Length / _arrayHorizontalLength - 1 || id.y < 0 || Bouncer[(int)id.x, (int)id.y].Occupant != null)
            return null;
        return Bouncer[(int)id.x, (int)id.y];
    }


}


