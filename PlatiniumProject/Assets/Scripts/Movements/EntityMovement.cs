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
    [SerializeField] protected CharacterAnimation _characterAnimation;
    protected ITimingable _timingable => Globals.BeatManager;
    public bool IsMoving => _movementCoroutine != null;

    private Vector3 _destination;

    private void Awake()
    {
        _characterAnimation = GetComponent<CharacterAnimation>();
    }

    protected virtual float _TimeBetweenMovements => _movementData.MovementDurationPercent * (_timingable.BeatDurationInMilliseconds / 1000f);
    public MovementData MovementData
    {
        get { return _movementData; }
        set { if(value != null) _movementData = value; }
    }
    
    public virtual bool MoveToPosition(Vector3 position)
    {
        if (IsMoving) return false;
        _movementCoroutine = StartCoroutine(MovementCoroutineAnimation(position));
        return true;
    }

    public void CorrectDestination(Vector3 newDestination)
    {
        _destination = newDestination;
    }
    
    protected virtual IEnumerator MovementCoroutineAnimation (Vector3 positionToGo)
    {
        float timer = 0;
        Vector3 initialPosition = _transformToModify.position;
        Vector3 initialScale = _transformToModify.localScale;
        _destination = positionToGo;

        _characterAnimation.SetFullAnim(ANIMATION_TYPE.MOVE, _movementData.MovementDurationPercent * (Globals.BeatManager.BeatDurationInMilliseconds / 1000f));
        
        while (timer < _TimeBetweenMovements)
        {
            timer += Time.deltaTime;
            
            _transformToModify.position = Vector3.LerpUnclamped(initialPosition, _destination, 
                _movementData.MovementCurve.Evaluate(timer / _TimeBetweenMovements));
            
            _transformToModify.localScale = initialScale +
                (new Vector3(initialScale.x * _movementData.BounceMultiplierX, initialScale.y * _movementData.BounceMultiplierY, 0)
                 * _movementData.BounceCurve.Evaluate(timer / _TimeBetweenMovements));
            
            yield return null;
        }

        yield return new WaitUntil(() => !_characterAnimation.IsAnimationPlaying);
        _transformToModify.localScale = initialScale;
        _transformToModify.position = _destination;
        _movementCoroutine = null;
    }

    // private IEnumerator AnimationRoutine(int loop, float duration, Action callBack)
    // {
    //     for (int i = 0; i < loop - 1; i++)
    //     {
    //         callBack?.Invoke();
    //         yield return new WaitForSeconds(duration);
    //     }
    //     _animRoutine = null;
    // }
}
