using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class DJController : MonoBehaviour
{
    [SerializeField] List<SlotInformation> _shapesLight;

    readonly Color Red = Color.red;
    readonly Color Green = new Color(0f, 1f, 1f / 18f);

    public enum Direction
    {
        Right = 0,
        Left = 1,
        Down = 2,
        Up = 3,
    }

    private void Start()
    {
        UpdateLightTiles(_shapesLight);
    }

    public void MoveLightShape(Direction direction)
    {
        if (_shapesLight.TrueForAll(x => x.Neighbours[(int)direction] != null))
        {
            List<SlotInformation> newList = new();
            _shapesLight.ForEach(x => newList.Add(x.Neighbours[(int)direction]));
            UpdateLightTiles(newList);
            _shapesLight = newList;
        }
    }

    private void UpdateLightTiles(List<SlotInformation> newSlots)
    {
        foreach (SlotInformation slot in _shapesLight)
        {
            slot.GetComponent<SpriteRenderer>().color = Green;
            slot.IsEnlighted = false;
        }
        foreach (SlotInformation slot in newSlots)
        {
            slot.IsEnlighted = true;
            slot.GetComponent<SpriteRenderer>().color = Red;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLightShape(Direction.Right);
        }        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLightShape(Direction.Left);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveLightShape(Direction.Up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveLightShape(Direction.Down);
        }
    }
}
