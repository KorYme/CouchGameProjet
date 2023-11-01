using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public bool IsCurrentlyDropping { get; private set; } = false;
    public void StartDrop()
    {
        if (IsCurrentlyDropping) return;
        IsCurrentlyDropping = true;
        for (int i = 0; i < Players.MAXPLAYERS; i++)
        {
            if (Players.PlayersController[i] == null) continue;

        }
    }

    public void EndDrop()
    {
        if (!IsCurrentlyDropping) return;
        IsCurrentlyDropping = false;
        for (int i = 0; i < Players.MAXPLAYERS; i++)
        {
            if (Players.PlayersController[i] == null) continue;
            Players.PlayersController[i].LT.OnInputStart += () =>
            {

            };
        }
    }
}