using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BouncerMovement : PlayerMovement, IQTEable
{
    public enum BouncerState
    {
        Moving,
        Checking
    }

    [Space, Header("Bouncer Parameters")]
    [SerializeField] private AreaManager _areaManager;

    private BouncerState currentState = BouncerState.Moving;

    private SlotInformation _currentSlot;

    protected override PlayerRole _playerRole => PlayerRole.Bouncer;

    QTEHandler _qteHandler;
    [SerializeField] TextMeshProUGUI _text;

    protected override IEnumerator Start()
    {
        _text.text = "";
        yield return base.Start();

        _currentSlot = _areaManager.BouncerBoard.Board[_areaManager.BouncerBoard.BoardDimension.x
            * Mathf.Max(1,_areaManager.BouncerBoard.BoardDimension.y / 2 + _areaManager.BouncerBoard.BoardDimension.y % 2) -1];
        _currentSlot.PlayerOccupant = this;
        transform.position = _currentSlot.transform.position;
        _qteHandler = GetComponent<QTEHandler>();
        if (_qteHandler != null)
        {
            _qteHandler.RegisterQTEable(this);
        }
        Debug.Log("Bouncer Initialis�");
    }

    protected override void OnInputMove(Vector2 vector)
    {
        if (currentState == BouncerState.Moving)
        {
            Move((int)GetClosestDirectionFromVector(vector));
        }
    }

    public void Move(int index)
    {
        if (_currentSlot.Neighbours[index] == null)
            return;

        if (_currentSlot.Neighbours[index].Occupant != null)
        {
            if (MoveToPosition(_currentSlot.Neighbours[index].transform.position + new Vector3(_areaManager.BouncerBoard.HorizontalSpacing / 2, 0, 0)))
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
            if (MoveToPosition(_currentSlot.Neighbours[index].transform.position))
            {
                _currentSlot.PlayerOccupant = null;
                _currentSlot = _currentSlot.Neighbours[index];
                _currentSlot.PlayerOccupant = this;
            }
        }
    }

    private Vector2 GetClosestUnitVectorFromVector(Vector2 vector)
    {
        if (vector.magnitude < _inputDeadZone) return Vector2.zero;
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
            if (_playerController.Action1.InputValue) //ACCEPT
            {
                CharacterCheckByBouncerState chara = _currentSlot.Occupant.CurrentState as CharacterCheckByBouncerState;
                chara.BouncerAction(true);
                currentState = BouncerState.Moving;
                transform.position = _currentSlot.transform.position;
                yield break;
            }

            if (_playerController.Action3.InputValue)//REFUSE
            {
                StartQTE();
                yield break;
            }
            yield return null;
        }
    }

    public void StartQTE()
    {
        _qteHandler.StartRandomQTE();
    }

    public void OnQTEStarted(QTESequence sequence)
    {
        _text.text = _qteHandler.GetQTEString();
    }

    public void OnQTEComplete()
    {
        CharacterCheckByBouncerState chara = _currentSlot.Occupant.CurrentState as CharacterCheckByBouncerState;
        chara.BouncerAction(false);
        currentState = BouncerState.Moving;
        transform.position = _currentSlot.transform.position;
        _text.text = "";
    }

    public void OnQTECorrectInput()
    {
        _text.text = _qteHandler.GetQTEString();
    }
}
