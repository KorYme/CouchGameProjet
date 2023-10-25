using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class DJController : MonoBehaviour
{
    [SerializeField] List<SlotInformation> _shapesLight;
    PlayerInputController _djInputController;

    readonly Color Red = Color.red;
    readonly Color Green = new Color(0f, 1f, 1f / 18f);

    Coroutine _movementCoroutine;

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
        yield return new WaitUntil(()=> Players.PlayersController[(int)PlayerRole.Bouncer] != null);
        Debug.Log("Initialisé");
        _djInputController = Players.PlayersController[(int)PlayerRole.DJ];
        _djInputController.LeftJoystick.OnInputStart += () => Debug.Log("DEBUG");
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
