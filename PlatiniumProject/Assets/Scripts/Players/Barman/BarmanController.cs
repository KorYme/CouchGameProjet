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

    
}
