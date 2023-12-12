using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Direction
{
    Right = 0,
    Left = 1,
    Down = 2,
    Up = 3,
}

public class DJController : MonoBehaviour, IIsControllable
{
    [SerializeField] List<SlotInformation> _shapesLight;
    [SerializeField, Range(0f, 1f)] float _inputDistance = .4f;
    [SerializeField, Range(2, 5)] int _quarterChecked = 4;
    [SerializeField] Direction _rightJoystickClockwise = Direction.Down;
    [SerializeField] Direction _rightJoystickAntiClockwise = Direction.Up;
    [SerializeField] Direction _leftJoystickClockwise = Direction.Right;
    [SerializeField] Direction _leftJoystickAntiClockwise = Direction.Left;

    PlayerInputController _djInputController;
    DJQTEController _djQTEController;

    RollInputChecker _rollRightJoystick;
    RollInputChecker _rollLeftJoystick;
    private bool _isInDrop = false;
    DropManager _dropManager;

    //TO CHECK
    private IEnumerator Start()
    {
        _djQTEController = GetComponent<DJQTEController>();
        UpdateLightTiles(_shapesLight);
        _dropManager = Globals.DropManager;
        SetUpEventsDrop();
        yield return new WaitUntil(()=> Players.PlayersController[(int)PlayerRole.DJ] != null);
        Players.AddListenerPlayerController(this);
        _djInputController = Players.PlayersController[(int)PlayerRole.DJ];
        SetUpInputs();
        Debug.Log("DJ Initialise");
    }

    //TO COMPLETE WITH OTHER INPUTS
    private void SetUpInputs()
    {
        _rollLeftJoystick = new RollInputChecker(_djInputController.LeftJoystick, _inputDistance, _quarterChecked);
        _rollRightJoystick = new RollInputChecker(_djInputController.RightJoystick, _inputDistance, _quarterChecked);
        _rollLeftJoystick.TurnClockWise += () => MoveLightShape(_leftJoystickClockwise);
        _rollLeftJoystick.TurnAntiClockWise += () => MoveLightShape(_leftJoystickAntiClockwise);
        _rollRightJoystick.TurnClockWise += () => MoveLightShape(_rightJoystickClockwise);
        _rollRightJoystick.TurnAntiClockWise += () => MoveLightShape(_rightJoystickAntiClockwise);
    }
    //TO COMPLETE WITH SETUPINPUTS
    private void OnDestroy()
    {
        if (_rollLeftJoystick != null)
        {
            _rollLeftJoystick.TurnClockWise -= () => MoveLightShape(_leftJoystickClockwise);
            _rollLeftJoystick.TurnAntiClockWise -= () => MoveLightShape(_leftJoystickAntiClockwise);
            _rollRightJoystick.TurnClockWise -= () => MoveLightShape(_rightJoystickClockwise);
            _rollRightJoystick.TurnAntiClockWise -= () => MoveLightShape(_rightJoystickAntiClockwise);
        }
        Players.RemoveListenerPlayerController(this);
        _dropManager.OnBeginBuildUp -= OnBeginDrop;
        _dropManager.OnDropSuccess -= OnDropEnd;
        _dropManager.OnDropFail -= OnDropEnd;
    }

    //DONE
    public void MoveLightShape(Direction direction)
    {
        if (!_isInDrop && _shapesLight.TrueForAll(x => x.Neighbours[(int)direction] != null))
        {
            List<SlotInformation> newList = new();
            _shapesLight.ForEach(x => newList.Add(x.Neighbours[(int)direction]));
            UpdateLightTiles(newList);
            _shapesLight = newList;
            
        }
    }
    
    //DONE
    private void UpdateLightTiles(List<SlotInformation> newSlots)
    {
        foreach (SlotInformation slot in _shapesLight)
        {
            if (slot.IsEnlighted)
            {
                slot.OnOccupantChanges -= DeactivateQTE;
            }
            slot.IsEnlighted = false;
            if (slot.SlotRenderer != null)
            {
                slot.SlotRenderer.ChangeColor(slot.IsEnlighted);
            } else
            {
                slot.SpriteRenderer.color = Color.green;
            }
        }
        foreach (SlotInformation slot in newSlots)
        {
            slot.OnOccupantChanges += DeactivateQTE;
            slot.IsEnlighted = true;
            if (slot.SlotRenderer != null)
            {
                slot.SlotRenderer.ChangeColor(slot.IsEnlighted);
            }
            else
            {
                slot.SpriteRenderer.color = Color.red;
            }
        }
        _djQTEController.UpdateQTE(newSlots);
    }
    private void DeactivateQTE()
    {
        _djQTEController.UpdateQTE(_shapesLight);
    }

    public void ChangeController()
    {
        _djInputController = Players.PlayersController[(int)PlayerRole.DJ];
        if (_djInputController != null)
        {
            _rollLeftJoystick = new RollInputChecker(_djInputController.LeftJoystick, _inputDistance, _quarterChecked);
            _rollRightJoystick = new RollInputChecker(_djInputController.RightJoystick, _inputDistance, _quarterChecked);
        }
    }
    private void SetUpEventsDrop()
    {
        _dropManager.OnBeginBuildUp += OnBeginDrop;
        _dropManager.OnDropSuccess += OnDropEnd;
        _dropManager.OnDropFail += OnDropEnd;
    }

    private void OnBeginDrop()
    {
        _isInDrop = true;
        _djQTEController.OnBeginDrop();
    }
    private void OnDropEnd()
    {
        _isInDrop = false;
        _djQTEController.OnDropEnd();
    }
}
