using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;

public enum Direction
{
    Right = 0,
    Left = 1,
    Down = 2,
    Up = 3,
}
public class DJController : MonoBehaviour,IQTEable
{
    [SerializeField] List<SlotInformation> _shapesLight;
    [SerializeField, Range(0f, 1f)] float _inputDistance = .4f;
    [SerializeField] Direction _rightJoystickClockwise = Direction.Down;
    [SerializeField] Direction _rightJoystickAntiClockwise = Direction.Up;
    [SerializeField] Direction _leftJoystickClockwise = Direction.Right;
    [SerializeField] Direction _leftJoystickAntiClockwise = Direction.Left;

    /// <summary>
    /// If rotate clockwise then 1 else -1 
    /// </summary>
    int _rotationOrientation = 0;
    int _directionChecked = 0;
    Vector2 _lastDirection = Vector2.zero;
    PlayerInputController _djInputController;
    QTEHandler _qteHandler;
    bool _areInputsSetUp = false;

    readonly Color Red = Color.red;
    readonly Color Green = new Color(0f, 1f, 1f / 18f);

    #region ToRemove
    [SerializeField] TextMeshProUGUI _QTEDisplay;

    public void OnQTEStarted(QTESequence sequence)
    {
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }

    public void OnQTEComplete()
    {
        if (CheckNbPlayersInLight() > 0)
        {
            _qteHandler.StartRandomQTE();
        }
    }

    public void OnQTECorrectInput()
    {
        foreach (SlotInformation information in _shapesLight)
        {
            if (information.Occupant != null)
            {
                CharacterStateDancing state = information.Occupant.DancingState as CharacterStateDancing;
                if (state != null)
                {
                    state.OnQTECorrectInput();
                }
            }
        }
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }
    #endregion


    //TO CHECK
    private IEnumerator Start()
    {
        UpdateLightTiles(_shapesLight);
        _qteHandler = GetComponent<QTEHandler>();
        _QTEDisplay.text = ""; 
        if (_qteHandler != null )
        {
            _qteHandler.RegisterQTEable(this);
        }
        _areInputsSetUp = false;
        yield return new WaitUntil(()=> Players.PlayersController[(int)PlayerRole.DJ] != null);
        _djInputController = Players.PlayersController[(int)PlayerRole.DJ];
        SetUpInputs();
        Debug.Log("DJ Initialis�");
    }

    //TO COMPLETE WITH OTHER INPUTS
    private void SetUpInputs()
    {
        _djInputController.LeftJoystick.OnInputChange += () =>
        {
            GetDirection(_djInputController.LeftJoystick, _leftJoystickClockwise, _leftJoystickAntiClockwise);
        };
        _djInputController.RightJoystick.OnInputChange += () =>
        {
            GetDirection(_djInputController.RightJoystick, _rightJoystickClockwise, _rightJoystickAntiClockwise);
        };
        _areInputsSetUp = true;
    }
    //TO COMPLETE WITH SETUPINPUTS
    private void OnDestroy()
    {
        if (_areInputsSetUp)
        {
            _djInputController.LeftJoystick.OnInputChange -= () =>
            {
                GetDirection(_djInputController.LeftJoystick, _leftJoystickClockwise, _leftJoystickAntiClockwise);
            };
            _djInputController.RightJoystick.OnInputChange -= () =>
            {
                GetDirection(_djInputController.RightJoystick, _rightJoystickClockwise, _rightJoystickAntiClockwise);
            };
        }
        if (_qteHandler != null)
        {
            _qteHandler.UnregisterQTEable(this);
        }
    }
    //DONE
    public void MoveLightShape(Direction direction)
    {
        if (_shapesLight.TrueForAll(x => x.Neighbours[(int)direction] != null))
        {
            List<SlotInformation> newList = new();
            _shapesLight.ForEach(x => newList.Add(x.Neighbours[(int)direction]));
            UpdateLightTiles(newList);
            _shapesLight = newList;
            if (CheckNbPlayersInLight() > 0)
            {
                _qteHandler.StartRandomQTE();
            } else
            {
                _qteHandler.StopCoroutine();
            }
        }
    }
    //DONE
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

    private void GetDirection(InputVector2 vectorInput, Direction clockwiseDirection, Direction antiClockwiseDirection)
    {
        Vector2 closestPoint = GetClosestUnitVectorFromVector(vectorInput.InputValue);
        //Debug.Log(closestPoint);
        if (_lastDirection == closestPoint) return;
        if (closestPoint == Vector2.zero)
        {
            _lastDirection = closestPoint;
            _rotationOrientation = 0;
            _directionChecked = 0;
            return;
        }
        if (closestPoint == -_lastDirection)
        {
            //Cas quasi impossible mais on sait jamais, si le joueur est ultra rapide
            _lastDirection = closestPoint;
            _rotationOrientation = 0;
            _directionChecked = 1;
            return;
        }
        if (_rotationOrientation == 0)
        {
            if (_lastDirection != Vector2.zero)
            {
                _rotationOrientation = (new Vector2(_lastDirection.y, -_lastDirection.x) == closestPoint) ? 1 : -1;
            }
        }
        else if (new Vector2(_lastDirection.y, -_lastDirection.x) * _rotationOrientation != closestPoint)
        {
            _rotationOrientation *= -1;
            _directionChecked = 1;
        }     
        _lastDirection = closestPoint;
        _directionChecked++;
        //Debug.Log($"Quart effectu�, Position actuelle - {_lastDirection}");
        if (_directionChecked >= 4)
        {
            switch (_rotationOrientation)
            {
                case 1:
                    //Clockwise
                    MoveLightShape(clockwiseDirection);
                    break;
                case -1:
                    //AntiClockwise
                    MoveLightShape(antiClockwiseDirection);
                    break;
                default:
                    break;
            }
            _directionChecked = 1;
            _rotationOrientation = 0;
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

    private Vector2 GetVectorFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return Vector2.right;
            case Direction.Left:
                return Vector2.left;
            case Direction.Down:
                return Vector2.down;
            case Direction.Up:
                return Vector2.up;
            default:
                return Vector2.zero;
        }
    }
    //Return the number of players
    private int CheckNbPlayersInLight()
    {
        int nbPlayers = 0;
        foreach (SlotInformation information in _shapesLight)
        {
            if (information.Occupant != null)
            {
                nbPlayers++;
            }
        }
        return nbPlayers;
    }

    
}
