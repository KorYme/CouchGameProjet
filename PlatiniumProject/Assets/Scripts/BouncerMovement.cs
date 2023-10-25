using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BouncerMovement : PlayerMovement
{
    public enum BouncerState
    {
        Moving,
        Checking
    }
    [SerializeField] private AreaManager _areaManager;
    [SerializeField] private BeatManager _beatManager;

    private IMovable _movement;
    private BouncerState currentState = BouncerState.Moving;

    private SlotInformation _currentSlot;

    protected override void Start()
    {
        base.Start();
        _movement = GetComponent<IMovable>();
        _currentSlot = _areaManager.BouncerBoard.Board[_areaManager.BouncerBoard.BoardDimension.x * Mathf.Max(1,_areaManager.BouncerBoard.BoardDimension.y / 2 + _areaManager.BouncerBoard.BoardDimension.y % 2) -1];
        transform.position = _currentSlot.transform.position;
    }

    private void Update()
    {
        if(Input.inputString == "" || _movement.IsMoving || currentState == BouncerState.Checking)
            return;

        var keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), Input.inputString.ToUpper());
        switch (keyCode)
        {
            case KeyCode.Z:
                if (_currentSlot.Neighbours[3] != null)
                {
                    Move(3);
                }
                break;
            case KeyCode.S:
                if (_currentSlot.Neighbours[2] != null)
                {
                    Move(2);
                }
                break;
            case KeyCode.Q:
                if (_currentSlot.Neighbours[1] != null)
                {
                    Move(1);
                }
                break;
            case KeyCode.D:
                if (_currentSlot.Neighbours[0] != null)
                {
                    Move(0);
                }
                break;
        }
    }

    public void Move(int index)
    {
        _currentSlot.PlayerOccupant = null;
        _movement.MoveToPosition(_currentSlot.Neighbours[index].transform.position);
        _currentSlot = _currentSlot.Neighbours[index];
        _currentSlot.PlayerOccupant = this;

        if (_currentSlot.Occupant != null)
        {
            currentState = BouncerState.Checking;
            _currentSlot.Occupant.ChangeState(_currentSlot.Occupant.BouncerCheckState);
        }
    }
}
