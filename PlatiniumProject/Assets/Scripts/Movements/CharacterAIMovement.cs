using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIMovement : EntityMovement
{
    protected override IEnumerator Start()
    {
        yield return base.Start();
    }

    public override bool MoveToPosition(Vector3 position)
    {
        return base.MoveToPosition(position);
    }
}
