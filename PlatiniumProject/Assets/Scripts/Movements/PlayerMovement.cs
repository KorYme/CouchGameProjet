using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    [SerializeField] protected PlayerRole _playerRole;
    protected PlayerInputController _playerController;

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => Players.PlayersController[(int)_playerRole] != null);
        _playerController = Players.PlayersController[(int)_playerRole];
    }

    public override bool MoveToPosition(Vector3 position)
    {
        return base.MoveToPosition(position);
        // A MODIFIER
    }
}
