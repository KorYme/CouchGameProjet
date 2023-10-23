using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BouncerMovement : EntityMovement
{
    [SerializeField] private AreaManager _areaManager;
    [SerializeField] private BeatManager _beatManager;

    private IMovable _movement;

    private SlotInformation _currentSlot;

    private void Start()
    {
        _movement = GetComponent<IMovable>();
        _currentSlot = _areaManager.BouncerBoard.Board[_areaManager.BouncerBoard.BoardDimension.x *Mathf.Max(1,_areaManager.BouncerBoard.BoardDimension.y / 2 + _areaManager.BouncerBoard.BoardDimension.y % 2) -1];
        transform.position = _currentSlot.transform.position;
    }

    private void Update()
    {
        if(Input.inputString == "" || _movement.IsMoving)
            return;

        var keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), Input.inputString.ToUpper());
        switch (keyCode)
        {
            case KeyCode.Z:
                if (_currentSlot.Neighbours[3] != null)
                {
                    _currentSlot.Occupant = null;
                    _movement.MoveToPosition(_currentSlot.Neighbours[3].transform.position);
                    _currentSlot = _currentSlot.Neighbours[3];
                    _currentSlot.Occupant = gameObject;
                }
                break;
            case KeyCode.S:
                if (_currentSlot.Neighbours[2] != null)
                {
                    _currentSlot.Occupant = null;
                    _movement.MoveToPosition(_currentSlot.Neighbours[2].transform.position);
                    _currentSlot = _currentSlot.Neighbours[2];
                    _currentSlot.Occupant = gameObject;
                }
                break;
            case KeyCode.Q:
                if (_currentSlot.Neighbours[1] != null)
                {
                    _currentSlot.Occupant = null;
                    _movement.MoveToPosition(_currentSlot.Neighbours[1].transform.position);
                    _currentSlot = _currentSlot.Neighbours[1];
                    _currentSlot.Occupant = gameObject;
                }
                break;
            case KeyCode.D:
                if (_currentSlot.Neighbours[0] != null)
                {
                    _currentSlot.Occupant = null;
                    _movement.MoveToPosition(_currentSlot.Neighbours[0].transform.position);
                    _currentSlot = _currentSlot.Neighbours[0];
                    _currentSlot.Occupant = gameObject;
                }
                break;
        }
    }
}
