using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class QueueTest : MonoBehaviour
{
    [SerializeField] private int _arrayHorizontalLength;
    [SerializeField] private SlotInformation[] _slotList;
    [SerializeField] private SlotInformation[] _transitQueue;
    private SlotInformation[,] list;

    public SlotInformation[] Transit => _transitQueue;
    public SlotInformation[,] Bouncer => list;

    private void Start()
    {
        list = new SlotInformation[_arrayHorizontalLength, _slotList.Length / _arrayHorizontalLength];
        int index = 0;
        for (int i = 0; i < _arrayHorizontalLength; ++i)
        {
            for(int j = 0; j < _slotList.Length / _arrayHorizontalLength; ++j)
            {
                list[i,j] = _slotList[index];
                index++;
            }
        }

        for (int i = 0; i < _transitQueue.Length; ++i)
        {
            if(i >= _transitQueue.Length - 2)
                return;
            
            Transit[i].Next = Transit[i + 1];
        }
        
    }
}


