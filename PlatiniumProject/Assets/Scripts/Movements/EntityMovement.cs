using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityMovement : MonoBehaviour, IMovable
{
    [Header("Parameters")]
    [SerializeField] protected MovementData _movementData;

    [Header("References")]
    [SerializeField] protected Transform _transformToModify;

    protected Coroutine _movementCoroutine;
    protected ITimingable _timingable;
    public bool IsMoving => _movementCoroutine != null;

    protected virtual float _TimeBetweenMovements => _movementData.MovementDurationPercent * _timingable.BeatDurationInMilliseconds / 1000f;

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => Globals.BeatTimer  != null);
        _timingable = Globals.BeatTimer;
    }
    public virtual bool MoveToPosition(Vector3 position)
    {
        if (IsMoving) return false;
        _movementCoroutine = StartCoroutine(MovementCoroutine(position));
        return true;
    }

    protected virtual IEnumerator MovementCoroutine(Vector3 positionToGo)
    {
        float timer = 0;
        Vector3 initialPosition = _transformToModify.position;
        Vector3 initialScale = _transformToModify.localScale;
        while (timer < _TimeBetweenMovements)
        {
            timer += Time.deltaTime;
            _transformToModify.position = Vector3.LerpUnclamped(initialPosition, positionToGo, 
                _movementData.MovementCurve.Evaluate(timer / _TimeBetweenMovements));
            _transformToModify.localScale = initialScale + 
                (new Vector3(initialScale.x * _movementData.BounceMultiplierX, initialScale.y * _movementData.BounceMultiplierY, 0) 
                * _movementData.BounceCurve.Evaluate(timer / _TimeBetweenMovements));
            yield return null;
        }
        _transformToModify.localScale = initialScale;
        _transformToModify.position = positionToGo;
        _movementCoroutine = null;
    }
}
