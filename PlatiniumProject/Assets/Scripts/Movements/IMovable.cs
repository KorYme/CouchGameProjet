using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public bool IsMoving { get; }
    public float MovementDuration { get; }
    public int SpeedMultiplier { get; }
    public AnimationCurve MovementCurve { get; }
    public void MoveToPosition(Vector3 position);
}