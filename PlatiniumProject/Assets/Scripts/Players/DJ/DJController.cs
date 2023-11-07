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

public class DJController : MonoBehaviour, IQTEable
{
    [SerializeField] List<SlotInformation> _shapesLight;
    [SerializeField, Range(0f, 1f)] float _inputDistance = .4f;
    [SerializeField] Direction _rightJoystickClockwise = Direction.Down;
    [SerializeField] Direction _rightJoystickAntiClockwise = Direction.Up;
    [SerializeField] Direction _leftJoystickClockwise = Direction.Right;
    [SerializeField] Direction _leftJoystickAntiClockwise = Direction.Left;

    PlayerInputController _djInputController;
    QTEHandler _qteHandler;

    RollInputChecker _rollRightJoystick;
    RollInputChecker _rollLeftJoystick;

    #region ToRemove
    [SerializeField] TextMeshProUGUI _QTEDisplay;

    public void OnQTEStarted(QTESequence sequence)
    {
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }

    public void OnQTEComplete()
    {
        _QTEDisplay.text = _qteHandler.GetQTEString();
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
                    state.OnQTECorrectInput(_qteHandler.LengthInputs);
                }
            }
        }
        _QTEDisplay.text = _qteHandler.GetQTEString();
    }
    #endregion
    //TO CHECK
    private IEnumerator Start()
    {
        _qteHandler = GetComponent<QTEHandler>();
        UpdateLightTiles(_shapesLight);
        if (_qteHandler != null )
        {
            _qteHandler.RegisterQTEable(this);
        }
        _QTEDisplay.text = _qteHandler.GetQTEString();
        yield return new WaitUntil(()=> Players.PlayersController[(int)PlayerRole.DJ] != null);
        _djInputController = Players.PlayersController[(int)PlayerRole.DJ];
        SetUpInputs();
        Debug.Log("DJ Initialise");
    }

    //TO COMPLETE WITH OTHER INPUTS
    private void SetUpInputs()
    {
        _rollLeftJoystick = new RollInputChecker(_djInputController.LeftJoystick, _inputDistance);
        _rollRightJoystick = new RollInputChecker(_djInputController.RightJoystick, _inputDistance);
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
            UpdateQTE();
        }
    }

    private void UpdateQTE()
    {
        if (NbPlayersInLight() > 0)
        {
            CharacterTypeData[] clientsData = new CharacterTypeData[NbPlayersInLight()];
            int index = 0;
            foreach(SlotInformation info in _shapesLight)
            {
                if (info.Occupant != null)
                {
                    clientsData[index] = info.Occupant.TypeData;
                    index++;
                }
            }
            _qteHandler.StartNewQTE(clientsData);
        }
        else
        {
            _qteHandler.DeleteCurrentCoroutine();
        }
        _QTEDisplay.text = _qteHandler.GetQTEString();
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
            if (slot.GetComponent<SlotRenderer>() != null)
            {
                slot.GetComponent<SlotRenderer>().ChangeColor(slot.IsEnlighted);
            } else
            {
                slot.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
        foreach (SlotInformation slot in newSlots)
        {
            slot.OnOccupantChanges += DeactivateQTE;
            slot.IsEnlighted = true;
            if (slot.GetComponent<SlotRenderer>() != null)
            {
                slot.GetComponent<SlotRenderer>().ChangeColor(slot.IsEnlighted);
            }
            else
            {
                slot.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        
    }

    //Return the number of players
    private int NbPlayersInLight()
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

    private void DeactivateQTE()
    {
        UpdateQTE();
    }
}
