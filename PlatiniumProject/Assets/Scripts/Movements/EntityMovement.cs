using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityMovement : MonoBehaviour, IMovable, IBounceable
{
    [Header("Parameters")]
    [SerializeField] MovementData _movementData;

    [Header("References")]
    [SerializeField] Transform _transformToModify;

    Coroutine _movementCoroutine;
    public bool IsMoving => _movementCoroutine != null; 
    public bool IsBouncing => _movementCoroutine != null;
    public bool HasAlreadyMovedThisBeat { get; protected set; }

    protected virtual void Start()
    {
        HasAlreadyMovedThisBeat = false;
        Globals.BeatTimer.OnBeatStartEvent.AddListener(AllowNewInput);
    }

    protected virtual void OnDestroy()
    {
        Globals.BeatTimer.OnBeatStartEvent.RemoveListener(AllowNewInput);
    }

    private void AllowNewInput() => HasAlreadyMovedThisBeat = false;

    protected virtual IEnumerator MovementCoroutine(Vector3 positionToGo)
    {
        float timer = 0;
        Vector3 initialPosition = _transformToModify.position;
        Vector3 initialScale = _transformToModify.localScale;
        while (timer < _movementData.MovementDurationPercent)
        {
            timer += Time.deltaTime;
            _transformToModify.position = Vector3.LerpUnclamped(initialPosition, positionToGo, 
                _movementData.MovementCurve.Evaluate(timer / _movementData.MovementDurationPercent));
            _transformToModify.localScale = initialScale + 
                (new Vector3(initialScale.x * _movementData.BounceMultiplierX, initialScale.y * _movementData.BounceMultiplierY, 0) 
                * _movementData.BounceCurve.Evaluate(timer / _movementData.MovementDurationPercent));
            yield return null;
        }
        _transformToModify.localScale = initialScale;
        _transformToModify.position = positionToGo;
        _movementCoroutine = null;
    }

    public virtual void MoveToPosition(Vector3 position)
    {
        //Debug.Log($"{name} - {HasAlreadyMovedThisBeat}");
        if (HasAlreadyMovedThisBeat || IsMoving) return;
        _movementCoroutine = StartCoroutine(MovementCoroutine(position));
        HasAlreadyMovedThisBeat = true;
    }
}
