using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DanceFloorSideArea : MonoBehaviour
{
    [SerializeField] private CheckerBoard _danceFloor;
    [SerializeField] private Vector3 _offSet;
    [SerializeField] private GameObject go;

    [SerializeField] private float _firstBoxSize;
    [SerializeField] private float _secondBoxSize;
    private Vector3 _center;

    public Vector3 GetOffDanceFloorPostion(Vector3 pos)
    {
        bool isOnSide = Random.Range(0, 2) == 0;
        int horizontalPos = pos.x > (_danceFloor.Center + _offSet).x ? 1 : -1;
        int verticalPos = pos.y > (_danceFloor.Center + _offSet).y? 1 : -1;
        
        Vector3 newPos = Vector3.zero;
        if (isOnSide)
        {
            newPos = _danceFloor.Center + _offSet + new Vector3(Random.Range(_firstBoxSize / 2, _secondBoxSize / 2) * horizontalPos,
            Random.Range(-_secondBoxSize / 2, _secondBoxSize / 2), 0);
        }
        else
        {
            newPos = _danceFloor.Center + _offSet + new Vector3(Random.Range(-_firstBoxSize / 2, _firstBoxSize / 2),
                Random.Range(_firstBoxSize / 2, _secondBoxSize / 2) * verticalPos, 0);
        }
        return newPos;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_danceFloor.Center + _offSet, Vector3.one * _firstBoxSize);
        Gizmos.DrawWireCube(_danceFloor.Center + _offSet, Vector3.one * _secondBoxSize);
    }
}
