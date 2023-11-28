using System.Collections;
using UnityEngine;

public class LerpTargetLight : MonoBehaviour 
{
    [SerializeField] float _timeToLerp = 0.5f;
    [SerializeField] float _distanceLightSnapToTarget = 0.2f;
    [SerializeField] int _targetIndex = 0;
    [SerializeField] Transform[] _targetPlayers;
    Coroutine _routineMove;

    public int TargetIndex { 
        get => _targetIndex;
        set { if (_targetIndex >= 0 && _targetIndex < _targetPlayers.Length)
            {
                _targetIndex = value;
                SetTargetLight();
            } 
        }
    }

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

    public IEnumerator SmoothlyMoveToIndex()
    {
        while ((transform.position - _targetPlayers[_targetIndex].position).magnitude > _distanceLightSnapToTarget)
        {
            transform.position = Vector3.Lerp(transform.position, _targetPlayers[_targetIndex].position, Time.deltaTime / _timeToLerp);
            yield return null;
        }
        transform.position = _targetPlayers[_targetIndex].position;
        _routineMove = null;
    }
    public void MoveToIndex(int indexCharacter)
    {
        if (_targetIndex >= 0 &&  indexCharacter < _targetPlayers.Length)
        {
            _targetIndex = indexCharacter;
            //SetTargetLight();
            if (_routineMove == null)
            {
                _routineMove = StartCoroutine(SmoothlyMoveToIndex());
            }
        }
    }
}
