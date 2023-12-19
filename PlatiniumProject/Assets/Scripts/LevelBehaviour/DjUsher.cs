using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class DjUsher : MonoBehaviour
{
    [SerializeField] private CheckerBoard _dancefloor;
    [SerializeField] private Sprite _selectedSlot;
    public SlotInformation NextSlot { get; private set; }

    public void SetNextSlot()
    {
        if (NextSlot != null)
        {
            NextSlot.SpriteRenderer.color = Color.white;
            NextSlot.SlotRenderer._useShader = true;
            NextSlot.SpriteRenderer.sprite = NextSlot.BaseSprite;
        }
        NextSlot = _dancefloor.GetRandomAvailableSlot();
        NextSlot.SpriteRenderer.color = Color.black;
        NextSlot.SpriteRenderer.sprite = _selectedSlot;
        NextSlot.SlotRenderer._useShader = false;
        //EditorGUIUtility.PingObject(NextSlot);
    }
}
