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
    private PlayerInputController _inputController;

    private IMovable _movement;
    private BouncerState currentState = BouncerState.Moving;

    private SlotInformation _currentSlot;
    [SerializeField, Range(0f, 1f)] private float _inputDistance;

    protected IEnumerator Start()
    {
        yield return new WaitUntil(() => Players.PlayersController[(int)PlayerRole.Bouncer] != null);
        _inputController = Players.PlayersController[(int)PlayerRole.Bouncer];

        _movement = GetComponent<IMovable>();
        _currentSlot = _areaManager.BouncerBoard.Board[_areaManager.BouncerBoard.BoardDimension.x
            * Mathf.Max(1,_areaManager.BouncerBoard.BoardDimension.y / 2 + _areaManager.BouncerBoard.BoardDimension.y % 2) -1];
        _currentSlot.PlayerOccupant = this;
        transform.position = _currentSlot.transform.position;
        _inputController.LeftJoystick.OnInputChange += OnInputMove;
        Debug.Log("Bouncer Initialisé");
    }

    private void OnInputMove()
    {
        Vector2 dir = GetClosestUnitVectorFromVector(_inputController.LeftJoystick.InputValue);
        if (_inputController != null && dir != Vector2.zero)
        {
            Move((int)GetClosestDirectionFromVector(dir));
        }
    }

    public void Move(int index)
    {
        if (_currentSlot.Neighbours[index] == null)
            return;

        if (_currentSlot.Neighbours[index].Occupant != null)
        {
            if (_movement.MoveToPosition(_currentSlot.Neighbours[index].transform.position + new Vector3(_areaManager.BouncerBoard.HorizontalSpacing / 2, 0, 0)))
            {
                currentState = BouncerState.Checking;
                _currentSlot.Neighbours[index].Occupant.ChangeState(_currentSlot.Neighbours[index].Occupant.BouncerCheckState);

                _currentSlot.PlayerOccupant = null;
                _currentSlot = _currentSlot.Neighbours[index];
                _currentSlot.PlayerOccupant = this;
                StartCoroutine(TestCheck());
            }
        }
        else
        {
            if (_movement.MoveToPosition(_currentSlot.Neighbours[index].transform.position))
            {
                _currentSlot.PlayerOccupant = null;
                _currentSlot = _currentSlot.Neighbours[index];
                _currentSlot.PlayerOccupant = this;
            }
        }

    }
    private Vector2 GetClosestUnitVectorFromVector(Vector2 vector)
    {
        if (vector.magnitude < _inputDistance) return Vector2.zero;
        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            return new Vector2(Mathf.Sign(vector.x), 0f);
        }
        else
        {
            return new Vector2(0f, Mathf.Sign(vector.y));
        }
    }
    private Direction GetClosestDirectionFromVector(Vector2 vector)
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


    IEnumerator TestCheck()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                CharacterCheckByBouncerState chara = _currentSlot.Occupant.CurrentState as CharacterCheckByBouncerState;
                chara.BouncerAction(true);
                currentState = BouncerState.Moving;
                transform.position = _currentSlot.transform.position;
                yield break;
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                CharacterCheckByBouncerState chara = _currentSlot.Occupant.CurrentState as CharacterCheckByBouncerState;
                chara.BouncerAction(false);
                currentState = BouncerState.Moving;
                transform.position = _currentSlot.transform.position;
                yield break;
            }
            yield return null;
        }
    }
}
