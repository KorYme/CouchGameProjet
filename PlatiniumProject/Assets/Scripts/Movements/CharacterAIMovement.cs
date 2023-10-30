using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIMovement : EntityMovement
{
    protected virtual void Start()
    {
        HasAlreadyMovedThisBeat = false;
        Globals.BeatTimer.OnBeatStartEvent.AddListener(AllowNewInput);
    }

    protected virtual void OnDestroy()
    {
        Globals.BeatTimer.OnBeatStartEvent.RemoveListener(AllowNewInput);
    }

    public override bool MoveToPosition(Vector3 position)
    {
        return base.MoveToPosition(position);
        // A MODIFIER
    }
}
