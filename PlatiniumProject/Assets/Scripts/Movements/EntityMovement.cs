using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EntityMovement : MonoBehaviour, IMovable, IBounceable
{
    [Header("Parameters")]
    [SerializeField] MovementData _movementData;

    [Header("References")]
    [SerializeField] Transform _transformToModify;

    public float MovementDuration => _movementData.MovementDuration; 
    public int SpeedMultiplier => _movementData.SpeedMultiplier; 
    public AnimationCurve MovementCurve => _movementData.MovementCurve; 

    public AnimationCurve BounceCurve => _movementData.BounceCurve;
    public float BounceMultiplier => _movementData.BounceMultiplier;
    public Vector2 BounceFactors => _movementData.BounceFactors;


    Coroutine _movementCoroutine;
    public bool IsMoving => _movementCoroutine != null; 
    public bool IsBouncing => _movementCoroutine != null;


    protected virtual IEnumerator MovementCoroutine(Vector3 directionToGo)
    {
        float timer = 0;
        Vector3 initialPosition = _transformToModify.position;
        Vector3 initialScale = _transformToModify.localScale;
        while (timer < MovementDuration)
        {
            timer += Time.deltaTime;
            _transformToModify.position = Vector3.Lerp(initialPosition, initialPosition + directionToGo, MovementCurve.Evaluate(timer / MovementDuration));
            _transformToModify.localScale = initialScale + new Vector3(BounceCurve.Evaluate(timer / MovementDuration) * BounceFactors.x, 
                BounceCurve.Evaluate(timer / MovementDuration) * BounceFactors.y, 0) * BounceMultiplier;
            yield return null;
        }
        _transformToModify.localScale = initialScale;
        _transformToModify.position = initialPosition + directionToGo;
        _movementCoroutine = null;
    }

    public void MoveTo(Vector3 directionToGo) => _movementCoroutine = StartCoroutine(MovementCoroutine(directionToGo * SpeedMultiplier));
}
