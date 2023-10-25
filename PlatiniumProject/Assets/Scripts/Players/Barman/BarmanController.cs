using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class BarmanController : MonoBehaviour
{
    BarmanMovement _barmanMovement;

    public void Awake()
    {
        _barmanMovement = GetComponent<BarmanMovement>();
    }

    public void HandleQTE(CallbackContext context)
    {
        if (context.performed && _barmanMovement.IsInputDuringBeatTime())
        {
            _barmanMovement.BarmanPositions[_barmanMovement.IndexPosition].WaitingLine.CheckInputFromLine(context.control.name);
        }
    }
}
