using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class BarmanMovement : MonoBehaviour
{
    [SerializeField] Transform[] _barmanPositions;
    [SerializeField,Range(0f,1f)] float _inputAcceptanceThreshold = 0.1f;
    int _indexPosition;
    private void Awake()
    {
        _indexPosition = 0;
        if (_barmanPositions.Length > 0)
        {
            MoveBarmanToIndex();
        }
    }

    public void MoveBarmanToIndex()
    {
        transform.position = _barmanPositions[_indexPosition].position;
    }

    public void OnMovementInput(CallbackContext context)
    {
        if (context.started)
        {
            float value = context.ReadValue<float>();
            if (value > 1f - _inputAcceptanceThreshold)
            {
                if (_indexPosition < _barmanPositions.Length - 1)
                {
                    _indexPosition++;
                }
                MoveBarmanToIndex();
            }
            else if (value < -1f + _inputAcceptanceThreshold)
            {
                if (_indexPosition > 0)
                {
                    _indexPosition--;
                }
                MoveBarmanToIndex();
            }
        }
    }
}
