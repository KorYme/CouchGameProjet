using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    public override bool MoveToPosition(Vector3 position)
    {
        return base.MoveToPosition(position);
        // A MODIFIER
    }
}
