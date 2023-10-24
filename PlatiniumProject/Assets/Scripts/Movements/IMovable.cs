using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovable
{
    public bool IsMoving { get; }
    public bool HasAlreadyMovedThisBeat { get; }
    public void MoveToPosition(Vector3 position);
}