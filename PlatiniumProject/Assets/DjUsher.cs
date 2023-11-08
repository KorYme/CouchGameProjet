using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class DjUsher : MonoBehaviour
{
    [SerializeField] private CheckerBoard _dancefloor;
    public SlotInformation NextSlot { get; private set; }

    public void SetNextSlot()
    {
        if (NextSlot != null)
        {
            NextSlot.SpriteRenderer.color = Color.white;
            NextSlot.SlotRenderer._useShader = true;
        }
        NextSlot = _dancefloor.GetRandomAvailableSlot();
        NextSlot.SpriteRenderer.color = Color.black;
        NextSlot.SlotRenderer._useShader = false;
        EditorGUIUtility.PingObject(NextSlot);
    }
}
