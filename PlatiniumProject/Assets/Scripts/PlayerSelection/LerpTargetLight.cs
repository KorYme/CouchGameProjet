using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTargetLight : MonoBehaviour 
{
    [SerializeField] float _timeToLerp = 0.5f;
    [SerializeField] int _targetIndex = 0;
    [SerializeField] Transform[] _targetPlayers;
    void Start()
    {
        SetTargetLight();
    }

    public void SetTargetLight()
    {
        if ( _targetIndex >= 0 && _targetIndex < _targetPlayers.Length)
        {
            transform.position = _targetPlayers[_targetIndex].position;
        }
    }

    public void MoveToIndex(int indexCharacter)
    {
        if (_targetIndex >= 0 &&  indexCharacter < _targetPlayers.Length)
        {
            _targetIndex = indexCharacter;
            SetTargetLight();
        }
    }
}
