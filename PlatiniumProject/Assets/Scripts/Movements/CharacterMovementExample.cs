using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovementExample : EntityMovement
{
    Vector2 direction = Vector2.right;

    protected void Start()
    {
        Globals.BeatTimer.OnBeatEvent.AddListener(Test);
    }

    protected void OnDestroy()
    {
        Globals.BeatTimer.OnBeatEvent.RemoveListener(Test);
    }

    void Test()
    {
        MoveToPosition((Vector2)transform.position + direction);
        direction = new Vector2(-direction.y, direction.x);
    }

    public override bool MoveToPosition(Vector3 position)
    {
         return base.MoveToPosition(position);
    }
}
