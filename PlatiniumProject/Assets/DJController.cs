using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class DJController : MonoBehaviour
{
    [SerializeField] List<SlotInformation> _shapesLight;
    PlayerInputController _djInputController;

    bool _areInputsSetUp = false;

    readonly Color Red = Color.red;
    readonly Color Green = new Color(0f, 1f, 1f / 18f);
    public enum Direction
    {
        Right = 0,
        Left = 1,
        Down = 2,
        Up = 3,
    }

    private IEnumerator Start()
    {
        UpdateLightTiles(_shapesLight);
        _areInputsSetUp = false;
        yield return new WaitUntil(()=> Players.PlayersController[(int)PlayerRole.Bouncer] != null);
        Debug.Log("Initialisé");
        _djInputController = Players.PlayersController[(int)PlayerRole.DJ];
        _djInputController.LeftJoystick.OnInputStart += () => Debug.Log("DEBUG");

        SetUpInputs();
    }

    private void SetUpInputs()
    {
        _djInputController.RightJoystick.OnInputStart += () =>
        {
            if (_djInputController.RightJoystick.InputValue != Vector2.zero)
            {
                MoveLightShape(GetDirectionFromVector(_djInputController.LeftJoystick.InputValue));
            }
        };
        _areInputsSetUp = true;
    }

    private void OnDestroy()
    {
        if (_areInputsSetUp)
        {

        }
    }

    Direction GetDirectionFromVector(Vector2 vector)
    {
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            if (vector.x > 0)
            {
                return Direction.Right;
            }
            else
            {
                return Direction.Left;
            }
        }
        else
        {
            if (vector.y > 0)
            {
                return Direction.Up;
            }
            else
            {
                return Direction.Down;
            }
        }
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
}
