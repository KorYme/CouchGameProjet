using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EntityMovement : MonoBehaviour, IMovable
{
    [Header("Parameters")]
    [SerializeField] protected MovementData _movementData;

    [Header("References")]
    [SerializeField] protected Transform _transformToModify;

    protected Coroutine _movementCoroutine;
    protected Action OnMove;
    protected ITimingable _timingable => Globals.BeatManager;
    public bool IsMoving => _movementCoroutine != null;

    private Vector3 _destination;

    protected virtual float _TimeBetweenMovements => _movementData.MovementDurationPercent * _timingable.BeatDurationInMilliseconds / 1000f;
    public MovementData MovementData
    {
        get { return _movementData; }
        set { if(value != null) _movementData = value; }
    }
    
    public virtual bool MoveToPosition(Vector3 position, int animationFrames)
    {
        if (IsMoving) return false;
        _movementCoroutine = StartCoroutine(MovementCoroutineAnimation(position, OnMove, animationFrames));
        return true;
    }

    public void CorrectDestination(Vector3 newDestination)
    {
        _destination = newDestination;
    }
    
    protected virtual IEnumerator MovementCoroutineAnimation (Vector3 positionToGo, Action callBack, int animationsFrames)
    {
        float timer = 0;
        Vector3 initialPosition = _transformToModify.position;
        Vector3 initialScale = _transformToModify.localScale;
        float timeBetweenAnims = _TimeBetweenMovements / animationsFrames;
        float animTimer = 0f;
        _destination = positionToGo;
        
        callBack?.Invoke();
        
        while (timer < _TimeBetweenMovements)
        {
            timer += Time.deltaTime;
            animTimer += Time.deltaTime;

            if (animTimer >= timeBetweenAnims )
            {
                animTimer = 0f;
                callBack?.Invoke();
            }
            
            _transformToModify.position = Vector3.LerpUnclamped(initialPosition, _destination, 
                _movementData.MovementCurve.Evaluate(timer / _TimeBetweenMovements));
            
            _transformToModify.localScale = initialScale +
                (new Vector3(initialScale.x * _movementData.BounceMultiplierX, initialScale.y * _movementData.BounceMultiplierY, 0)
                 * _movementData.BounceCurve.Evaluate(timer / _TimeBetweenMovements));
            
            yield return null;
        }
        _transformToModify.localScale = initialScale;
        _transformToModify.position = _destination;
        _movementCoroutine = null;
    }
}
